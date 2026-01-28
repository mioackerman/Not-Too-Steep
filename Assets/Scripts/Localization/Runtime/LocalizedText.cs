using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class LocalizedText : MonoBehaviour
{
    public string key;
    [TextArea] public string editorPreviewFallback = "<missing>";

    private TMP_Text tmpText;
    private Text uiText;

    private void OnEnable()
    {
        CacheComponents();
        Subscribe(true);
        UpdateText();
    }

    private void OnDisable()
    {
        Subscribe(false);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            CacheComponents();
            UpdateText();
        }
    }
#endif

    private void CacheComponents()
    {
        if (!tmpText) tmpText = GetComponent<TMP_Text>();
        if (!uiText) uiText = GetComponent<Text>();
    }

    private void Subscribe(bool add)
    {
        if (LanguageManager.Instance == null) return;

        if (add)
            LanguageManager.Instance.OnLanguageChanged += UpdateText;
        else
            LanguageManager.Instance.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        string value = editorPreviewFallback;

        if (LanguageManager.Instance != null && !string.IsNullOrEmpty(key))
        {
            value = LanguageManager.Instance.Get(key, editorPreviewFallback);
        }

        if (tmpText) tmpText.text = value;
        if (uiText) uiText.text = value;

        if (!tmpText && !uiText)
        {
            Debug.LogWarning($"[LocalizedText] No Text or TMP_Text component found on {name}. " +
                             "Please add Text or TextMeshProUGUI.");
        }

    }
}
