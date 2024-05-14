using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Round : MonoBehaviour
{
    Text text;

    int RoundCount;

    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        text = GetComponent<Text>();
        RoundCount = 0;
    }


    public int round
    {
        get => RoundCount;
        set => photonView.RPC("SetRound", RpcTarget.All, value);
    }

    [PunRPC]
    void SetRound(int count)
    {
        Debug.LogError("Round : " + count);
        text.text = "¶ó¿îµå " + count.ToString();
        RoundCount = count;
    }
}
