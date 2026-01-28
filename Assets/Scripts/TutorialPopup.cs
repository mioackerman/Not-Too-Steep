using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    public GameObject panel;
    public KeyCode closeKey = KeyCode.F;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    private void Update()
    {
        if (panel == null) return;

        if (panel.activeSelf && Input.GetKeyDown(closeKey))
        {
            panel.SetActive(false);
        }
    }
}
