using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Dice : MonoBehaviour
{
    [SerializeField] Sprite[] m_Sprites;
    [SerializeField] Sprite[] highlight_sprite;
    int m_Number;

    Image m_Image;
    Animator animator;
    [SerializeField] AudioClip[] audioClip;

    public PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        
        m_Number = 0;
        m_Image = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    public void ChangeHighlight_sprite(int value)
    {
        m_Image.sprite = highlight_sprite[value - 1];
    }

    public int Number
    { 
        get => m_Number;
        set
        {
            m_Number = value;
            m_Image.sprite = m_Sprites[value - 1];
        }
    }

    public IEnumerator Re_Roll()
    {
        photonView.RPC("SetSprite", RpcTarget.All);
        photonView.RPC("On_ReRoll", RpcTarget.All, true);

        float start_Time = Time.time;
        float now_Time = Time.time - start_Time;
        float end_Time = 1.5f;

        while(true)
        {
            if (now_Time > end_Time)
                break;

            now_Time = Time.time - start_Time;
            yield return null;
        }

        photonView.RPC("On_ReRoll", RpcTarget.All, false);

        yield return new WaitForSeconds(0.75f);
    }

    [PunRPC]
    void SetSprite() => m_Image.sprite = m_Sprites[m_Number - 1];

    [PunRPC]
    public void On_ReRoll(bool On_OFF)
    {
        if (On_OFF)
        {
            AudioManager.instance.SFXPlay("Dice_ReRoll", audioClip[0]);
            animator.SetTrigger("On_Re-Roll");
        }
        else
            animator.SetTrigger("Off_Re-Roll");
    }

}
