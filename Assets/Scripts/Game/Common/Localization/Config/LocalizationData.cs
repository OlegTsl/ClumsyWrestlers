using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Common.Localization
{
    [CreateAssetMenu(fileName = "locales", menuName = "Game/Locales/Locales Data")]
    public class LocalizationData : ScriptableObject
    {
        [System.Serializable]
        public sealed class LocalizationEntry
        {
            [FormerlySerializedAs("Key")]
            [SerializeField] private string _key;
            [FormerlySerializedAs("Value")]
            [SerializeField] private string _value;

            public string Key => _key;
            public string Value => _value;

            public LocalizationEntry()
            {
            }

            public LocalizationEntry(string key, string value)
            {
                _key = key;
                _value = value;
            }

            public void SetValue(string value)
                => _value = value;
        }

        [FormerlySerializedAs("Entries")]
        [SerializeField] private List<LocalizationEntry> _entries;

        public string GetValue(string key)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                LocalizationEntry entry = _entries[i];
                if (entry.Key == key)
                    return entry.Value;
            }
            return key;
        }

        public void SetOrAdd(string key, string value)
        {
            _entries ??= new List<LocalizationEntry>();
            for (int i = 0; i < _entries.Count; i++)
            {
                LocalizationEntry entry = _entries[i];
                if (entry != null && entry.Key == key)
                {
                    entry.SetValue(value);
                    return;
                }
            }

            _entries.Add(new LocalizationEntry(key, value));
        }
    }
}
