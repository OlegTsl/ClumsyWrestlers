using Game.Core.Character;

namespace Game.Core.Bots
{
    public interface IBotUtilityEvaluator
    {
        BotIntent Evaluate(
            in BotWorldState world,
            ICharacterModel character);
    }
}
