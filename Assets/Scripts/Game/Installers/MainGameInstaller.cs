using Game.Common.Installers;
using Game.Core.Installers;
using Game.Core.Round;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class MainGameInstaller : MonoInstaller
    {
        [SerializeField] private RoundConfiguration _roundConfiguration;

        public override void InstallBindings()
        {
            _roundConfiguration.Validate();
            Container.Bind<IRoundConfiguration>()
                .FromInstance(_roundConfiguration)
                .AsSingle();
            CommonInstaller.Install(Container);
            CoreInstaller.Install(Container);
        }
    }
}
