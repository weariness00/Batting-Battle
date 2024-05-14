using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PopUp : MonoBehaviourPun
{
    [SerializeField] GameObject[] popUp;
    [SerializeField] Text Gold_Text;

    [Space]
    [SerializeField] GameObject[] Attack_PopUp;

    bool isSurrender;

    PhotonView photonView;
    public enum PopUp_type
    {
        EMPTY = -1,
        LOSE,
        WIN,
        DRAW,
        Start,
        FightStart,
        MAXINDEX
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        Init_Object();
        isSurrender = false;
    }

    void Init_Object()
    {
        for (int i = 0; i < (int)PopUp_type.MAXINDEX; i++)
            popUp[i].gameObject.SetActive(false);
    }

    [PunRPC]
    public void WhosWin(int playerHp, int enemyHp)
    {
        if (isSurrender)
            return;

        if (playerHp > enemyHp)
        {
            popUp[(int)PopUp_type.WIN].SetActive(true);
            SetGoldText(100);
        }
        else if(enemyHp > playerHp)
        {
            popUp[(int)PopUp_type.LOSE].SetActive(true);
            SetGoldText(-100);
        }
        else if(playerHp == enemyHp)
        {
            popUp[(int)PopUp_type.DRAW].SetActive(false);
            SetGoldText(0);
        }   
    }

    public void SetGoldText(int GoldCount)
    {
        Gold_Text.text = "<color=#FFFF24>" + GoldCount + "</color>";
    }

    public IEnumerator Start_Beaner(PopUp_type type)
    {
        popUp[(int)type].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        popUp[(int)type].SetActive(false);
    }

    [PunRPC]
    public void SetActivePopUp(PopUp_type type, bool value) => popUp[(int)type].SetActive(value);

    [PunRPC]
    public void Set_SuccessORFail(int type, bool value) => Attack_PopUp[type].SetActive(value);

    public void PunWhosWind(int playerHp, int enemyHp)
    {
        isSurrender = true;
        photonView.RPC("WhosWin", RpcTarget.All, playerHp, enemyHp);
    }
}
