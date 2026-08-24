using System;
using Game.Core.Bots;
using Game.Core.Teams;
using UnityEngine;

namespace Game.Core.Round
{
    [CreateAssetMenu(fileName = "RoundData", menuName = "Game/Round/Configuration")]
    public sealed class RoundData : ScriptableObject
    {
        [Header("Character")]
        [SerializeField] private string _characterAddress = "Wrestler";

        [Header("Teams")]
        [SerializeField,      Min(1)] private uint _playerTeamId   = 1u;
        [SerializeField,      Min(1)] private uint _opponentTeamId = 2u;
        [SerializeField, Range(0, 5)] private int _alliedBotCount;
        [SerializeField, Range(0, 5)] private int _opponentBotCount = 1;

        [Header("Bot Difficulty")]
        [SerializeField] private BotBehaviorProfile _botBehaviorProfile;

        public string               CharacterAddress    => _characterAddress;
        public TeamId               PlayerTeamId        => new(_playerTeamId);
        public TeamId               OpponentTeamId      => new(_opponentTeamId);
        public int                  AlliedBotCount      => _alliedBotCount;
        public int                  OpponentBotCount    => _opponentBotCount;
        public IBotBehaviorSettings BotBehaviorSettings => _botBehaviorProfile;
    }
}
