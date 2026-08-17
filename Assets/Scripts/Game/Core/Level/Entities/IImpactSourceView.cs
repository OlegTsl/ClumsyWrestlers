using UnityEngine;

namespace Game.Core.Level.Entities
{
    public interface IImpactSourceView
    {
        void DampenAfterImpact(Vector3 impactVelocity);
    }
}
