using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAudioController : MonoBehaviour
{
    [Header("Slide sound")]
    public AudioSource slideSource;
    public AudioClip slideClip;
    [Tooltip("Speed above which slide sound starts to fade in.")]
    public float slideMinSpeed = 0.5f;
    public float slideMaxSpeed = 15f;
    public float slideMaxVolume = 0.8f;

    [Header("Heartbeat sound")]
    public AudioSource heartbeatSource;
    public AudioClip heartbeatClip;
    [Tooltip("Speed above which heartbeat starts to increase.")]
    public float heartbeatMinSpeed = 3f;
    public float heartbeatMaxSpeed = 20f;
    public float heartbeatBaseVolume = 0.1f;
    public float heartbeatMaxVolume = 0.9f;
    public float heartbeatBasePitch = 1.0f;
    public float heartbeatMaxPitch = 1.8f;

    private CharacterController controller;
    private Vector3 lastPosition;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;

        // Setup slide source
        if (slideSource != null)
        {
            slideSource.clip = slideClip;
            slideSource.loop = true;
            slideSource.playOnAwake = false;
            slideSource.volume = 0f;
        }

        // Setup heartbeat source
        if (heartbeatSource != null)
        {
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.playOnAwake = false;
            heartbeatSource.volume = 0f;
        }
    }

    private void Start()
    {
        if (slideSource != null && slideClip != null)
            slideSource.Play();

        if (heartbeatSource != null && heartbeatClip != null)
            heartbeatSource.Play();
    }

    private void Update()
    {
        // Estimate horizontal speed based on position delta
        Vector3 currentPos = transform.position;
        Vector3 delta = currentPos - lastPosition;
        delta.y = 0f;
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPosition = currentPos;

        bool grounded = controller != null && controller.isGrounded;

        UpdateSlideAudio(speed, grounded);
        UpdateHeartbeatAudio(speed);
    }

    private void UpdateSlideAudio(float speed, bool grounded)
    {
        if (slideSource == null) return;

        if (!grounded)
        {
            slideSource.volume = 0f;
            return;
        }

        if (speed <= slideMinSpeed)
        {
            slideSource.volume = 0f;
            return;
        }

        float t = Mathf.InverseLerp(slideMinSpeed, slideMaxSpeed, speed);
        float targetVolume = t * slideMaxVolume;
        slideSource.volume = targetVolume;
    }

    private void UpdateHeartbeatAudio(float speed)
    {
        if (heartbeatSource == null) return;

        if (speed <= heartbeatMinSpeed)
        {
            heartbeatSource.volume = 0f;
            heartbeatSource.pitch = heartbeatBasePitch;
            return;
        }

        float t = Mathf.InverseLerp(heartbeatMinSpeed, heartbeatMaxSpeed, speed);

        float volume = Mathf.Lerp(heartbeatBaseVolume, heartbeatMaxVolume, t);
        float pitch = Mathf.Lerp(heartbeatBasePitch, heartbeatMaxPitch, t);

        heartbeatSource.volume = volume;
        heartbeatSource.pitch = pitch;
    }
}
