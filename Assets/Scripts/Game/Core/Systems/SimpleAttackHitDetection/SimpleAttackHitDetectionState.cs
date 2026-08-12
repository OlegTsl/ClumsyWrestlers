namespace Game.Core.Systems
{
    internal sealed class SimpleAttackHitDetectionState
    {
        public bool  IsActive { get; set; }
        public float Elapsed  { get; set; }

        public void Reset()
        {
            IsActive = false;
            Elapsed  = 0f;
        }
    }
}
