using Game.Common.AssetsManager;

namespace Game.Core.Character
{
    public sealed class CharacterRuntime : ICharacterRuntime
    {
        private readonly ICharacterContext _characterContext;
        private readonly ICharacterViewContext _viewContext;
        private IViewLease<CharacterView> _viewLease;

        public ICharacterModel Model { get; }
        public ICharacterView View => _viewLease?.View;

        public CharacterRuntime(
            ICharacterModel model,
            IViewLease<CharacterView> viewLease,
            ICharacterContext characterContext,
            ICharacterViewContext viewContext
        )
        {
            Model = model;
            _viewLease = viewLease;
            _characterContext = characterContext;
            _viewContext = viewContext;

            _characterContext.AddCharacter(model);
            _viewContext.AddView(model.CharacterID, viewLease.View);
        }

        public void Dispose()
        {
            IViewLease<CharacterView> viewLease = _viewLease;
            if (viewLease == null)
            {
                return;
            }

            _viewLease = null;
            _characterContext.RemoveCharacter(Model.CharacterID);
            _viewContext.RemoveView(Model.CharacterID);
            viewLease.Dispose();
        }
    }
}
