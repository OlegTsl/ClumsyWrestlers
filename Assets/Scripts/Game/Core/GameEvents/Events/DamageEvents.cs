using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnDamageEvent
    {
        public readonly Guid AttackerID { get; }
        public readonly Guid TargetID   { get; }
        public readonly float Force     { get; }
        public OnDamageEvent(Guid attackerID, Guid targetID, float force)
        {
            AttackerID = attackerID;
            TargetID   = targetID;
            Force      = force;
        }
    }

    public readonly struct OnApplyDamageEvent
    {
        public readonly Guid AttackerID { get; }
        public readonly Guid TargetID   { get; }
        public readonly float Force     { get; }
        public OnApplyDamageEvent(Guid attackerID, Guid targetID, float force)
        {
            AttackerID = attackerID;
            TargetID   = targetID;
            Force      = force;
        }
    }
}