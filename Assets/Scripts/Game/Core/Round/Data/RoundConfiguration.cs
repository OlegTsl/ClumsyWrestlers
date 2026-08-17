using System;
using Game.Core.Bots;
using Game.Core.Teams;
using UnityEngine;

namespace Game.Core.Round
{
    [CreateAssetMenu(
        fileName = "RoundConfiguration",
        menuName = "Game/Round/Configuration")]
    public sealed class RoundConfiguration : ScriptableObject, IRoundConfiguration
    {
        [Header("Character")]
        [SerializeField] private string _characterAddress = "Wrestler";

        [Header("Teams")]
        [SerializeField, Min(1)] private uint _playerTeamId = 1u;
        [SerializeField, Min(1)] private uint _opponentTeamId = 2u;
        [SerializeField, Range(0, 7)] private int _alliedBotCount;
        [SerializeField, Range(1, 8)] private int _opponentBotCount = 1;

        [Header("Bot Difficulty")]
        [SerializeField] private BotBehaviorProfile _botBehaviorProfile;

        public string CharacterAddress => _characterAddress;
        public TeamId PlayerTeamId => new(_playerTeamId);
        public TeamId OpponentTeamId => new(_opponentTeamId);
        public int AlliedBotCount => _alliedBotCount;
        public int OpponentBotCount => _opponentBotCount;
        public IBotBehaviorSettings BotBehaviorSettings => _botBehaviorProfile;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(_characterAddress))
            {
                throw new InvalidOperationException(
                    "Round character address must not be empty.");
            }

            if (!PlayerTeamId.IsValid || !OpponentTeamId.IsValid)
            {
                throw new InvalidOperationException(
                    "Round team ids must be valid.");
            }

            if (PlayerTeamId == OpponentTeamId)
            {
                throw new InvalidOperationException(
                    "Player and opponent teams must have different ids.");
            }

            if (_alliedBotCount is < 0 or > 7 ||
                _opponentBotCount is < 1 or > 8)
            {
                throw new InvalidOperationException(
                    "Round bot counts are outside the supported range.");
            }

            if (_botBehaviorProfile == null)
            {
                throw new InvalidOperationException(
                    "Round bot behavior profile is not assigned.");
            }

            _botBehaviorProfile.Validate();
        }
    }
}
