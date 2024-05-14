using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CowboyController : MonoBehaviour
{
    Animator animator;
    [SerializeField] SpriteRenderer spriteRendere;

    public AudioClip[] audioClip;

    // 0 Position, 1 Rotation
    [SerializeField] Vector3[] local_Transform;
    [SerializeField] Vector3[] remote_Transform;
    [Range(0, 1)]
    public float alpha_ChargingIndex;

    public Blood blood;

    PhotonView photonView;

    enum AudioType
    {
        Attack,
        Death,
        MaxIndex
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        blood = GetComponentInChildren<Blood>();
    }

    public void Init_Cowboy(bool isMine)
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsMine", isMine);

        if(isMine)
        {
            transform.localPosition = local_Transform[0];
            transform.localEulerAngles = local_Transform[1];
        }
        else
        {
            transform.localPosition = remote_Transform[0];
            transform.localEulerAngles = remote_Transform[1];
        }
    }


    public IEnumerator OnAni(string name, float rateTime)
    {
        //photonView.RPC("RPC_Ani", RpcTarget.All, name);
        photonView.RPC("PlayAudio", RpcTarget.All, name);

        if (photonView.IsMine)
        {
            name = "MY_" + name;
            animator.SetTrigger(name);
        }
        else
        {
            name = "Other_" + name;
            animator.SetTrigger(name);
        }

        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > rateTime)
                break;

            yield return null;
        }
    }

    [PunRPC]
    void RPC_Ani(string name)
    {
        if (photonView.IsMine)
        {
            name = "MY_" + name;
            animator.SetTrigger(name);
        }
        else
        {
            name = "Other_" + name;
            animator.SetTrigger(name);
        }

        PlayAudio(name);
    }

    [PunRPC]
    void PlayAudio(string name)
    {
        if (name == "Attack")
        {
            AudioManager.instance.SFXPlay(name, audioClip[0]);
        }
        else if (name == "Death")
        {
            AudioManager.instance.SFXPlay(name, audioClip[(int)AudioType.Death]);
        }
    }

    public IEnumerator BeTransparent()
    {
        Color alpha_Color = spriteRendere.material.color;
        while (true)
        {
            if (alpha_Color.a <= alpha_ChargingIndex)
                break;

            alpha_Color.a -= 0.01f;
            spriteRendere.material.color = alpha_Color;

            yield return null;
        }
        alpha_Color.a = 1;
        spriteRendere.material.color = alpha_Color;
    }
}
