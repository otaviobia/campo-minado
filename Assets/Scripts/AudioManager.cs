using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<Sound> sounds;
    private AudioSource m_AudioSource;

    public enum SoundType
    {
        EXPLOSION,
        SELECTION,
        WIN,
        FLAG,
        UNFLAG
    }

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType sound, float volume)
    {
        m_AudioSource.PlayOneShot(sounds[(int)sound].source, volume);
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip source;
}
