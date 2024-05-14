using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public Animator blood_Animator;
    public AudioClip[] audioClip;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(OnBlood());
    }

    public IEnumerator OnBlood()
    {
        AudioManager.instance.SFXPlay(audioClip[0].name, audioClip[0]);

        while (true)
        {
            if (blood_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
                break;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
