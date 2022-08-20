using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect", menuName = "Audio/SoundFX")]
public class SoundEffect : ScriptableObject
{
    [Tooltip("Prevents multiple instances of the sound to play at the same time")]
    public bool preventMultiple = false;

    [Tooltip("Turns this ON if you want the sound to play continiously")]
    public bool loop = false;

    [SerializeField, Tooltip("Single clip to play")]
    AudioClip clip;

    [Tooltip("Random clips to play")]
    public AudioClip[] clips;

    [Range(0.1f, 1f), Tooltip("Volume to play the audio at")]
    public float volume = 1f;

    [SerializeField, Range(0.1f, 1f), Tooltip("Pitch to play the audio at")]
    float pitch = 1f;

    /// <summary>
    /// Returns the pitch to play the clip at
    /// </summary>
    public float Pitch
    {
        get
        {
            var _pitch = pitch;
            if (randomPitch.min > 0 && randomPitch.max > 0)
                _pitch = Random.Range(randomPitch.min, randomPitch.max);

            return _pitch;
        }
    }

    [Tooltip("Minimum/Maximum values to randomize the pitch at. zero or below will be ignored")]
    public MinMaxFloat randomPitch = new MinMaxFloat(0f, 0f);

    /// <summary>
    /// Returns the AudioClip to use based on the clip or clips assigned to this sound effect
    /// </summary>
    /// <param name="clipIndex"></param>
    /// <returns></returns>
    public AudioClip Clip(int clipIndex = -1)
    {
        AudioClip _clip = clip;

        // Looks like we have a collection of clops to chose from
        // Let's get a random one unless we were given a specific clip to choose from
        if (clips != null && clips.Length > 0)
        {
            // Autocap the index to the last element
            if (clipIndex >= clips.Length)
                clipIndex = clips.Length - 1;

            // Grab the specific clip requested
            // Or Get a random one
            if (clipIndex >= 0)
                _clip = clips[clipIndex];
            else
                _clip = clips[Random.Range(0, clips.Length)];
        }

        return _clip;
    }
}
