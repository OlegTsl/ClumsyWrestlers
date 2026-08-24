using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using UnityEngine;

namespace Game.Common.Localization
{
    public sealed class LocalizationService : ILocalizationService
    {
        private readonly IAssetManager _assetManager;
        private readonly Dictionary<string, string> _commonLocales = new()
        {
            { "en", "common_locales_en" },
            { "ru", "common_locales_ru" }
        };

        private IAssetLease<LocalizationData> _commonLocalizationLease;
        private CancellationTokenSource       _loadCancellation;

        public LocalizationService(IAssetManager assetManager)
            => _assetManager = assetManager;

        public UniTask SetDefaultLanguageAsync(CancellationToken cancellationToken)
        {
            string languageCode = Application.systemLanguage == SystemLanguage.Russian ? "ru" : "en";
            return SetLanguageAsync(languageCode, cancellationToken);
        }

        public async UniTask SetLanguageAsync(string languageCode, CancellationToken cancellationToken)
        {
            CancelCurrentLoad();

            _loadCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            CancellationTokenSource currentLoad     = _loadCancellation;
            IAssetLease<LocalizationData> nextLease = null;

            try
            {
                string address = _commonLocales.TryGetValue(languageCode, out string localized)
                    ? localized : _commonLocales["en"];
                
                nextLease = await _assetManager.LoadAssetAsync<LocalizationData>(
                    address, currentLoad.Token);
                
                currentLoad.Token.ThrowIfCancellationRequested();

                IAssetLease<LocalizationData> previousLease = _commonLocalizationLease;
                _commonLocalizationLease = nextLease;
                
                nextLease = null;
                previousLease?.Dispose();
            }
            finally
            {
                nextLease?.Dispose();
                if (ReferenceEquals(_loadCancellation, currentLoad))
                {
                    _loadCancellation = null;
                    currentLoad.Dispose();
                }
            }
        }

        public string GetTranslation(string key, params object[] args)
        {
            string value = TryGetFrom(_commonLocalizationLease?.Asset, key);
            return value == null ? key : string.Format(value, args);
        }

        private static string TryGetFrom(LocalizationData data, string key)
        {
            if (data == null)
                return null;

            string value = data.GetValue(key);
            return string.IsNullOrEmpty(value) || value == key ? null : value;
        }

        private void CancelCurrentLoad()
        {
            CancellationTokenSource cancellation = _loadCancellation;
            _loadCancellation = null;

            cancellation?.Cancel();
            cancellation?.Dispose();
        }

        public void Dispose()
        {
            CancelCurrentLoad();
            _commonLocalizationLease?.Dispose();
            _commonLocalizationLease = null;
        }
    }
}
