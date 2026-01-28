using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Localization/Database", fileName = "LocalizationDatabase")]
public class LocalizationDatabase : ScriptableObject
{
    public List<Language> languages = new List<Language>();
    public int defaultLanguageIndex = 0;

    public IEnumerable<string> GetAllKeysUnion()
    {
        var set = new HashSet<string>();
        foreach (var lang in languages)
        {
            foreach (var k in lang.Keys())
                if (!string.IsNullOrEmpty(k)) set.Add(k);
        }
        return set;
    }

    public void EnsureKeyConsistency()
    {
        var allKeys = new HashSet<string>(GetAllKeysUnion());
        foreach (var lang in languages)
            foreach (var k in allKeys)
                lang.EnsureKey(k);
    }

    public int IndexOf(Language lang) => languages.IndexOf(lang);
}
