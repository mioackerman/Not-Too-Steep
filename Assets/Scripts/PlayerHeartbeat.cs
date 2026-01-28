using UnityEngine;

public class PlayerHeartbeat : MonoBehaviour
{
    public AudioSource heartbeatSource;

    // Normal heartbeat pitch
    public float normalPitch = 1.0f;

    // Maximum heartbeat speed (5x)
    public float maxPitch = 2.0f;

    // How fast heartbeat ramps up/down
    public float pitchLerpSpeed = 5.0f;

    // Sensitivity: smaller = more sensitive
    // When speed reaches this value, pitch -> maxPitch
    public float tenseSpeed = 8f;

    public float normalVolume = 0.4f;
    public float maxVolume = 1.0f;

    private Vector3 lastPosition;

    private void Awake()
    {
        lastPosition = transform.position;

        if (heartbeatSource != null)
        {
            heartbeatSource.loop = true;
            heartbeatSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        if (heartbeatSource != null && heartbeatSource.clip != null)
        {
            heartbeatSource.pitch = normalPitch;
            heartbeatSource.volume = normalVolume;
            heartbeatSource.Play();
        }
    }

    private void Update()
    {
        Vector3 current = transform.position;
        Vector3 delta = current - lastPosition;
        delta.y = 0f;

        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPosition = current;

        UpdateHeartbeat(speed);
    }

    private void UpdateHeartbeat(float speed)
    {
        if (heartbeatSource == null)
            return;

        // t = 0 when speed = 0
        // t = 1 when speed >= tenseSpeed
        float t = Mathf.InverseLerp(0f, tenseSpeed, speed);

        // Heartbeat pitch goes from 1x to 5x
        float targetPitch = Mathf.Lerp(normalPitch, maxPitch, t);

        // Volume also increases
        float targetVolume = Mathf.Lerp(normalVolume, maxVolume, t);

        // Smooth change
        heartbeatSource.pitch = Mathf.Lerp(
            heartbeatSource.pitch,
            targetPitch,
            Time.deltaTime * pitchLerpSpeed
        );

        heartbeatSource.volume = Mathf.Lerp(
            heartbeatSource.volume,
            targetVolume,
            Time.deltaTime * pitchLerpSpeed
        );
    }
}
