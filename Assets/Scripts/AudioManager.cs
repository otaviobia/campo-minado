using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<Audio> audios;

    private AudioSource _TocadorDeAudio;

    public enum TipoDeSom
    {
        EXPLOSAO,
        CLIQUE,
        VENCER,
        BOTAR_BANDEIRA,
        TIRAR_BANDEIRA
    }

    void Start()
    {
        _TocadorDeAudio = GetComponent<AudioSource>();
    }

    public void TocarSom(TipoDeSom som, float volume)
    {
        _TocadorDeAudio.PlayOneShot(audios[(int)som].Fonte, volume);
    }
}

[System.Serializable]
public class Audio
{
    public string Nome;
    public AudioClip Fonte;
}
