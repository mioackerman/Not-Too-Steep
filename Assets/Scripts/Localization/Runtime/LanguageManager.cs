using System;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    public LocalizationDatabase database;
    public int currentIndex = 0;

    public event Action OnLanguageChanged;

    public Language Current =>
        (database && currentIndex >= 0 && currentIndex < database.languages.Count)
        ? database.languages[currentIndex]
        : null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (database)
        {
            currentIndex = Mathf.Clamp(database.defaultLanguageIndex, 0, database.languages.Count - 1);
            Rebuild();
        }
    }

    public void SetLanguageIndex(int idx)
    {
        if (!database) return;
        idx = Mathf.Clamp(idx, 0, database.languages.Count - 1);
        if (idx == currentIndex) return;
        currentIndex = idx;
        Rebuild();
    }

    public string Get(string key, string fallback = "")
    {
        var lang = Current;
        if (lang != null && lang.TryGet(key, out var val)) return val;
        return fallback;
    }

    private void Rebuild()
    {
        if (Current != null) Current.BuildMap();

        OnLanguageChanged?.Invoke();

        foreach (var t in FindObjectsOfType<LocalizedText>(true))
            t.UpdateText();
    }

}
