using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [HideInInspector]public AudioSource audioSource;

    public bool isSFXAduio;
    public bool isMusicAudio;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isSFXAduio)
        {
            audioSource.volume = AudioManager.instance.SFX_Volume * AudioManager.instance.master_Volume;
        }
        else if(isMusicAudio)
        {
            audioSource.volume = AudioManager.instance.Music_Volum * AudioManager.instance.master_Volume;
        }
    }
}
