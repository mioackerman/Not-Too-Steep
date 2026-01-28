using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Localization/Language", fileName = "Language_")]
public class Language : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string key;
        [TextArea] public string value;
    }

    public string languageName = "English";
    public List<Entry> entries = new List<Entry>();

    private Dictionary<string, string> _map;

    public void BuildMap()
    {
        _map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var e in entries)
        {
            if (!string.IsNullOrEmpty(e.key))
                _map[e.key] = e.value ?? "";
        }
    }

    public bool TryGet(string key, out string val)
    {
        if (_map == null) BuildMap();
        return _map.TryGetValue(key, out val);
    }

    public IEnumerable<string> Keys()
    {
        foreach (var e in entries) yield return e.key;
    }

    public bool HasKey(string key)
    {
        return entries.Exists(e => e.key == key);
    }

    public void EnsureKey(string key)
    {
        if (!HasKey(key)) entries.Add(new Entry { key = key, value = "" });
    }

    public void RemoveKey(string key)
    {
        entries.RemoveAll(e => e.key == key);
    }
}
