using System;
using Cysharp.Threading.Tasks;
using Game.Common.Localization;
using Game.Core.Extensions;
using Game.Core.Round;
using UnityEngine;

namespace Game.Core.Controllers
{
    public class MainGameController : IMainGameController, IDisposable
    {
        private readonly ILocalizationService _localizationService;
        private readonly IRoundController     _roundController;

        public MainGameController(
            ILocalizationService localizationService,
            IRoundController     roundController
        )
        {
            _localizationService = localizationService;
            _roundController     = roundController;
        }

        public void RunGame()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            RunGameAsync().Forget();
        }

        private async UniTask RunGameAsync()
        {
            LocalizationExtension.InitializeService(_localizationService);
            await _localizationService.SetDefaultLanguage();

            await _roundController.StartRound("BrawlArena");
        }

        public void Dispose()
        {
            
        }

        /*private readonly ILocalizationService _localizationService;
        private readonly ISplashLoaderViewController _loaderController;
        private readonly IStateMachine _lobbyStateMachine;
        private readonly IStateMachine _mapStateMachine;
        private readonly IStateMachine _levelStateMachine;
        private readonly ILoadingOrchestrator _loadingOrchestrator;

        private CancellationTokenSource _lobbyRootCts;
        private CancellationTokenSource _mapRootCts;
        private CancellationTokenSource _levelRootCts;

        public MainGameController(
            ILocalizationService localizationService,
            ISplashLoaderViewController loaderController,
            IStateMachine lobbyStateMachine,
            IStateMachine mapStateMachine,
            IStateMachine levelStateMachine,
            ILoadingOrchestrator loadingOrchestrator)
        {
            _localizationService = localizationService;
            _loaderController = loaderController;
            _lobbyStateMachine = lobbyStateMachine;
            _mapStateMachine = mapStateMachine;
            _levelStateMachine = levelStateMachine;
            _loadingOrchestrator = loadingOrchestrator;
        }

        public void RunGame()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            RunGameAsync().Forget();
        }

        private async UniTask RunGameAsync()
        {
            LocalizationExtension.InitializeService(_localizationService);

            await _localizationService.SetDefaultLanguage();
            await _loaderController.InitializeAsync();

            RunLobby(new LobbyViewPayload
            {
                Task = new StateTask(),
                TransitionType = LevelTransitionType.None
            });
        }

        public void RunLobby(LobbyViewPayload payload)
        {
            StopLobby();

            _lobbyRootCts = new CancellationTokenSource();

            _lobbyStateMachine
                .Execute<LobbyViewController, LobbyViewPayload>(payload, _lobbyRootCts.Token)
                .Forget();

            if (payload.Task != null)
                _loadingOrchestrator.Enqueue(payload.Task, OperationType.Load);
        }

        public void RunMap(MapViewPayload payload)
        {
            StopMap();

            _mapRootCts = new CancellationTokenSource();

            _mapStateMachine
                .Execute<MapViewController, MapViewPayload>(payload, _mapRootCts.Token)
                .Forget();

            if (payload.Task != null)
                _loadingOrchestrator.Enqueue(payload.Task, OperationType.Load);
        }

        public void RunLevel(LevelPayload payload)
        {
            StopLevel();

            _levelRootCts = new CancellationTokenSource();

            _levelStateMachine
                .Execute<LevelController, LevelPayload>(payload, _levelRootCts.Token)
                .Forget();
        }

        public void StopLobby()
        {
            _lobbyRootCts?.Cancel();
            _lobbyRootCts?.Dispose();
            _lobbyRootCts = null;
        }

        public void StopMap()
        {
            _mapRootCts?.Cancel();
            _mapRootCts?.Dispose();
            _mapRootCts = null;
        }

        public void StopLevel()
        {
            _levelRootCts?.Cancel();
            _levelRootCts?.Dispose();
            _levelRootCts = null;
        }

        public void StopAll()
        {
            StopLevel();
            StopMap();
            StopLobby();
        }

        public void Dispose()
        {
            StopAll();
        }*/
    }
}