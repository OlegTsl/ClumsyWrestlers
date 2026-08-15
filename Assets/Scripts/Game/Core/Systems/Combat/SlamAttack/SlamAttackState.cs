namespace Game.Core.Systems
{
    internal sealed class SlamAttackState
    {
        public bool IsActive      { get; private set; }
        public bool IsWaveApplied { get; private set; }
        public float Elapsed      { get; set; }

        public void Begin()
        {
            IsActive      = true;
            IsWaveApplied = false;
            Elapsed       = 0f;
        }

        public void MarkWaveApplied()
            => IsWaveApplied = true;

        public void End()
        {
            IsActive      = false;
            IsWaveApplied = false;
            Elapsed       = 0f;
        }
    }
}
