using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    Scrollbar[] sound_Scrollbars; 

    [Range(0f, 1f)]
    public float master_Volume = 0.1f;
    [Range(0f, 1f)]
    public float Music_Volum = 0.5f;
    [Range(0f, 1f)]
    public float SFX_Volume = 0.5f;

    private void Awake()
    {
        Init_SoundSlider();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init_SoundSlider()
    {
        sound_Scrollbars[0].value = master_Volume;
        sound_Scrollbars[1].value = Music_Volum;
        sound_Scrollbars[2].value = SFX_Volume;
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject SFXobj = new GameObject(sfxName + " Sound");
        AudioSource audioSource = SFXobj.AddComponent<AudioSource>();
        AudioController audioController = SFXobj.AddComponent<AudioController>();

        audioController.isSFXAduio = true;

        audioSource.volume = SFX_Volume;
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(SFXobj, clip.length);
    }

    public void MusicPlay(string musicName, AudioClip clip)
    {
        GameObject Music_Obj = new GameObject(musicName + " Sound");
        AudioSource audioSource = Music_Obj.AddComponent<AudioSource>();
        AudioController audioController = Music_Obj.AddComponent<AudioController>();

        audioController.isMusicAudio = true;

        audioSource.volume = SFX_Volume;
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(Music_Obj, clip.length);
    }

    public void OnButtenSound(AudioClip clip)
    {
        SFXPlay(clip.name, clip);
    }

    public void OnControlMasterVolum(Scrollbar scrollbar)
    {
        master_Volume = scrollbar.value;
    }

    public void OnControlMusciVolum(Scrollbar scrollbar)
    {
        Music_Volum = scrollbar.value;
    }

    public void OnControlSFXVolum(Scrollbar scrollbar)
    {
        SFX_Volume = scrollbar.value;
    }
}
