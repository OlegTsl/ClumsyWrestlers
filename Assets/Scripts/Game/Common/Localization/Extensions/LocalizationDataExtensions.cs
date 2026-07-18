using System.Collections.Generic;

namespace Game.Common.Localization
{
    public static class LocalizationDataExtensions
    {
        public static void SetOrAdd(this LocalizationData data, string key, string value)
        {
            if (data == null)
                return;

            if (data.Entries == null)
                data.Entries = new List<LocalizationData.LocalizationEntry>();

            for (int idx = 0; idx < data.Entries.Count; idx++)
            {
                var entry = data.Entries[idx];
                if (entry == null)
                    continue;

                if (entry.Key == key)
                {
                    entry.Value = value;
                    return;
                }
            }

            data.Entries.Add(new LocalizationData.LocalizationEntry
            {
                Key   = key,
                Value = value
            });
        }
    }
}