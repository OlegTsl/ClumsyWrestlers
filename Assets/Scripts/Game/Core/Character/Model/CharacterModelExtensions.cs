using System;

namespace Game.Core.Character
{
    public static class CharacterModelExtensions
    {
        public static TState GetState<TState>(this ICharacterModel model)
            where TState : class
        {
            if (model is TState state)
            {
                return state;
            }

            throw new InvalidOperationException(
                $"Character {model.CharacterID} does not provide {typeof(TState).Name}.");
        }
    }
}
