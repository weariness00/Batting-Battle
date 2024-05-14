using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HpBar : MonoBehaviour
{
    public Slider slider;
    public GameObject fill;

    public Sprite[] hpBar_Sprite;

    Image fill_Image;

    public Vector3 default_LocalPosition;
    public Vector3 default_RemotePosition;

    PhotonView photonView;

    enum hpBar_SpriteNumber
    {
        LocalPlayerHpBar,
        RemotePlayerHpBar
    }

    #region PunRPC

    public int Set_Slider
    {
        get => (int)slider.value * 30;
        set => SetSlider(value);
    }

    public int PunRPC_SetSlider
    {
        get => (int)slider.value * 30;
        set => photonView.RPC("SetSlider", RpcTarget.All, value);
    }

    #endregion

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        fill_Image = fill.GetComponent<Image>();

        slider.value = 0.3f;

        Init_Sprite();
    }

    void Init_Sprite()
    {
        if (photonView.IsMine)
            fill_Image.sprite = hpBar_Sprite[(int)hpBar_SpriteNumber.LocalPlayerHpBar];
        else
            fill_Image.sprite = hpBar_Sprite[(int)hpBar_SpriteNumber.RemotePlayerHpBar];
    }

    [PunRPC]
    public void SetSlider(int value)
    {
        float result = (float)value / 30;

        StartCoroutine(GraduallyChange_Slider(result));
    }

    IEnumerator GraduallyChange_Slider(float result)
    {
        float value;
        float amount = result - slider.value;
        float result_Amout = slider.value + ((amount / 100) * 100);

        for(int i = 0; i < 100; i++)
        {
            if (slider.value == result_Amout ||
                slider.value == 0 ||
                slider.value == 1)
                break;

            value = slider.value + (amount / 100);
            slider.value = value;
            yield return null;
        }
        slider.value = result;
    }

    public void SetPositon(Vector3 pos, bool value)
    {
        RectTransform rt;
        rt = gameObject.transform as RectTransform;
        
        if (value)//Loacl
        {
            rt.anchoredPosition = new Vector3(pos.x, default_LocalPosition.y, 0);
        }
        else
        { 
            rt.anchoredPosition = new Vector3(pos.x, default_RemotePosition.y, 0);
        }
    }
}
