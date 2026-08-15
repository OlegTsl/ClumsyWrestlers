namespace Game.Core.Systems
{
    internal sealed class PowerAttackState
    {
        public bool  IsAttacking { get; private set; }
        public float Elapsed     { get; set; }

        public void StartAttack()
        {
            IsAttacking = true;
            Elapsed     = 0f;
        }

        public void EndAttack()
        {
            IsAttacking = false;
            Elapsed     = 0f;
        }
    }
}
