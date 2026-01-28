using UnityEngine;

public class BGMSliderProxy : MonoBehaviour
{
    public void OnValueChanged(float value)
    {
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.SetVolume(value);
        }
    }
}
