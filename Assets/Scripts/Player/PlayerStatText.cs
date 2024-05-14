using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStatText : MonoBehaviour
{
    public Text[] stat_Text;
    public Text[] plusStat_Text;
    public Text name_Text;

    int hp;
    int str;
    
    [Space]
    public Vector3 default_HP_LocalPosition;
    public Vector3 default_STK_LocalPosition;

    [Space]
    public Vector3 default_HP_RemotePosition;
    public Vector3 default_STK_RemotePosition;

    [Space]
    public Vector3 default_Name_LocalPosition;
    public Vector3 default_Name_RemotePosition;

    PhotonView photonView;

    public enum TextObjectIndex
    {
        HPTextIndex,
        STKTextIndex,
        MaxInex = STKTextIndex + 1
    }

    #region PunRPC

    public int HPText
    {
        get => hp;
        set => SetHPText(value);
    }
    public int PunHPText
    {
        get => hp;
        set => photonView.RPC("SetHPText", RpcTarget.All, value);
    }

    public int STRText
    {
        get => str;
        set => SetSTRText(value);
    }

    public int PunSTRText
    {
        get => str;
        set => photonView.RPC("SetSTRText", RpcTarget.All, value);
    }


    #endregion

    #region PunRPC_Function

    [PunRPC]
    public void SetHPText(int value)
    {
        if(value != hp)
           StartCoroutine(OnPlusText(0, value));

        hp = value;
        if (hp > 30)
            hp = 30;
        stat_Text[(int)TextObjectIndex.HPTextIndex].text = "<color=#1DDB16> " + hp.ToString() + "</color>";
    }

    [PunRPC]
    public void SetSTRText(int value)
    {
        if(value != str)
            StartCoroutine(OnPlusText(1, value));

        str = value;
        stat_Text[(int)TextObjectIndex.STKTextIndex].text = "<color=#FF0000> " + str.ToString() + "</color>";
    }

    #endregion

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        hp = 10; str = 1;
        stat_Text[(int)TextObjectIndex.HPTextIndex].text = "<color=#1DDB16> " + hp.ToString() + "</color>";
        stat_Text[(int)TextObjectIndex.STKTextIndex].text = "<color=#FF0000> " + str.ToString() + "</color>";

        Init_Text();
    }

    public void Init_Text()
    {
        Debug.LogError("Init_Player_Text");

        RectTransform rt_HP, rt_STK, rt_Name;
        rt_HP = stat_Text[(int)TextObjectIndex.HPTextIndex].gameObject.transform as RectTransform;
        rt_STK = stat_Text[(int)TextObjectIndex.STKTextIndex].gameObject.transform as RectTransform;
        rt_Name = name_Text.gameObject.transform as RectTransform;

        if (photonView.IsMine)
        {
            rt_HP.anchoredPosition = default_HP_LocalPosition;
            rt_STK.anchoredPosition = default_STK_LocalPosition;
            rt_Name.anchoredPosition = default_Name_LocalPosition;
        }
        else
        {
            rt_HP.anchoredPosition = default_HP_RemotePosition;
            rt_STK.anchoredPosition = default_STK_RemotePosition;
            rt_Name.anchoredPosition = default_Name_RemotePosition;
        }

        foreach (var item in plusStat_Text)
        {
            item.gameObject.SetActive(false);
        }
    }

    IEnumerator OnPlusText(int textIndex, int value)
    {
        int plusValue;
        string mark = "+"; // + , - ;
        Vector2 diffrent_Dis;
        RectTransform rt_PlusText, rt_StatText;
        rt_PlusText = plusStat_Text[textIndex].gameObject.transform as RectTransform;
        rt_StatText = stat_Text[textIndex].gameObject.transform as RectTransform;

        plusStat_Text[textIndex].gameObject.SetActive(true);

        if(textIndex == 0)
        {
            plusValue = value - hp;
            if (plusValue < 0)
                mark = "";
            plusStat_Text[textIndex].text = "<color=#1DDB16> " + mark + plusValue.ToString() + "</color>";
        }
        else
        {
            plusValue = value - str;
            if (plusValue < 0)
                mark = "";
            plusStat_Text[textIndex].text = "<color=#FF0000> " + mark + plusValue.ToString() + "</color>";
        }

        if (photonView.IsMine)
        {
            diffrent_Dis = new Vector2(-100, 0);
            rt_PlusText.anchoredPosition = rt_StatText.anchoredPosition - diffrent_Dis;
            yield return new WaitForSeconds(0.5f);

            while (true)
            {
                if (rt_PlusText.anchoredPosition.x <= rt_StatText.anchoredPosition.x)
                    break;

                rt_PlusText.anchoredPosition += diffrent_Dis / 100;
                yield return null;
            }
        }
    
        plusStat_Text[textIndex].gameObject.SetActive(false);
    }
}