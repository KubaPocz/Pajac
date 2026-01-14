using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioClip soundEffect;
    public AudioClip musicClip;
    private AudioSource sfxSource;
    private AudioSource musicSource;

    private static AudioManager instance;

    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }

        PlayMusic();
    }

    void Initialize()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        if (soundEffect != null)
        {
            sfxSource.clip = soundEffect;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;

        if (soundEffect != null)
        {
            musicSource.clip = musicClip;
        }

    }

    // Static method to play sound from anywhere
    public static void PlaySoundEffect()
    {
        if (instance != null && instance.sfxSource != null && instance.sfxSource.clip != null)
        {
            instance.sfxSource.Play();
        }
        else
        {
            Debug.LogWarning("PersistentAudioManager not initialized or no sound effect set!");
        }
    }

    public static void PlayMusic()
    {
        if (instance != null && instance.musicSource != null && instance.musicSource.clip != null)
        {
            instance.musicSource.Play();
        }
        else
        {
            Debug.LogWarning("PersistentAudioManager not initialized or no sound effect set!");
        }
    }
}