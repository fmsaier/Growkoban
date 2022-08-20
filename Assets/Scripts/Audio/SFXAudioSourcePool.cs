using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SFXAudioSourcePool : MonoBehaviour
{
    ObjectPool<SFXAudioSource> pool;
    public ObjectPool<SFXAudioSource> Pool
    { 
        get 
        {
            if (pool == null)
                pool = new ObjectPool<SFXAudioSource>(CreateSource, OnSourceGet, OnSourceReleased, null, false);
            return pool; 
        } 
    }

    List<SFXAudioSource> sources;
    public List<SFXAudioSource> Sources
    {
        get
        {
            if (sources == null)
                sources = new List<SFXAudioSource>();
            return sources;
        }
    }

    /// <summary>
    /// Returns the first instance of an SFXAudioSource
    /// that is already playing the given clip
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public SFXAudioSource GetSourceAssignedToClip(AudioClip clip) => Sources.Where(s => s.Clip == clip).FirstOrDefault();

    /// <summary>
    /// Creates the game object and components required for an SFXAudioSource to work
    /// assinging it to the right mixes before returning it
    /// </summary>
    /// <returns></returns>
    SFXAudioSource CreateSource()
    {
        // Create the GO for us to attach the source
        var srcName = $"AudioSource_{Sources.Count}";
        var go = new GameObject(srcName);
        go.transform.SetParent(transform);

        // Need a regular audio source
        AudioSource src;
        src = go.AddComponent<AudioSource>();
     
        // Now we create the interface for that audio source
        var sfx = go.AddComponent<SFXAudioSource>();

        // Subscribe to the toggle/stop audio delegates
        AudioManager.instance.TogglePlaybackDelegates += sfx.OnSoundToggled;
        AudioManager.instance.StopSoundDelegates += sfx.Stop;

        Sources.Add(sfx);
        return sfx;
    }

    /// <summary>
    /// Returns the next available source in the pool
    /// </summary>
    /// <returns></returns>
    public SFXAudioSource GetSource() => Pool.Get();

    /// <summary>
    /// Makes the source available
    /// </summary>
    /// <param name="src"></param>
    void OnSourceGet(SFXAudioSource src) => src.SetState(true);

    /// <summary>
    /// Disables the source
    /// </summary>
    /// <param name="src"></param>
    void OnSourceReleased(SFXAudioSource src) => src.SetState(false);

    /// <summary>
    /// Releases the src from the pool
    /// </summary>
    /// <param name="src"></param>
    public void Release(SFXAudioSource src) => Pool.Release(src);
}