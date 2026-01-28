using UnityEngine;
using TMPro;

public class LanguageDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        var mgr = LanguageManager.Instance;
        if (!mgr || !mgr.database || dropdown == null) return;

        dropdown.ClearOptions();
        var opts = new System.Collections.Generic.List<string>();
        foreach (var lang in mgr.database.languages)
            opts.Add(lang.languageName);
        dropdown.AddOptions(opts);

        dropdown.value = Mathf.Clamp(mgr.currentIndex, 0, Mathf.Max(0, opts.Count - 1));
        dropdown.onValueChanged.AddListener(mgr.SetLanguageIndex);
    }
}
