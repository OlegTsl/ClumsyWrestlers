using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Common.Input;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterContextBuilder
    {
        UniTask<ICharacterContext> BuildCharacterContext(
            string                    name,
            bool                      isPlayer,
            Vector3                   position,
            Quaternion                rotation,
            IEnumerable<IInputSource> inputSources);
    }
}