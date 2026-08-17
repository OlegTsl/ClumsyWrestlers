using UnityEngine;

namespace Game.Core.Level.Entities
{
    public interface IImpulseReceiverView
    {
        void ApplyImpulse(Vector3 impulse);
    }
}
