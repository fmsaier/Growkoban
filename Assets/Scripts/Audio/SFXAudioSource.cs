using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Provides an interface to the AudioSource while supported object pooling
/// and SoundEffect ScriptableObjects.
/// 
/// Note: There is no "Play" handle since the play happens when the sound is spanwed
///       and then the object is released when the audio is done playing. Meaning, you
///       cannot Stop then Play but you can Pause and then Resume.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SFXAudioSource : MonoBehaviour
{
    AudioSource source;
    AudioSource Source
    {
        get
        {
            if (source == null)
                source = GetComponent<AudioSource>();
            return source;
        }
    }

    public AudioClip Clip { get { return Source.clip; } }

    bool isPaused = false;
    bool isConfigured = false;
    bool isMasterAudioPaused = false;
    public bool IsPaused { get { return isPaused; } }
    public bool IsPlaying { get { return Source.isPlaying; } }

    /// <summary>
    /// The menu open/close pausing/resuming all the audio
    /// </summary>
    /// <param name="playAudio"></param>
    public void OnSoundToggled(bool playAudio)
    {
        // Ignore this request when it is not configured
        if (!isConfigured)
            return;

        isMasterAudioPaused = (playAudio == false);

        // If this sound was already paused before opening the menu
        // don't unpause it
        if (playAudio && !isPaused && !Source.isPlaying)
            Source.Play();

        if (!playAudio && Source.isPlaying)
            Source.Pause();
    }

    public void Pause()
    {
        if (isConfigured && Source.isPlaying)
        {
            isPaused = true;
            Source.Pause();
        }
    }

    public void Resume()
    {
        if (isConfigured && !Source.isPlaying)
        {
            isPaused = false;
            Source.Play();
        }
    }

    public void Stop()
    {
        // We have to trigger a manual release for clips that loop
        // But only release it if it was playing since a stopped clip usually means a released one
        if (IsPlaying && !isPaused && Source.loop)
            Release();

        Source.Stop();
        isPaused = false;
        isConfigured = false;
    }

    /// <summary>
    /// TODO: Review why sometimes objects being access from the pool are disabled
    /// </summary>
    /// <param name="isEnabled"></param>
    public void SetState(bool isEnabled) => gameObject.SetActive(true);
    public void SetOutputMixer(AudioMixerGroup mixerGroup) => Source.outputAudioMixerGroup = mixerGroup;

    /// <summary>
    /// Configures the audiosource and plays the audio based on the effects given
    /// Triggers the an auto-release when the audio is done playing
    /// 
    /// Note: For looping sounds you must invoke <see cref="Stop"/> to release them
    /// </summary>
    /// <param name="sfx"></param>
    /// <param name="clipIndex"></param>
    /// <returns></returns>
    public SFXAudioSource Play(SoundEffect sfx, int clipIndex = -1)
    {
        if (sfx == null || gameObject == null)
        {
            // Since a clip was no given we will release this and end here
            // Also, if the object is disabled for some reason, we want to do the same
            Release();
            return null;
        }

        if (!isConfigured)
            Configure(sfx, clipIndex);

        // If the clip is already playing then we don't need to play it again
        if (Source.isPlaying)
            return this;

        // Play it and start the routine to release this source
        Source.Play();

        // For non-looping sounds we want to auto-release when they are done playing
        if(!sfx.loop)
            StartCoroutine(ReleaseAudioSourceRoutine());

        return this;
    }

    /// <summary>
    /// Configures the AudioSource based on the given effects
    /// Does not start playback until <see cref="Play(SoundEffect, int)"/> is called
    /// Note: Play() will configure it if this was not called first
    /// </summary>
    /// <param name="sfx"></param>
    /// <param name="clipIndex"></param>
    /// <returns></returns>
    public SFXAudioSource Configure(SoundEffect sfx, int clipIndex = -1)
    {
        AudioClip clip = sfx.Clip(clipIndex);
        if (clip == null)
        {
            // Since there is no clip
            // we need to release this and return here
            Release();
            return null;
        }

        // Setup the source
        Source.clip = clip;
        Source.pitch = sfx.Pitch;
        Source.loop = sfx.loop;
        Source.volume = sfx.volume;
        Source.playOnAwake = false;

        isConfigured = true;
        return this;
    }

    /// <summary>
    /// Waits for the sound to finish playing to release it back into the pool
    /// Is aware that a sound might be paused so that it does not get released
    /// until it is unpaused and finishes
    /// </summary>
    /// <returns></returns>
    IEnumerator ReleaseAudioSourceRoutine()
    {
        while (Source.isPlaying)
        {
            yield return new WaitForEndOfFrame();

            // Audio source is paused, which means is is not playing
            // however,  we don't want to release it yet since we can resume it
            while (isPaused || isMasterAudioPaused)
                yield return new WaitForEndOfFrame();
        }

        Stop();
        Release();
    }

    /// <summary>
    /// Releases itself back into the pool
    /// </summary>
    void Release()
    {
        isConfigured = false;
        AudioManager.instance.OnSourceReleased(this);
    }
}