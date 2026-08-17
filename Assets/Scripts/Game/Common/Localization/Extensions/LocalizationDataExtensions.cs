namespace Game.Common.Localization
{
    public static class LocalizationDataExtensions
    {
        public static void SetOrAdd(this LocalizationData data, string key, string value)
        {
            data?.SetOrAdd(key, value);
        }
    }
}
