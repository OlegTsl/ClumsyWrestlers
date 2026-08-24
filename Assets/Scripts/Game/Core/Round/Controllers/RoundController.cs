using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Common.Input;
using Game.Core.Bots;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Extension;
using Game.Core.Level;
using Game.Core.Teams;
using UnityEngine;
using Zenject;

namespace Game.Core.Round
{
    public sealed class RoundController : IRoundController, ILateTickable
    {
        private const string CRoundConfigName = "RoundConfiguration";

        private readonly ILevelController         _levelController;
        private readonly ICharacterBuilder        _characterBuilder;
        private readonly IInputEventsBus          _inputEvents;
        private readonly ICharacterCommandSink    _commandSink;
        private readonly ICharacterCommandBuffer  _commandBuffer;
        private readonly ISimulationClock         _clock;
        private readonly IRoundSystemsScope       _systemsScope;
        private readonly IBotDecisionScheduler    _botScheduler;
        private readonly IBotDecisionAgentFactory _botFactory;
        private readonly IAssetManager            _assetManager;
        private readonly List<ICharacterRuntime>  _characters    = new(16);
        private readonly List<ICharacterRuntime>  _botCharacters = new(15);
        private readonly List<IBotDecisionAgent>  _botAgents     = new(15);

        private CancellationTokenSource _roundCancellation;
        private ICharacterController    _playerController;
        private ICharacterRuntime       _player;
        private IAssetLease<RoundData>  _configLease;
        private uint                    _generation;

        public RoundController(
            ILevelController         levelController,
            ICharacterBuilder        characterBuilder,
            IInputEventsBus          inputEvents,
            ICharacterCommandSink    commandSink,
            ICharacterCommandBuffer  commandBuffer,
            ISimulationClock         clock,
            IRoundSystemsScope       systemsScope,
            IBotDecisionScheduler    botScheduler,
            IBotDecisionAgentFactory botFactory,
            IAssetManager            assetManager
        )
        {
            _levelController  = levelController;
            _characterBuilder = characterBuilder;
            _inputEvents      = inputEvents;
            _commandSink      = commandSink;
            _commandBuffer    = commandBuffer;
            _clock            = clock;
            _systemsScope     = systemsScope;
            _botScheduler     = botScheduler;
            _botFactory       = botFactory;
            _assetManager     = assetManager;
        }

        public async UniTask StartRoundAsync(string levelAddress, CancellationToken cancellationToken)
        {
            EndRound();

            uint generation    = ++_generation;
            _roundCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            CancellationTokenSource currentCancellation = _roundCancellation;
            CancellationToken       roundToken          = currentCancellation.Token;

            try
            {
                await _levelController.LoadLevelAsync(levelAddress, roundToken);
                roundToken.ThrowIfCancellationRequested();

                _configLease = await _assetManager.LoadAssetAsync<RoundData>(CRoundConfigName, roundToken);
                _player      = await BuildCharacterAsync(_configLease.Asset.PlayerTeamId, false, roundToken);
                
                await BuildBotsAsync(_configLease.Asset.AlliedBotCount,   _configLease.Asset.PlayerTeamId,   roundToken);
                await BuildBotsAsync(_configLease.Asset.OpponentBotCount, _configLease.Asset.OpponentTeamId, roundToken);
                roundToken.ThrowIfCancellationRequested();

                SpawnCharacters();
                _systemsScope.StartRound();
                CreateBotAgents();

                Camera mainCamera = Camera.main;
                _playerController = new Character.CharacterController(
                    _player.Model, _player.View, _inputEvents, _commandSink, _clock, mainCamera.transform);
            }
            catch
            {
                if (_generation == generation && ReferenceEquals(_roundCancellation, currentCancellation))
                    EndRound();

                throw;
            }
        }

        private async UniTask<ICharacterRuntime> BuildCharacterAsync(TeamId teamId, bool isBot, CancellationToken cancellationToken)
        {
            ICharacterRuntime pendingCharacter = null;
            try
            {
                pendingCharacter = await _characterBuilder.BuildCharacterAsync(
                    _configLease.Asset.CharacterAddress, teamId, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                ICharacterRuntime character = pendingCharacter;
                pendingCharacter = null;
                _characters.Add(character);
                
                if (isBot)
                    _botCharacters.Add(character);

                return character;
            }
            finally
            {
                pendingCharacter?.Dispose();
            }
        }

        private async UniTask BuildBotsAsync(
            int count,
            TeamId teamId,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < count; i++)
            {
                await BuildCharacterAsync(teamId, true, cancellationToken);
            }
        }

        private void SpawnCharacters()
        {
            int playerTeamSpawnIndex = 0;
            int opponentTeamSpawnIndex = 0;
            for (int i = 0; i < _characters.Count; i++)
            {
                ICharacterRuntime character = _characters[i];
                TeamId teamId = character.Model.GetState<ICharacterTeamState>().TeamId;
                
                bool isPlayerTeam = teamId == _configLease.Asset.PlayerTeamId;
                int spawnIndex = isPlayerTeam ? playerTeamSpawnIndex++ : opponentTeamSpawnIndex++;
                
                _levelController.SpawnCharacter(character.Model, isPlayerTeam, spawnIndex);
            }
        }

        private void CreateBotAgents()
        {
            ILevelEntityRegistry levelEntities = (ILevelEntityRegistry)_levelController.Level;
            ILevelArenaData arena = (ILevelArenaData)_levelController.Level;
            
            for (int i = 0; i < _botCharacters.Count; i++)
            {
                ICharacterRuntime character = _botCharacters[i];
                IBotDecisionAgent agent = _botFactory.Create(
                    character.Model, character.View.BotNavigationAgent,
                    levelEntities, arena, _configLease.Asset.BotBehaviorSettings);
                
                _botAgents.Add(agent);
                _botScheduler.Register(agent);
            }
        }

        public void EndRound()
        {
            _generation++;

            CancellationTokenSource cancellation = _roundCancellation;
            _roundCancellation = null;
            
            cancellation?.Cancel();
            cancellation?.Dispose();

            _playerController?.Dispose();
            _playerController = null;

            for (int i = _botAgents.Count - 1; i >= 0; i--)
            {
                IBotDecisionAgent agent = _botAgents[i];
                _botScheduler.Unregister(agent);
                agent.Dispose();
            }

            _botAgents.Clear();
            _botCharacters.Clear();
            _systemsScope.EndRound();

            for (int i = _characters.Count - 1; i >= 0; i--)
            {
                _characters[i].Dispose();
            }

            _characters.Clear();
            _player = null;
            _levelController.UnloadLevel();
            _commandBuffer.Clear();
            _clock.Reset();

            _configLease?.Dispose();
            _configLease = null;
        }

        public void LateTick()
            => _playerController?.LateTick();

        public void Dispose()
            => EndRound();
    }
}
