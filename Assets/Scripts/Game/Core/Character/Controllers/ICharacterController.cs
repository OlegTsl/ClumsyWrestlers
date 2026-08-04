using System;

namespace Game.Core.Character
{
    public interface ICharacterController : IDisposable
    {
        void LateTick();
    }
}