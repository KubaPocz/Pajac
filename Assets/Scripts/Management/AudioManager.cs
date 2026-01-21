using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Sound Effects - Add as many as you need
    [Header("Sound Effects")]
   
    public AudioClip attackSound;
    public AudioClip uiClickSound;
    public AudioClip curtainOpenSound;
    public AudioClip curtainCloseSound;
    public AudioClip movementSound;
    public AudioClip applauseSound;
    public AudioClip gameoverSound;

    // Music Tracks
    [Header("Music Tracks")]
    public AudioClip mainMenuMusic;
    public AudioClip battleMusic;
    public AudioClip bossMusic;
    public AudioClip upgradeMusic;

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
    }

    void Initialize()
    {
        // Create and configure SFX source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // Create and configure Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true; // Music typically loops
        musicSource.volume = 0.7f; // Slightly lower volume for music
    }

    // === SOUND EFFECT METHODS ===

    // Play a specific sound effect by passing the clip
    public static void PlaySoundEffect(AudioClip clip, float volume = 1.0f)
    {
        if (instance != null && clip != null)
        {
            instance.sfxSource.PlayOneShot(clip, volume);
        }
    }

    // Convenience methods for specific sounds
    public static void PlayAttackSound() => PlaySoundEffect(instance?.attackSound);
    public static void PlayUiClickSound() => PlaySoundEffect(instance?.uiClickSound);
    public static void PlayCurtainOpenSound() => PlaySoundEffect(instance?.curtainOpenSound);
    public static void PlayCurtainCloseSound() => PlaySoundEffect(instance?.curtainCloseSound);
    public static void PlayMovementSound() => PlaySoundEffect(instance?.movementSound);
    public static void PlayApplauseSound() => PlaySoundEffect(instance?.applauseSound);
    public static void PlayGameOverSound() => PlaySoundEffect(instance?.gameoverSound);

    // === MUSIC METHODS ===

    // Play a specific music track
    public static void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (instance != null && musicClip != null)
        {
            instance.musicSource.clip = musicClip;
            instance.musicSource.loop = loop;
            instance.musicSource.Play();
        }
    }

    // Stop current music
    public static void StopMusic()
    {
        if (instance != null)
        {
            instance.musicSource.Stop();
        }
    }

    // Pause current music
    public static void PauseMusic()
    {
        if (instance != null)
        {
            instance.musicSource.Pause();
        }
    }

    // Resume paused music
    public static void ResumeMusic()
    {
        if (instance != null)
        {
            instance.musicSource.UnPause();
        }
    }

    // Convenience methods for specific music tracks
    public static void PlayMainMenuMusic() => PlayMusic(instance?.mainMenuMusic);
    public static void PlayLevelMusic() => PlayMusic(instance?.battleMusic);
    public static void PlayBossMusic() => PlayMusic(instance?.bossMusic);
    public static void PlayVictoryMusic() => PlayMusic(instance?.upgradeMusic); 

}