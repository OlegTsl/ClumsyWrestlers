using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Level;
using UnityEngine;
using Zenject;

namespace Game.Core.Round
{
    public sealed class RoundController : IRoundController, ILateTickable
    {
        private const string CharacterAddress = "Wrestler";

        private readonly ILevelController _levelController;
        private readonly ICharacterBuilder _characterBuilder;
        private readonly IInputEventsBus _inputEvents;
        private readonly ICharacterCommandSink _commandSink;
        private readonly ICharacterCommandBuffer _commandBuffer;
        private readonly ISimulationClock _clock;
        private readonly IRoundSystemsScope _systemsScope;

        private CancellationTokenSource _roundCancellation;
        private ICharacterController _playerController;
        private ICharacterRuntime _player;
        private ICharacterRuntime _enemy;
        private uint _generation;

        public RoundController(
            ILevelController levelController,
            ICharacterBuilder characterBuilder,
            IInputEventsBus inputEvents,
            ICharacterCommandSink commandSink,
            ICharacterCommandBuffer commandBuffer,
            ISimulationClock clock,
            IRoundSystemsScope systemsScope
        )
        {
            _levelController = levelController;
            _characterBuilder = characterBuilder;
            _inputEvents = inputEvents;
            _commandSink = commandSink;
            _commandBuffer = commandBuffer;
            _clock = clock;
            _systemsScope = systemsScope;
        }

        public async UniTask StartRoundAsync(
            string levelAddress,
            CancellationToken cancellationToken
        )
        {
            EndRound();
            uint generation = ++_generation;
            _roundCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);
            CancellationTokenSource currentCancellation = _roundCancellation;
            CancellationToken roundToken = currentCancellation.Token;
            ICharacterRuntime player = null;
            ICharacterRuntime enemy = null;

            try
            {
                await _levelController.LoadLevelAsync(levelAddress, roundToken);
                player = await _characterBuilder.BuildCharacterAsync(
                    CharacterAddress,
                    roundToken);
                enemy = await _characterBuilder.BuildCharacterAsync(
                    CharacterAddress,
                    roundToken);
                roundToken.ThrowIfCancellationRequested();

                _player = player;
                _enemy = enemy;
                player = null;
                enemy = null;

                _levelController.SpawnCharacter(_player.Model, true);
                _levelController.SpawnCharacter(_enemy.Model, false);
                _systemsScope.StartRound();

                Camera mainCamera = Camera.main;
                _playerController = new Game.Core.Character.CharacterController(
                    _player.Model,
                    _player.View,
                    _inputEvents,
                    _commandSink,
                    _clock,
                    mainCamera.transform);
            }
            catch
            {
                player?.Dispose();
                enemy?.Dispose();
                if (_generation == generation &&
                    ReferenceEquals(_roundCancellation, currentCancellation))
                {
                    EndRound();
                }

                throw;
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

            _systemsScope.EndRound();

            _player?.Dispose();
            _player = null;
            _enemy?.Dispose();
            _enemy = null;

            _levelController.UnloadLevel();
            _commandBuffer.Clear();
            _clock.Reset();
        }

        public void LateTick()
            => _playerController?.LateTick();

        public void Dispose()
            => EndRound();
    }
}
