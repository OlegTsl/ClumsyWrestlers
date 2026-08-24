using UnityEngine;

namespace Game.Core.Data
{
    public static class AnimationData
    {
        public const string KnockdownLayer = "Knockdown";

        public static readonly int Speed              = Animator.StringToHash("Speed");
        public static readonly int SimpleAttackSpeed  = Animator.StringToHash("SimpleAttackSpeed");
        public static readonly int PowerAttackSpeed   = Animator.StringToHash("PowerAttackSpeed");
        public static readonly int Grounded           = Animator.StringToHash("Grounded");
        public static readonly int MoveInput          = Animator.StringToHash("MoveInput");
        public static readonly int FallTrigger        = Animator.StringToHash("Fall");
        public static readonly int JumpTrigger        = Animator.StringToHash("Jump");
        public static readonly int PunchTrigger       = Animator.StringToHash("Punch");
        public static readonly int MirrorPunch        = Animator.StringToHash("MirrorPunch");
        public static readonly int PowerAttackTrigger = Animator.StringToHash("PowerAttack");
        public static readonly int IsPowerAttacking   = Animator.StringToHash("IsPowerAttacking");
        public static readonly int HitTrigger         = Animator.StringToHash("Hit");
        public static readonly int Blocked            = Animator.StringToHash("Blocked");
        public static readonly int KnockdownTrigger   = Animator.StringToHash("Knockdown");
        public static readonly int FallBackState      = Animator.StringToHash("FallBack");
        public static readonly int GetUpState         = Animator.StringToHash("GetUp");
    }
}
