using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClick;
    public AudioClip explosion;
    public AudioClip spawn;
    public AudioClip complete;

    [Header("Settings State")]
    public bool isMusicOn = true;
    public bool isSFXOn = true;

    private void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load audio preferences (1 = ON, 0 = OFF)
        isMusicOn = PlayerPrefs.GetInt("SETTING_MUSIC", 1) == 1;
        isSFXOn = PlayerPrefs.GetInt("SETTING_SFX", 1) == 1;
    }

    private void Start()
    {
        ApplyAudioSettings();

        if (isMusicOn && backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    #region Music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = isMusicOn ? 1f : 0f;
        musicSource.mute = !isMusicOn;
        if (isMusicOn)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.volume = 0f;
            musicSource.Stop();
        }
    }

    public void ToggleMusic()
    {
        SetMusic(!isMusicOn);
    }

    public void SetMusic(bool enable)
    {
        isMusicOn = enable;
        PlayerPrefs.SetInt("SETTING_MUSIC", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
        ApplyAudioSettings();

        if (isMusicOn)
        {
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.volume = 1f;
                musicSource.mute = false;
                if (!musicSource.isPlaying)
                {
                    PlayMusic(backgroundMusic);
                }
            }
        }
        else
        {
            if (musicSource != null)
            {
                musicSource.volume = 0f;
                musicSource.mute = true;
                musicSource.Pause();
            }
        }
    }
    #endregion

    #region SFX
    public void PlaySFX(AudioClip clip)
    {
        if (!isSFXOn || sfxSource == null || clip == null) return;
        sfxSource.volume = 1f;
        sfxSource.mute = false;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUISound(AudioClip clip)
    {
        if (!isSFXOn || clip == null) return;
        if (uiSource != null)
        {
            uiSource.volume = 1f;
            uiSource.mute = false;
            uiSource.PlayOneShot(clip);
        }
        else if (sfxSource != null)
        {
            sfxSource.volume = 1f;
            sfxSource.mute = false;
            sfxSource.PlayOneShot(clip);
        }
    }

    public void ToggleSFX()
    {
        SetSFX(!isSFXOn);
    }

    public void SetSFX(bool enable)
    {
        isSFXOn = enable;
        PlayerPrefs.SetInt("SETTING_SFX", isSFXOn ? 1 : 0);
        PlayerPrefs.Save();
        ApplyAudioSettings();
    }
    #endregion

    public void ApplyAudioSettings()
    {
        if (musicSource != null)
        {
            musicSource.mute = !isMusicOn;
            musicSource.volume = isMusicOn ? 1f : 0f;
        }

        if (sfxSource != null)
        {
            sfxSource.mute = !isSFXOn;
            sfxSource.volume = isSFXOn ? 1f : 0f;
        }

        if (uiSource != null)
        {
            uiSource.mute = !isSFXOn;
            uiSource.volume = isSFXOn ? 1f : 0f;
        }
    }
}