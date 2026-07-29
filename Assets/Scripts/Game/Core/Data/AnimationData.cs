using UnityEngine;

namespace Game.Core.Data
{
    public static class AnimationData
    {
        public static readonly int MoveX             = Animator.StringToHash("MoveX");
        public static readonly int MoveY             = Animator.StringToHash("MoveY"); 
        public static readonly int Speed             = Animator.StringToHash("Speed");
        public static readonly int Grounded          = Animator.StringToHash("Grounded");
        public static readonly int MoveInput         = Animator.StringToHash("MoveInput");
        public static readonly int FallTrigger       = Animator.StringToHash("Fall");
        public static readonly int JumpTrigger       = Animator.StringToHash("Jump");
        public static readonly int PunchTrigger      = Animator.StringToHash("Punch");
        public static readonly int PowerPunchTrigger = Animator.StringToHash("PowerPunch");
    }
}