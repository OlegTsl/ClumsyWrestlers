using System.Collections.Generic;
using UnityEngine;

namespace Game.Common.Localization
{
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/LocalizationData")]
    public class LocalizationData : ScriptableObject
    {
        [System.Serializable]
        public class LocalizationEntry
        {
            public string Key;
            public string Value;
        }

        public List<LocalizationEntry> Entries;

        public string GetValue(string key)
        {
            foreach (var entry in Entries)
            {
                if (entry.Key == key)
                    return entry.Value;
            }
            return key;
        }
    }
}
