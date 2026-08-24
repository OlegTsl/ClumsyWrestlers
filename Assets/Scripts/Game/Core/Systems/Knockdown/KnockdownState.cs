namespace Game.Core.Systems
{
    internal enum KnockdownPhase
    {
        None,
        AwaitingFallBack,
        Falling,
        GettingUp
    }

    internal sealed class KnockdownState
    {
        public KnockdownPhase Phase { get; set; }
        public bool IsActive => Phase != KnockdownPhase.None;
    }
}
