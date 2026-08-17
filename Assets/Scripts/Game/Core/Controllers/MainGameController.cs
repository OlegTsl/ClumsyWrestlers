using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.Localization;
using Game.Core.Extensions;
using Game.Core.Round;
using UnityEngine;

namespace Game.Core.Controllers
{
    public sealed class MainGameController : IMainGameController, IDisposable
    {
        private readonly ILocalizationService _localizationService;
        private readonly IRoundController _roundController;

        private CancellationTokenSource _lifetimeCancellation;

        public MainGameController(
            ILocalizationService localizationService,
            IRoundController roundController
        )
        {
            _localizationService = localizationService;
            _roundController = roundController;
        }

        public void RunGame()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            _lifetimeCancellation?.Cancel();
            _lifetimeCancellation?.Dispose();
            _lifetimeCancellation = new CancellationTokenSource();
            RunGameAsync(_lifetimeCancellation.Token).Forget();
        }

        private async UniTask RunGameAsync(CancellationToken cancellationToken)
        {
            try
            {
                LocalizationExtension.InitializeService(_localizationService);
                await _localizationService.SetDefaultLanguageAsync(cancellationToken);
                await _roundController.StartRoundAsync("BrawlArena", cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        public void Dispose()
        {
            _lifetimeCancellation?.Cancel();
            _lifetimeCancellation?.Dispose();
            _lifetimeCancellation = null;
            _roundController.EndRound();
        }
    }
}
