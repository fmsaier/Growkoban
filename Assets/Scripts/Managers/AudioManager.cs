using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

public enum MixerType
{
    MasterAudio, // Handles all volume
    MusicAudio,  // Handles music volume
    SFXAudio,    // Handles sound effects volume
}

/// <summary>
/// Handles the play back of all types of audio (music/sfx)
/// We use an AudioMixer to control the audio output based on
/// current audio settings. 
/// 
/// Sound effects are handled via a pool of audio sources to allocate 
/// resources to play sounds when needed while also limiting how many
/// effects can be played simultaniously.
/// 
/// Currently only a single music clip can be played at once.
/// Meaning, you can only change the track.
/// To do multiple tracks would require multiple mixers which are you are free to do
/// but it is beyond the scope of this package (at least currently)
/// 
/// For the code that handles audio settings changes <see cref="AudioSettings"/>
/// 
/// TODO: Since we are using SoundEffects scriptable objects and ObjectPool now we should clean this class up from the old way of doing things
/// </summary>
/// 
[RequireComponent(typeof(SFXAudioSourcePool))]
public class AudioManager : Singleton<AudioManager>
{
    #region Constants
    /// <summary>
    /// When converting the slider value into a log value we need to multiply it 
    /// to get a volume between -80 and 0. Assuming the min slider value is set to 
    /// 0.0001 and the max value is set to 1, multiplying by 20f seems to do the trick
    /// </summary>
    public const float DEFAULT_MULTIPLIER = 20f;

    /// <summary>
    /// The name audio mixer group's parent name"
    /// </summary>
    public const string AUDIO_MIXER_MASTER_NAME = "Master";

    /// <summary>
    /// The name audio mixer that controls the music
    /// </summary>
    public const string AUDIO_MIXER_MUSIC_NAME = "Music";

    /// <summary>
    /// The name audio mixer that controls the sound effects
    /// </summary>
    public const string AUDIO_MIXER_SFX_NAME = "SFX";
    #endregion

    [Header("Configurations")]
    [SerializeField, Tooltip("SoundEffect to preview volume change for SFXs")]
    SoundEffect sfxPreview;
    [SerializeField, Tooltip("The song to start playing as soon as this object is loaded")]
    MusicClip musicClip;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)]
    float defaultMasterVolume = 1f;

    [SerializeField, Range(0f, 1f)]
    float defaultMusicVolume = .25f;

    [SerializeField, Range(0f, 1f)]
    float defaultSFXVolume = .5f;

    [Header("Mixer Info")]
    [SerializeField, Tooltip("Main audio mixer that handles all audio output")]
    AudioMixer mixer;
    public AudioMixer Mixer { get { return mixer; } }
    [SerializeField, Tooltip("AudioMixer parameter name that handles the master volume")]
    string masterVolumeParameterName = "MasterVolume";

    [SerializeField, Tooltip("AudioMixer parameter name that handles the music volume")]
    string musicVolumeParameterName = "MusicVolume";

    [SerializeField, Tooltip("AudioMixer parameter name that handles the sfx volume")]
    string sfxVolumeParameterName = "SFXVolume";

    /// <summary>
    /// Since we typically only have a single source for music
    /// We will set it here and use it as needed
    /// </summary>
    SFXAudioSource musicSource;
    SFXAudioSource MusicSource
    {
        get
        {
            if (musicSource == null)
            {
                musicSource = GetSource(AUDIO_MIXER_MUSIC_NAME);

                // Since the pool subscribes to these, we want to unsubscribe
                // as this is Music and should not be affected by "SFX" changes
                TogglePlaybackDelegates -= musicSource.OnSoundToggled;
                StopSoundDelegates -= musicSource.Stop;
            }

            return musicSource;
        }
    }

    SFXAudioSourcePool srcPool;
    SFXAudioSourcePool SrcPool
    {
        get
        {
            if (srcPool == null)
                srcPool = GetComponent<SFXAudioSourcePool>();
            return srcPool;
        }
    }

    List<SFXAudioSource> SFXSources
    {
        get
        {
            return SrcPool.Sources;
        }
    }

    public delegate void TogglePlayback(bool enabled);
    public TogglePlayback TogglePlaybackDelegates;

    public delegate void StopSound();
    public StopSound StopSoundDelegates;

    public bool AudioPaused { get; protected set; }

    void Start()
    {
        if (gameObject != null)
        {
            // Set default volumes
            SetMasterVolume(defaultMasterVolume);
            SetMusicVolume(defaultMusicVolume);
            SetSFXVolume(defaultSFXVolume);
        }
    }

    public void StartMusic() => MusicSource.Play(musicClip);
    public void SetMasterVolume(float volume) => SetVolume(masterVolumeParameterName, volume);
    public void SetMusicVolume(float volume) => SetVolume(musicVolumeParameterName, volume);
    public void SetSFXVolume(float volume) => SetVolume(sfxVolumeParameterName, volume);
    void SetVolume(string name, float volume, float multiplier = DEFAULT_MULTIPLIER)
    {
        // Anything above "0" increased the sound where it starts to clip
        // While -80f is the lowest value we can set something to
        var value = Mathf.Clamp(Mathf.Log10(volume) * multiplier, -80f, 0f);
        if (mixer != null)
            mixer.SetFloat(name, value);
    }

    /// <summary>
    /// Updates the mixers volume for the given type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="volume"></param>
    public void SetVolume(MixerType type, float volume)
    {
        switch (type)
        {
            case MixerType.MasterAudio:
                defaultMasterVolume = volume;
                SetMasterVolume(volume);
                break;
            case MixerType.MusicAudio:
                defaultMusicVolume = volume;
                SetMusicVolume(volume);
                break;
            case MixerType.SFXAudio:
                defaultSFXVolume = volume;
                SetSFXVolume(volume);
                break;
        }
    }

    /// <summary>
    /// Plays a clip for the given type, if one exists, to preview what the volume
    /// for that specific type of audio will sound like with given change
    /// </summary>
    /// <param name="type"></param>
    /// <param name="volume"></param>
    public void SetAndPreviewVolumeChange(MixerType type, float volume)
    {
        SetVolume(type, volume);

        switch (type)
        {
            case MixerType.SFXAudio:
                // Preview the SFX volume change
                Play(sfxPreview);
                break;
        }
    }

    /// <summary>
    /// Returns the volume default for the given audio mixer.
    /// Defaults to 0f.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetDefaultVolume(MixerType type)
    {
        float volume = 0f;
        switch (type)
        {
            case MixerType.MasterAudio:
                volume = defaultMasterVolume;
                break;
            case MixerType.MusicAudio:
                volume = defaultMusicVolume;
                break;
            case MixerType.SFXAudio:
                volume = defaultSFXVolume;
                break;
        }

        return volume;
    }

    /// <summary>
    /// Playse the given effect as a Music which means it loops
    /// </summary>
    /// <param name="music"></param>
    public void PlayMusic(SoundEffect music) => MusicSource.Play(music);

    /// <summary>
    /// Returns a random clip from the collection given
    /// </summary>
    /// <param name="clips"></param>
    /// <returns></returns>
    public static AudioClip GetRandomClip(AudioClip[] clips) => clips[Random.Range(0, clips.Length)];

    /// <summary>
    /// Attempts to play the speficic clip index from the clip collection
    /// as defined in the <see cref="SoundEffect"/> scriptable object for the given sfx
    /// </summary>
    /// <param name="sfx"></param>
    /// <param name="clipIndex"></param>
    /// <returns></returns>
    public SFXAudioSource Play(SoundEffect sfx, int clipIndex = -1, string mixerGroupname = AUDIO_MIXER_SFX_NAME)
    {
        // Nothing to play
        if (sfx == null)
            return null;

        // We only want one of these playing at a time
        if (sfx.preventMultiple)
        {
            var existingSrc = SrcPool.GetSourceAssignedToSFX(sfx);
            if (existingSrc != null)
            {
                // Before returning it make sure it is in deed playing
                existingSrc.Play(sfx, clipIndex);
                return existingSrc;
            }
        }

        var src = GetSource(mixerGroupname);
        src.Play(sfx, clipIndex);
        return src;
    }

    /// <summary>
    /// Creates and returns an AudioSource group assigned to the given mixer group
    /// Defaults to SFX Mixer group
    /// </summary>
    /// <param name="mixerGroupname"></param>
    /// <returns></returns>
    SFXAudioSource GetSource(string mixerGroupname)
    {
        var mixerGroup = mixer.FindMatchingGroups(AUDIO_MIXER_MASTER_NAME)
                                      .Where(g => g.name == mixerGroupname).First();

        var src = SrcPool.GetSource();
        src.SetOutputMixer(mixerGroup);
        return src;
    }

    /// <summary>
    /// Force stops all sounds and makes looping sounds no longer loop
    /// </summary>
    public void StopSFXs() => StopSoundDelegates?.Invoke();

    /// <summary>
    /// Pauses playback of both SFXs and Music
    /// </summary>
    public void PauseAudio()
    {
        PauseSFXs();
        PauseMusic();
    }

    /// <summary>
    /// Resumes playback of both SFXs and Music
    /// </summary>
    public void ResumeAudio()
    {
        ResumeSFXs();
        ResumeMusic();
    }

    /// <summary>
    /// Triggers all active sfxs to pause playback
    /// </summary>
    public void PauseSFXs()
    {
        AudioPaused = true;
        TogglePlaybackDelegates?.Invoke(false);
    }

    /// <summary>
    /// Triggers all paused sfxs to playback
    /// </summary>
    public void ResumeSFXs()
    {
        AudioPaused = false;
        TogglePlaybackDelegates?.Invoke(true);
    }

    public void PauseMusic() => MusicSource.Pause();
    public void ResumeMusic() => MusicSource.Resume();

    /// <summary>
    /// Releases the given src back into the pool
    /// </summary>
    /// <param name="src"></param>
    public void OnSourceReleased(SFXAudioSource src) => SrcPool.Release(src);
}