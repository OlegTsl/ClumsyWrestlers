using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Game.Common.AssetsManager;

namespace Game.Common.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private LocalizationData _currentLocalization = null;
        private LocalizationData _skillsLocalization = null;
        private LocalizationData _unitsNamesLocalization = null;
        private LocalizationData _tutorialsLocalization = null;

        private readonly IAssetManager _assetManager;

        private readonly Dictionary<string, string> _commonLocalizations = new Dictionary<string, string>
        {
            { "en", "LocalizationData_en" },
            { "ru", "LocalizationData_ru" }
        };

        private readonly Dictionary<string, string> _skillsLocalizations = new Dictionary<string, string>
        {
            { "en", "SkillsLocalizationData_en" },
            { "ru", "SkillsLocalizationData_ru" }
        };

        private readonly Dictionary<string, string> _unitsNamesLocalizations = new Dictionary<string, string>
        {
            { "en", "UnitNamesLocalizationData_en" },
            { "ru", "UnitNamesLocalizationData_ru" }
        };

        private readonly Dictionary<string, string> _tutorialsLocalizations = new Dictionary<string, string>
        {
            { "en", "TutorialsLocalizationData_en" },
            { "ru", "TutorialsLocalizationData_ru" }
        };

        public LocalizationService(IAssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public async UniTask SetDefaultLanguage()
        {
            var systemLanguage = Application.systemLanguage;

            string commonAddress = systemLanguage switch
            {
                SystemLanguage.Russian => _commonLocalizations["ru"],
                SystemLanguage.English => _commonLocalizations["en"],
                _ => _commonLocalizations["en"]
            };

            string skillsAddress = systemLanguage switch
            {
                SystemLanguage.Russian => _skillsLocalizations["ru"],
                SystemLanguage.English => _skillsLocalizations["en"],
                _ => _skillsLocalizations["ru"]
            };

            string unitsNamesAddress = systemLanguage switch
            {
                SystemLanguage.Russian => _unitsNamesLocalizations["ru"],
                SystemLanguage.English => _unitsNamesLocalizations["en"],
                _ => _unitsNamesLocalizations["en"]
            };

            string tutorialsAddress = systemLanguage switch
            {
                SystemLanguage.Russian => _tutorialsLocalizations["ru"],
                SystemLanguage.English => _tutorialsLocalizations["en"],
                _ => _tutorialsLocalizations["en"]
            };

            await LoadLocalizations(commonAddress, skillsAddress, unitsNamesAddress, tutorialsAddress);
        }

        public async UniTask SetLanguage(string languageCode)
        {
            string commonAddress = _commonLocalizations["en"];
            if (_commonLocalizations.TryGetValue(languageCode, out var common))
                commonAddress = common;

            string skillsAddress = _skillsLocalizations["en"];
            if (_commonLocalizations.TryGetValue(languageCode, out var skills))
                skillsAddress = skills;

            string unitsNamesAddress = _unitsNamesLocalizations["en"];
            if (_commonLocalizations.TryGetValue(languageCode, out var names))
                unitsNamesAddress = names;

            string tutorialsAddress = _tutorialsLocalizations["en"];
            if (_commonLocalizations.TryGetValue(languageCode, out var tutorials))
                tutorialsAddress = tutorials;

            await LoadLocalizations(commonAddress, skillsAddress, unitsNamesAddress, tutorialsAddress);
        }

        private async UniTask LoadLocalizations(string commonAddress, string skillsAddress, string unitsNamesAddress, string tutorialsAddress)
        {
            var commonTask    = _assetManager.LoadAsset<LocalizationData>(commonAddress);
            var skillsTask    = _assetManager.LoadAsset<LocalizationData>(skillsAddress);
            var namesTask     = _assetManager.LoadAsset<LocalizationData>(unitsNamesAddress);
            var tutorialsTask = _assetManager.LoadAsset<LocalizationData>(tutorialsAddress);

            var (common, skills, names, tutorials) = await UniTask.WhenAll(commonTask, skillsTask, namesTask, tutorialsTask);

            _currentLocalization    = common;
            _skillsLocalization     = skills;
            _unitsNamesLocalization = names;
            _tutorialsLocalization  = tutorials;
        }

        public string GetTranslation(string key, params object[] args)
        {
            var value = TryGetFrom(_currentLocalization, key);
            if (value != null) return string.Format(value, args);

            value = TryGetFrom(_skillsLocalization, key);
            if (value != null) return string.Format(value, args);

            value = TryGetFrom(_unitsNamesLocalization, key);
            if (value != null) return string.Format(value, args);

            value = TryGetFrom(_tutorialsLocalization, key);
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