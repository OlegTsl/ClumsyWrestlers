using System;

namespace Game.Core.Character
{
    public interface ICharacterRuntime : IDisposable
    {
        ICharacterModel Model { get; }
        ICharacterView View { get; }
    }
}
