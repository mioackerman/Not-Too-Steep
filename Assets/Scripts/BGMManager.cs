using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    public AudioSource bgmSource;
    public AudioClip[] playlist;

    public bool playInOrder = true;
    public bool loopPlaylist = true;

    private int currentIndex = 0;
    private bool muted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (playlist.Length > 0 && bgmSource != null)
        {
            currentIndex = 0;
            PlayCurrentSong();
        }
    }

    private void Update()
    {
        if (bgmSource != null &&
            !bgmSource.isPlaying &&
            bgmSource.clip != null &&
            !muted)
        {
            PlayNext();
        }
    }

    public void PlayCurrentSong()
    {
        if (playlist.Length == 0) return;

        bgmSource.clip = playlist[currentIndex];
        bgmSource.Play();
    }

    public void PlayNext()
    {
        if (playlist.Length == 0) return;

        if (playInOrder)
        {
            currentIndex++;

            if (currentIndex >= playlist.Length)
            {
                if (loopPlaylist)
                    currentIndex = 0;
                else
                    return;
            }
        }
        else
        {
            currentIndex = Random.Range(0, playlist.Length);
        }

        PlayCurrentSong();
    }

    public void PlayPrevious()
    {
        if (playlist.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = playlist.Length - 1;

        PlayCurrentSong();
    }

    public void ToggleMute()
    {
        muted = !muted;

        if (muted)
            bgmSource.Pause();
        else
            bgmSource.Play();
    }

    public void SetVolume(float value)
    {
        if (bgmSource != null)
            bgmSource.volume = value;
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
}
