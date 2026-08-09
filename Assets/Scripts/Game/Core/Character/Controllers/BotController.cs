namespace Game.Core.Character
{
    public sealed class BotController : IBotController
    {
        private readonly ICharacterModel _model;

        public BotController(
            ICharacterModel model
        )
        {
            _model = model;
        }

        public void Dispose()
        {
        }
    }
}