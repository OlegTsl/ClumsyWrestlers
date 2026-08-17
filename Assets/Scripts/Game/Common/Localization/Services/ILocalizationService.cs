using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Common.Localization
{
    public interface ILocalizationService : IDisposable
    {
        UniTask SetDefaultLanguageAsync(CancellationToken cancellationToken);
        UniTask SetLanguageAsync(string languageCode, CancellationToken cancellationToken);
        string GetTranslation(string key, params object[] args);
    }
}
