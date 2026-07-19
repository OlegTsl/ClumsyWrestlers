using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Game.Common.AssetsManager;

namespace Game.Common.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private LocalizationData _commonLocalization = null;

        private readonly IAssetManager _assetManager;

        private readonly Dictionary<string, string> _commonLocales = new Dictionary<string, string>
        {
            { "en", "common_locales_en" },
            { "ru", "common_locales_ru" }
        };

        public LocalizationService(IAssetManager assetManager)
            => _assetManager = assetManager;

        public async UniTask SetDefaultLanguage()
        {
            var systemLanguage = Application.systemLanguage;

            string commonAddress = systemLanguage switch
            {
                SystemLanguage.Russian => _commonLocales["ru"],
                SystemLanguage.English => _commonLocales["en"],
                _ => _commonLocales["en"]
            };

            await LoadLocalizations(commonAddress);
        }

        public async UniTask SetLanguage(string languageCode)
        {
            string commonAddress = _commonLocales["en"];
            if (_commonLocales.TryGetValue(languageCode, out var common))
                commonAddress = common;

            await LoadLocalizations(commonAddress);
        }

        private async UniTask LoadLocalizations(string commonAddress)
        {
            _commonLocalization = await _assetManager.LoadAsset<LocalizationData>(commonAddress);
        }

        public string GetTranslation(string key, params object[] args)
        {
            var value = TryGetFrom(_commonLocalization, key);
            if (value != null) return string.Format(value, args);
            
            return key;
        }

        private static string TryGetFrom(LocalizationData data, string key)
        {
            if (data == null) return null;

            var v = data.GetValue(key);

            if (string.IsNullOrEmpty(v) || v == key)
                return null;

            return v;
        }
    }
}