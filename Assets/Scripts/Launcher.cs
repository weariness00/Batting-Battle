using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] byte maxPlayersPerRoom = 2;

    string gameVersion = "1";

    #region Unity_Function

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.OfflineMode)
            PhotonNetwork.LoadLevel("GameScene");
        else
        {
            Debug.Log("Joined Room");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();


    void LoadArena()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount.Equals(PhotonNetwork.CurrentRoom.MaxPlayers))
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    #endregion

    public void isOnStart()
    {
        PhotonNetwork.OfflineMode = false;
        JoinRandomRoom();
    }

    public void OffLineMode()
    {
        PhotonNetwork.Disconnect();
        Debug.LogError("로그 체크용 1");

        JoinRandomRoom();
        Debug.LogError("로그 체크용 2");
        PhotonNetwork.OfflineMode = true;
        Debug.LogError("로그 체크용 3");
        PhotonNetwork.JoinRandomRoom();
    }
}
