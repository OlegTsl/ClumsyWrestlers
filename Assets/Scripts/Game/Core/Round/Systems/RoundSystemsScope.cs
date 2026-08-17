using Game.Core.Level;
using Game.Core.Systems;
using Zenject;

namespace Game.Core.Round
{
    public sealed class RoundSystemsScope :
        IRoundSystemsScope,
        ITickable,
        IFixedTickable,
        ILateTickable
    {
        private readonly DiContainer _rootContainer;
        private readonly ILevelController _levelController;

        private DiContainer _roundContainer;
        private Kernel      _roundKernel;

        public RoundSystemsScope(
            DiContainer rootContainer,
            ILevelController levelController
        )
        {
            _rootContainer = rootContainer;
            _levelController = levelController;
        }

        public void StartRound()
        {
            if (_roundKernel != null)
                return;

            DiContainer roundContainer = _rootContainer.CreateSubContainer();
            ILevelModel level = _levelController.Level;
            roundContainer.Bind<ILevelEntityRegistry>()
                .FromInstance((ILevelEntityRegistry)level);
            roundContainer.Bind<ILevelFixedTickSource>()
                .FromInstance((ILevelFixedTickSource)level);
            roundContainer.Bind<ILevelImpactSettingsRegistry>()
                .FromInstance((ILevelImpactSettingsRegistry)level);
            roundContainer.Bind<ILevelCollisionBuffer>()
                .FromInstance((ILevelCollisionBuffer)level);
            roundContainer.Bind<ILevelArenaData>()
                .FromInstance((ILevelArenaData)level);
            SystemsInstaller.Install(roundContainer);
            roundContainer.Bind<Kernel>().AsSingle();
            roundContainer.ResolveRoots();

            Kernel roundKernel = roundContainer.Resolve<Kernel>();

            try
            {
                roundKernel.Initialize();
            }
            catch
            {
                DisposeKernel(roundKernel, roundContainer);
                throw;
            }

            _roundContainer = roundContainer;
            _roundKernel    = roundKernel;
        }

        public void EndRound()
        {
            if (_roundKernel == null)
                return;

            Kernel roundKernel = _roundKernel;
            DiContainer roundContainer = _roundContainer;

            _roundKernel    = null;
            _roundContainer = null;

            DisposeKernel(roundKernel, roundContainer);
        }

        public void Tick()
            => _roundKernel?.Tick();

        public void FixedTick()
            => _roundKernel?.FixedTick();

        public void LateTick()
            => _roundKernel?.LateTick();

        public void Dispose()
            => EndRound();

        private static void DisposeKernel(
            Kernel roundKernel,
            DiContainer roundContainer
        )
        {
            try
            {
                roundKernel.Dispose();
            }
            finally
            {
                try
                {
                    roundKernel.LateDispose();
                }
                finally
                {
                    roundContainer.UnbindAll();
                }
            }
        }
    }
}
