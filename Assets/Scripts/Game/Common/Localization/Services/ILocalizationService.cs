using Cysharp.Threading.Tasks;

namespace Game.Common.Localization
{
    public interface ILocalizationService
    {
        UniTask SetDefaultLanguage();
        string GetTranslation(string key, params object[] args);
        UniTask SetLanguage(string languageCode);
    }
}
