using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWind : MonoBehaviour
{
    public AudioController audioController_Script;

    float volum;

    bool isOnEnable;
    private void Awake()
    {
        isOnEnable = false;
    }

    private void Update()
    {
        SoundDecrease();
    }

    private void OnEnable()
    {
        volum = audioController_Script.audioSource.volume;
        audioController_Script.isSFXAduio = false;
        isOnEnable = true;
    }

    private void OnDisable()
    {
        audioController_Script.isSFXAduio = true;
        
        isOnEnable = false;
    }

    void SoundDecrease()
    {
        if (!isOnEnable)
            return;

        if(!volum.Equals(AudioManager.instance.SFX_Volume * AudioManager.instance.master_Volume))
        {
            volum *= AudioManager.instance.SFX_Volume * AudioManager.instance.master_Volume / volum;
            audioController_Script.audioSource.volume = volum;
        }

        if (audioController_Script.audioSource.volume < volum/ 1000)
            return;

        audioController_Script.audioSource.volume -= volum / 1000;

        if (audioController_Script.audioSource.volume < 0)
            audioController_Script.audioSource.volume = 0;
    }
}
