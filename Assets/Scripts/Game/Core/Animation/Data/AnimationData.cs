using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Animation
{
    public static class AnimationData
    {
        public static readonly int MoveX             = Animator.StringToHash("MoveX");
        public static readonly int MoveY             = Animator.StringToHash("MoveY"); 
        public static readonly int Speed             = Animator.StringToHash("Speed");
        public static readonly int Grounded          = Animator.StringToHash("Grounded");
        public static readonly int Moving            = Animator.StringToHash("Moving");
        public static readonly int JumpTrigger       = Animator.StringToHash("Jump");
        public static readonly int PunchTrigger      = Animator.StringToHash("Punch");
        public static readonly int PowerPunchTrigger = Animator.StringToHash("PowerPunch");
    }
}