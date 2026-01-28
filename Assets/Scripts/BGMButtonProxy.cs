using UnityEngine;

public class BGMButtonProxy : MonoBehaviour
{
    public void OnClickToggle()
    {
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.ToggleMute();
        }
    }
}
