using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Volume Settings
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1.0f;
    [Range(0f, 1f)] public float soundEffectsVolume = 1.0f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float loopingSoundsVolume = 1.0f;

    // Individual Music Track Volumes
    [Header("Music Track Volumes")]
    [Range(0f, 1f)] public float mainMenuMusicVolume = 0.6f;
    [Range(0f, 1f)] public float battleMusicVolume = 0.7f;
    [Range(0f, 1f)] public float bossMusicVolume = 0.8f;
    [Range(0f, 1f)] public float upgradeMusicVolume = 0.5f;

    // Individual Looping Sound Volumes
    [Header("Looping Sound Volumes")]
    [Range(0f, 1f)] public float windUpLoopVolume = 0.5f;

    // Individual Sound Effect Volumes
    [Header("Sound Effect Volumes")]
    [Range(0f, 1f)] public float attackSoundVolume = 1.0f;
    [Range(0f, 1f)] public float uiClickSoundVolume = 0.8f;
    [Range(0f, 1f)] public float curtainOpenSoundVolume = 0.9f;
    [Range(0f, 1f)] public float curtainCloseSoundVolume = 0.9f;
    [Range(0f, 1f)] public float movementSoundVolume = 0.7f;
    [Range(0f, 1f)] public float applauseSoundVolume = 1.0f;
    [Range(0f, 1f)] public float gameoverSoundVolume = 0.8f;
    [Range(0f, 2f)] public float blockSoundVolume = 0.9f;

    // Sound Effects - Add as many as you need
    [Header("Sound Effects")]
    public AudioClip attackSound;
    public AudioClip uiClickSound;
    public AudioClip curtainOpenSound;
    public AudioClip curtainCloseSound;
    public AudioClip movementSound;
    public AudioClip applauseSound;
    public AudioClip gameoverSound;
    public AudioClip blockSound;

    // Add WindUpLoop sound
    public AudioClip windUpLoopSound;

    // Music Tracks
    [Header("Music Tracks")]
    public AudioClip mainMenuMusic;
    public AudioClip battleMusic;
    public AudioClip bossMusic;
    public AudioClip upgradeMusic;

    // Looping Sound Management
    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource loopingSoundSource;
    private static AudioManager instance;

    // Current music track volume
    private float currentMusicTrackVolume = 0.7f;

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

    private void Start()
    {
        PlayMainMenuMusic();
    }

    void Initialize()
    {
        // Create and configure SFX source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // Create and configure Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        // Create and configure Looping Sound source
        loopingSoundSource = gameObject.AddComponent<AudioSource>();
        loopingSoundSource.playOnAwake = false;
        loopingSoundSource.loop = true;

        // Apply initial volume settings
        UpdateAllVolumes();
    }

    void Update()
    {
        // Update volumes in real-time if they change in the inspector
        UpdateAllVolumes();
    }

    // Update all volume levels based on current settings
    public void UpdateAllVolumes()
    {
        // Update music source volume (with track-specific volume)
        musicSource.volume = currentMusicTrackVolume * musicVolume * masterVolume;

        // Update looping sound source volume
        loopingSoundSource.volume = loopingSoundsVolume * masterVolume;

        // Note: SFX volume is applied when playing each sound
    }

    // === VOLUME CONTROL METHODS ===

    // Set master volume
    public static void SetMasterVolume(float volume)
    {
        if (instance != null)
        {
            instance.masterVolume = Mathf.Clamp01(volume);
            instance.UpdateAllVolumes();
        }
    }

    // Set sound effects volume
    public static void SetSoundEffectsVolume(float volume)
    {
        if (instance != null)
        {
            instance.soundEffectsVolume = Mathf.Clamp01(volume);
        }
    }

    // Set music volume
    public static void SetMusicVolume(float volume)
    {
        if (instance != null)
        {
            instance.musicVolume = Mathf.Clamp01(volume);
            instance.UpdateAllVolumes();
        }
    }

    // Set looping sounds volume
    public static void SetLoopingSoundsVolume(float volume)
    {
        if (instance != null)
        {
            instance.loopingSoundsVolume = Mathf.Clamp01(volume);
            instance.UpdateAllVolumes();
        }
    }

    // Set specific music track volumes
    public static void SetMainMenuMusicVolume(float volume)
    {
        if (instance != null)
        {
            instance.mainMenuMusicVolume = Mathf.Clamp01(volume);
            // Update if this track is currently playing
            if (instance.musicSource.clip == instance.mainMenuMusic)
            {
                instance.currentMusicTrackVolume = volume;
                instance.UpdateAllVolumes();
            }
        }
    }

    public static void SetBattleMusicVolume(float volume)
    {
        if (instance != null)
        {
            instance.battleMusicVolume = Mathf.Clamp01(volume);
            if (instance.musicSource.clip == instance.battleMusic)
            {
                instance.currentMusicTrackVolume = volume;
                instance.UpdateAllVolumes();
            }
        }
    }

    public static void SetBossMusicVolume(float volume)
    {
        if (instance != null)
        {
            instance.bossMusicVolume = Mathf.Clamp01(volume);
            if (instance.musicSource.clip == instance.bossMusic)
            {
                instance.currentMusicTrackVolume = volume;
                instance.UpdateAllVolumes();
            }
        }
    }

    public static void SetUpgradeMusicVolume(float volume)
    {
        if (instance != null)
        {
            instance.upgradeMusicVolume = Mathf.Clamp01(volume);
            if (instance.musicSource.clip == instance.upgradeMusic)
            {
                instance.currentMusicTrackVolume = volume;
                instance.UpdateAllVolumes();
            }
        }
    }

    // Set specific sound effect volumes
    public static void SetAttackSoundVolume(float volume) => instance.attackSoundVolume = Mathf.Clamp01(volume);
    public static void SetUiClickSoundVolume(float volume) => instance.uiClickSoundVolume = Mathf.Clamp01(volume);
    public static void SetCurtainOpenSoundVolume(float volume) => instance.curtainOpenSoundVolume = Mathf.Clamp01(volume);
    public static void SetCurtainCloseSoundVolume(float volume) => instance.curtainCloseSoundVolume = Mathf.Clamp01(volume);
    public static void SetMovementSoundVolume(float volume) => instance.movementSoundVolume = Mathf.Clamp01(volume);
    public static void SetApplauseSoundVolume(float volume) => instance.applauseSoundVolume = Mathf.Clamp01(volume);
    public static void SetGameoverSoundVolume(float volume) => instance.gameoverSoundVolume = Mathf.Clamp01(volume);
    public static void SetBlockSoundVolume(float volume) => instance.blockSoundVolume = Mathf.Clamp01(volume);
    public static void SetWindUpLoopVolume(float volume) => instance.windUpLoopVolume = Mathf.Clamp01(volume);

    // === SOUND EFFECT METHODS ===

    // Play a specific sound effect by passing the clip
    public static void PlaySoundEffect(AudioClip clip, float volume = 1.0f)
    {
        if (instance != null && clip != null)
        {
            float finalVolume = volume * instance.soundEffectsVolume * instance.masterVolume;
            instance.sfxSource.PlayOneShot(clip, finalVolume);
        }
    }

    // Convenience methods for specific sounds with individual volume settings
    public static void PlayAttackSound() => PlaySoundEffect(instance?.attackSound, instance.attackSoundVolume);
    public static void PlayUiClickSound() => PlaySoundEffect(instance?.uiClickSound, instance.uiClickSoundVolume);
    public static void PlayCurtainOpenSound() => PlaySoundEffect(instance?.curtainOpenSound, instance.curtainOpenSoundVolume);
    public static void PlayCurtainCloseSound() => PlaySoundEffect(instance?.curtainCloseSound, instance.curtainCloseSoundVolume);
    public static void PlayMovementSound() => PlaySoundEffect(instance?.movementSound, instance.movementSoundVolume);
    public static void PlayApplauseSound() => PlaySoundEffect(instance?.applauseSound, instance.applauseSoundVolume);
    public static void PlayGameOverSound() => PlaySoundEffect(instance?.gameoverSound, instance.gameoverSoundVolume);
    public static void PlayBlockSound() => PlaySoundEffect(instance?.blockSound, instance.blockSoundVolume);

    // New method for WindUpLoop sound effect
    public static void PlayWindUpLoopSound()
    {
        if (instance != null && instance.windUpLoopSound != null)
        {
            float finalVolume = instance.windUpLoopVolume * instance.soundEffectsVolume * instance.masterVolume;
            instance.sfxSource.PlayOneShot(instance.windUpLoopSound, finalVolume);
        }
    }

    // === LOOPING SOUND METHODS ===

    // Start playing a looping sound
    public static void PlayLoopingSound(AudioClip loopClip, float volume = 1.0f)
    {
        if (instance != null && loopClip != null)
        {
            // If a different loop is playing, stop it first
            if (instance.loopingSoundSource.isPlaying && instance.loopingSoundSource.clip != loopClip)
            {
                instance.loopingSoundSource.Stop();
            }

            // Set up and play the new loop
            instance.loopingSoundSource.clip = loopClip;
            instance.loopingSoundSource.volume = volume * instance.loopingSoundsVolume * instance.masterVolume;
            instance.loopingSoundSource.Play();
        }
    }

    // Stop the current looping sound
    public static void StopLoopingSound()
    {
        if (instance != null && instance.loopingSoundSource.isPlaying)
        {
            instance.loopingSoundSource.Stop();
        }
    }

    // === WIND UP LOOP SPECIFIC METHODS ===

    // Start playing the wind-up loop as a persistent looping sound
    public static void StartWindUpLoop()
    {
        if (instance != null && instance.windUpLoopSound != null)
        {
            float finalVolume = instance.windUpLoopVolume * instance.loopingSoundsVolume * instance.masterVolume;
            PlayLoopingSound(instance.windUpLoopSound, instance.windUpLoopVolume);
        }
    }

    // Stop the wind-up loop if it's currently playing
    public static void StopWindUpLoop()
    {
        if (instance != null && instance.loopingSoundSource.isPlaying &&
            instance.loopingSoundSource.clip == instance.windUpLoopSound)
        {
            instance.loopingSoundSource.Stop();
        }
    }

    // === MUSIC METHODS ===

    // Play a specific music track with its individual volume
    public static void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (instance != null && musicClip != null)
        {
            instance.musicSource.clip = musicClip;
            instance.musicSource.loop = loop;

            // Set the appropriate track-specific volume
            if (musicClip == instance.mainMenuMusic)
                instance.currentMusicTrackVolume = instance.mainMenuMusicVolume;
            else if (musicClip == instance.battleMusic)
                instance.currentMusicTrackVolume = instance.battleMusicVolume;
            else if (musicClip == instance.bossMusic)
                instance.currentMusicTrackVolume = instance.bossMusicVolume;
            else if (musicClip == instance.upgradeMusic)
                instance.currentMusicTrackVolume = instance.upgradeMusicVolume;
            else
                instance.currentMusicTrackVolume = 0.7f; // Default volume for other tracks

            instance.UpdateAllVolumes();
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
    public static void PlayBattleMusic() => PlayMusic(instance?.battleMusic);
    public static void PlayBossMusic() => PlayMusic(instance?.bossMusic);
    public static void PlayUpgradeMusic() => PlayMusic(instance?.upgradeMusic);

    void OnDestroy()
    {
        // Clean up - stop all sounds when AudioManager is destroyed
        if (instance == this)
        {
            StopLoopingSound();
            StopMusic();
        }
    }
}