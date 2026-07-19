using Game.Common.Localization;
using UnityEngine;

namespace Game.Core.Extensions
{
    public static class LocalizationExtension
    {
        private static ILocalizationService _localizationService;

        public static void InitializeService(ILocalizationService localizationService)
            => _localizationService = localizationService;

        public static string GetTranslation(this string key, params object[] args)
        {
            if (_localizationService == null)
            {
                Debug.LogError("LocalizationService is not initialized. Call InitializeService first.");
                return $"[UNINITIALIZED: {key}]";
            }

            return _localizationService.GetTranslation(key, args);
        }
    }
}
