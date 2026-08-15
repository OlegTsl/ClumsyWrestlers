using Game.Core.Character;

namespace Game.Core.Systems
{
    internal sealed class SimpleAttackState
    {
        public bool IsInputPressed { get; set; }
        public float Elapsed       { get; set; }
        public bool IsAttacking    { get; private set; }
        public bool IsMirrored     { get; private set; }

        public AttackHand ActiveHand => IsMirrored
            ? AttackHand.Left : AttackHand.Right;

        public void StartAttack()
        {
            IsAttacking = true;
            Elapsed     = 0f;
        }

        public void FinishAttack()
        {
            IsAttacking = false;
            Elapsed     = 0f;
        }

        public void ToggleHand()
            => IsMirrored = !IsMirrored;

        public void Reset()
        {
            IsAttacking           = false;
            IsInputPressed        = false;
            IsMirrored            = false;
            Elapsed               = 0f;
        }
    }
}
