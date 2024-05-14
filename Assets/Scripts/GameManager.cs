using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject battleZonePrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject hide_View;
    [SerializeField] GameObject[] checkReadyObjects;

    [Header("Camera")]
    [SerializeField] float camRadiusOffset;
    [SerializeField] float camHeight;
    [SerializeField] float groundRadius;

    [SerializeField] Dice[] m_Dices;
    [Space]
    [Header("Toggle View")]
    [SerializeField] StatUI[] statUIs;
    [SerializeField] AlphaController statToggleView;
    [SerializeField] AlphaController playerPositionToggleView;
    [SerializeField] AlphaController attackPositionToggleView;
    [SerializeField] PopUp popUp_Script;
    [SerializeField] CheckReadyUI checkReadyUI_Script;

    [Header("Timer")]
    [SerializeField] float selectTimerDuration = 5f;
    [SerializeField] Text timerText;
    [SerializeField] Round roundText;
    [Space]

    [SerializeField] Toggle skipButton;

    Camera cam;
    public AudioClip[] backGorund_Music;

    PhotonView photonView;

    GameObject sand_Wind;
    GameObject hide_View_Text;

    float createSpacingDegree;
    float createLocalAngle;

    IEnumerator setStatEnumerator;
    IEnumerator setPlayerPositionEnumerator;
    IEnumerator setPlayerAttackPositionEnumerator;
    IEnumerator setStartFightEnumerator;

    bool isSetPlayerPosition = false;
    bool isSetPlayerAttackPosition = false;
    bool isSetStartFight = false;

    int selectedStatIndex = -1;
    int selectedPlayerPosIndex = -1;
    int selectedAttackPosIndex = -1;

    public BattleZone LocalBattleZone;
    public PlayerController LocalPlayerController;
    
    public BattleZone RemoteBattleZone;
    public PlayerController RemotePlayerController;

    BattleZone battleZone;

    bool isActiveDice = true;
    bool isSkip = false;

    WaitForSeconds waitUI = new WaitForSeconds(1.0f);

    enum ViewEnum
    {
        EMPTY,
        STAT,
        STATCOLOR,
        PLAYERPOSITION,
        ATTACKPOSITION,
    }

    #region UnityFunction
    private void Awake()
    {
        cam = Camera.main;

        photonView = GetComponent<PhotonView>();


        sand_Wind = hide_View.transform.GetChild(0).gameObject;
        hide_View_Text = hide_View.transform.GetChild(1).gameObject;


        createSpacingDegree = PhotonNetwork.OfflineMode ? 360.0f : 360.0f / PhotonNetwork.CurrentRoom.MaxPlayers;
        createLocalAngle = createSpacingDegree * (PhotonNetwork.LocalPlayer.ActorNumber - 1);
    }

    // battleZone Spacing = 20

    private void Start()
    {
        CreateBoard();
        StartCoroutine(MainFlow());
        StartCoroutine(PlayBackGround_Mucis());
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    public void LeavRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    void CreateBoard()
    {
        Vector3 pos = new Vector3(groundRadius * Mathf.Cos(createLocalAngle * Mathf.Deg2Rad), groundRadius * Mathf.Sin(createLocalAngle * Mathf.Deg2Rad), 0f);

        GameObject battleZoneObj = PhotonNetwork.Instantiate(battleZonePrefab.name, Vector3.zero, Quaternion.identity);
        battleZone = battleZoneObj.GetComponent<BattleZone>();
    }

    void CreateCam()
    {
        Vector3 pos = new Vector3((groundRadius + camRadiusOffset) * Mathf.Cos(createLocalAngle * Mathf.Deg2Rad), camHeight, (groundRadius + camRadiusOffset) * Mathf.Sin(createLocalAngle * Mathf.Deg2Rad));
        cam.transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, 0f, 0f));
    }

    // Only MasterClient Call this 
    public void ThrowDice()
    {
        int[] diceValues = new int[m_Dices.Length];

        for (int i = 0; i < diceValues.Length; i++)
            diceValues[i] = Random.Range(1, 6 + 1);

        photonView.RPC("ChangeDiceNumber", RpcTarget.AllBuffered, diceValues);
    }

    [PunRPC]
    void ChangeDiceNumber(int[] diceValues)
    {
        for (int i = 0; i < m_Dices.Length; i++)
            m_Dices[i].Number = diceValues[i];
    }

    IEnumerator IsDicesSetCoroutine()
    {
        while (true)
        {
            int setCount = 0;
            
            foreach (var item in m_Dices)
            {
                if (!item.Number.Equals(0))
                    setCount++;
            }

            if (setCount.Equals(m_Dices.Length))
                yield break;

            yield return null;
        }
    }

    IEnumerator PlayBackGround_Mucis()
    {
        int i = 0;
        WaitForSeconds[] musicTime = new WaitForSeconds[3];

        for (int n = 0; n < 3; n++)
        {
            musicTime[n] = new WaitForSeconds(backGorund_Music[n].length + 0.2f);
        }

        while (true)
        {
            AudioManager.instance.MusicPlay(backGorund_Music[i].name, backGorund_Music[i]);
            yield return musicTime[i];
            i++;
            if (i >= 3)
                i = 0;
        }
    }

    IEnumerator MainFlow()
    {
        // init

        // ThrowDice Call MASTER

        PhotonNetwork.Instantiate(playerPrefab.name, battleZone.areas[1].transform.position, Quaternion.identity);

        yield return StartCoroutine(Roding());
        Debug.LogError("END RODiNG 끝!");
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                yield return StartCoroutine(StartRound());

                yield return StartCoroutine(SelectStat());

                yield return StartCoroutine(SelectPlayerPosition());

                yield return StartCoroutine(SelectPlayerAttackPosition());

                yield return StartCoroutine(StartFight());

                yield return StartCoroutine(EndRound());
            }

            if (LocalPlayerController.statText_Script.HPText <= 0 ||
            RemotePlayerController.statText_Script.HPText <= 0)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1); //test
        popUp_Script.WhosWin(LocalPlayerController.statText_Script.HPText, RemotePlayerController.statText_Script.HPText); 

        yield break;
    }

    #region MainFlow

    [PunRPC]
    void SetUIView(ViewEnum uiViewIndex)
    {
        statToggleView.OnTransparent();
        playerPositionToggleView.OnTransparent();
        attackPositionToggleView.OnTransparent();

        switch (uiViewIndex)
        {
            case ViewEnum.STAT:
                statToggleView.gameObject.SetActive(false);
                statToggleView.gameObject.SetActive(true);
                break;
            case ViewEnum.PLAYERPOSITION:
                playerPositionToggleView.gameObject.SetActive(false);
                playerPositionToggleView.gameObject.SetActive(true);
                break;
            case ViewEnum.ATTACKPOSITION:
                attackPositionToggleView.gameObject.SetActive(false);
                attackPositionToggleView.gameObject.SetActive(true);
                break;              
            case ViewEnum.EMPTY:
                break;
            default:
                break;
        }
    }

    IEnumerator Roding()
    {
        //플레이어 생성까지 걸리는 대기시간
        yield return StartCoroutine(popUp_Script.Start_Beaner(PopUp.PopUp_type.Start));
        RemotePlayerController.gameObject.SetActive(true);
        photonView.RPC("SetActiveDice", RpcTarget.AllBuffered, true);
    }

    IEnumerator StartRound()
    {
        roundText.round = roundText.round + 1;

        yield return waitUI;
        photonView.RPC("SetActiveDice", RpcTarget.AllBuffered, true);

        ThrowDice();
    }

    IEnumerator SelectStat()
    {    
        int dicesIndex = 0;

        photonView.RPC("RPCRemotePlayerActive", RpcTarget.All, true);

        while (true)
        {
            if (setStatEnumerator == null)
            {
                if (dicesIndex < m_Dices.Length)
                {
                    photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.EMPTY);

                    photonView.RPC("SetActiveDice", RpcTarget.All, false);
                    photonView.RPC("SetActiveDice", RpcTarget.All, dicesIndex, true);
                    yield return StartCoroutine(m_Dices[dicesIndex].Re_Roll());

                    photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.STAT);
                    photonView.RPC("SetStat", RpcTarget.All, dicesIndex, m_Dices[dicesIndex].Number, PhotonNetwork.Time);
                    dicesIndex++;
                }
                else
                {
                    photonView.RPC("SetActiveDice", RpcTarget.All, false);
                    break;
                }
            }

            yield return null;
        }
    }

    IEnumerator SelectPlayerPosition()
    {
        yield return StartCoroutine(SandWind_Active());

        isSetPlayerPosition = false;

        photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.EMPTY);
        yield return waitUI;
        photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.PLAYERPOSITION);

        while (true)
        {
            if (isSetPlayerPosition)
                break;
            else
            {
                if (setPlayerPositionEnumerator == null)
                    photonView.RPC("SetPlayerPosition", RpcTarget.All, PhotonNetwork.Time);
            }

            yield return null;
        }
    }

    IEnumerator SelectPlayerAttackPosition()
    {
        isSetPlayerAttackPosition = false;
        photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.EMPTY);
        yield return waitUI;
        photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.ATTACKPOSITION);

        while (true)
        {
            if (isSetPlayerAttackPosition)
                break;
            else
            {
                if (setPlayerAttackPositionEnumerator == null)
                    photonView.RPC("SetAttackPosition", RpcTarget.All, PhotonNetwork.Time);
            }

            yield return null;
        }
    }

    IEnumerator StartFight()
    {
        photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.EMPTY);
        yield return waitUI;

        isSetStartFight = false;

        while (true)
        {
            if (isSetStartFight)
                break;
            else
            {
                if (setStartFightEnumerator == null)
                    photonView.RPC("SetStartFight", RpcTarget.All, PhotonNetwork.Time);
            }

            yield return null;
        }
    }

    IEnumerator EndRound()
    {
        while (true)
        {
            if (LocalPlayerController.IsAttackMotion)
                break;
            yield return null;
        }
        LocalPlayerController.IsAttackMotion = false;

        yield return new WaitForSeconds(3.0f);

        popUp_Script.photonView.RPC("SetActivePopUp", RpcTarget.All, PopUp.PopUp_type.FightStart, false);

        popUp_Script.photonView.RPC("Set_SuccessORFail", RpcTarget.All, 0, false);
        popUp_Script.photonView.RPC("Set_SuccessORFail", RpcTarget.All, 1, false);

        LocalBattleZone.photonView.RPC("SetActiveAreas", RpcTarget.All, false);
        RemoteBattleZone.photonView.RPC("SetActiveAreas", RpcTarget.All, false);
    }
    #endregion

    #region Stat
    [PunRPC]
    void SetStat(int diceIndex, int diceValue, double timerStart)
    {
        Debug.LogError("SetStat CALL");

        setStatEnumerator = SetStatCoroutine(diceIndex, diceValue, timerStart);
        StartCoroutine(setStatEnumerator);
    }

    IEnumerator SetStatCoroutine(int diceIndex,int diceValue, double timerStart)
    {
        foreach (var item in statUIs)
            item.Value = diceValue;

        yield return StartCoroutine(Timer(selectTimerDuration, timerStart));

        if (selectedStatIndex.Equals(-1))
            selectedStatIndex = Random.Range(0, statUIs.Length);

        switch (selectedStatIndex)
        {
            case 0:
                LocalPlayerController.statText_Script.STRText += diceValue;
                Debug.LogError("Add STR State: " + diceValue);
                break;
            case 1:
                LocalPlayerController.statText_Script.HPText += diceValue;
                LocalPlayerController.HpBar_Script.Set_Slider = LocalPlayerController.statText_Script.HPText;
                Debug.LogError("Add HP State " + diceValue);
                break;
        }

        selectedStatIndex = -1;
        m_Dices[diceIndex].Number = diceValue;

        if (PhotonNetwork.IsMasterClient)
            setStatEnumerator = null;
    }

    public void SelectStat(int statIndex)
    {
        if (TogglesController.isOn == false)
            return;

        selectedStatIndex = statIndex;
        Debug.LogError(selectedStatIndex);
    }
    #endregion

    #region PlayerPosition
    [PunRPC]
    void SetPlayerPosition(double timerStart)
    {
        Debug.LogError("SetPlayerPosition CALL");
        setPlayerPositionEnumerator = SetPlayerPositionCoroutine(timerStart);
        StartCoroutine(setPlayerPositionEnumerator);
    }

    IEnumerator SetPlayerPositionCoroutine(double timerStart)
    {
        LocalBattleZone.SetSectionSprite(BattleZone.SpritesType.Position_Section);

        yield return StartCoroutine(Timer(selectTimerDuration, timerStart));

        LocalBattleZone.SetSectionSprite(BattleZone.SpritesType.Basic_Section);

        if (selectedPlayerPosIndex.Equals(-1))
        {
            selectedPlayerPosIndex = Random.Range(0, battleZone.areas.Length);
            LocalPlayerController.LocalPositionIndex = selectedPlayerPosIndex;
        }
        selectedPlayerPosIndex = -1;

        if (PhotonNetwork.IsMasterClient)
        {
            setPlayerPositionEnumerator = null;
            isSetPlayerPosition = true;
        }
    }

    public void SelectPlayerPosition(int playerPosIndex)
    {
        selectedPlayerPosIndex = playerPosIndex;
        LocalPlayerController.LocalPositionIndex = selectedPlayerPosIndex;

        switch (selectedPlayerPosIndex)
        {
            case 0:
                Debug.LogError("My Left Position");
                break;
            case 1:
                Debug.LogError("My Middle Position");
                break;
            case 2:
                Debug.LogError("My Right Position");
                break;
        }
    }
    #endregion

    #region AttackPosition
    [PunRPC]
    void SetAttackPosition(double timerStart)
    {
        Debug.LogError("SetAttackPosition CALL");
        setPlayerAttackPositionEnumerator = SetAttackPositionCoroutine(timerStart);
        StartCoroutine(setPlayerAttackPositionEnumerator);
    }

    IEnumerator SetAttackPositionCoroutine(double timerStart)
    {
        selectedAttackPosIndex = -1;

        RemoteBattleZone.SetSectionSprite(BattleZone.SpritesType.Attack_Section);

        yield return StartCoroutine(Timer(selectTimerDuration, timerStart));

        RemoteBattleZone.SetSectionSprite(BattleZone.SpritesType.Basic_Section);

        if (selectedAttackPosIndex.Equals(-1))
        {
            selectedAttackPosIndex = Random.Range(0, battleZone.areas.Length);
            LocalPlayerController.SetAimObject(selectedAttackPosIndex, true);
            LocalPlayerController.attackPosition = selectedAttackPosIndex;
        }

        switch (selectedAttackPosIndex)
        {
            case 0:
                Debug.LogError("Attack Left Position");
                break;
            case 1:
                Debug.LogError("Attack Middle Position");
                break;
            case 2:
                Debug.LogError("Attack Right Position");
                break;
        }  

        if (PhotonNetwork.IsMasterClient)
        {
            setPlayerAttackPositionEnumerator = null;
            isSetPlayerAttackPosition = true;
        }
    }

    public void SelectAttackPosition(int attackPosIndex)
    {
        if (TogglesController.isOn == false)
            return;

        LocalPlayerController.SetAimObject(attackPosIndex, true);
        selectedAttackPosIndex = attackPosIndex;
        LocalPlayerController.attackPosition = selectedAttackPosIndex;
    }
    #endregion

    #region Fight

    [PunRPC]
    void SetStartFight(double timerStart)
    {
        Debug.LogError("Start Fight");
        setStartFightEnumerator = SetStartFightCoroutine(timerStart);
        StartCoroutine(setStartFightEnumerator);
    }

    IEnumerator SetStartFightCoroutine(double timerStart)
    { 
        hide_View.SetActive(false);
        RemotePlayerController.gameObject.SetActive(true);
        LocalPlayerController.RemotePositionIndex = LocalPlayerController.LocalPositionIndex; //Set Remote Player Position
        LocalPlayerController.statText_Script.PunHPText = LocalPlayerController.statText_Script.HPText;
        LocalPlayerController.HpBar_Script.PunRPC_SetSlider = LocalPlayerController.statText_Script.HPText;
        LocalPlayerController.statText_Script.PunSTRText = LocalPlayerController.statText_Script.STRText;
        ///OnlySetActive_RemotePlayer_Cowboy(true);
        popUp_Script.SetActivePopUp(PopUp.PopUp_type.FightStart, true);

        int attackPosIndexRemoteView = -selectedAttackPosIndex + 2;

        yield return StartCoroutine(Timer(1, timerStart));

        LocalPlayerController.photonView.RPC("AttackMotion", RpcTarget.All, attackPosIndexRemoteView);

        while(true)
        {
            if (LocalPlayerController.IsAttackMotion)
                break;
            yield return null;
        }

        Debug.LogError("selectedAttackPosIndex: " + selectedAttackPosIndex);
        Debug.LogError("attackPosIndexRemoteView: " + attackPosIndexRemoteView);
        Debug.LogError("Remote PositionIndex: " + RemotePlayerController.LocalPositionIndex);

        if (RemotePlayerController.LocalPositionIndex.Equals(attackPosIndexRemoteView))
        {
            RemotePlayerController.photonView.RPC("PRC_SetActive_Blood", RpcTarget.All, true);
            RemotePlayerController.statText_Script.PunHPText = RemotePlayerController.statText_Script.HPText - LocalPlayerController.statText_Script.STRText;
            RemotePlayerController.HpBar_Script.PunRPC_SetSlider = RemotePlayerController.statText_Script.HPText;

            if (RemotePlayerController.statText_Script.HPText <= 0)
                yield return StartCoroutine(RemotePlayerController.cowboy_Script.OnAni("Death", 0.9f));

            popUp_Script.Set_SuccessORFail(0, true);
        }
        else
        {
            popUp_Script.Set_SuccessORFail(1, true);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            setStartFightEnumerator = null;
            isSetStartFight = true;
        }
    }

    #endregion


    #region ETC

    IEnumerator Timer(float duration, double startTime)
    {
        double remainTime = duration - (PhotonNetwork.Time - startTime);

        while (0 <= remainTime)
        {
            if (isSkip && remainTime > 0.5f)
            {
                remainTime = 0.5f;
                photonView.RPC("BoolIsSkip", RpcTarget.All, false);
                Debug.LogError("Time Skip");
            }

            timerText.text = ((int)remainTime).ToString("N2");
            
            remainTime -= Time.deltaTime;

            yield return null;
        }

        timerText.text = "0.0";
        Debug.LogError("TimerEnd");
    }

    public void SetSkipToggle(bool value)
    {
        ExitGames.Client.Photon.Hashtable skipTable = new ExitGames.Client.Photon.Hashtable()
        {
            { "IsSkip", value },
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(skipTable);

        if(photonView.IsMine)
            photonView.RPC("SetReadyUI", RpcTarget.All, 0, value);
        else
            photonView.RPC("SetReadyUI", RpcTarget.All, 1, value);
    }

    [PunRPC]
    public void SetReadyUI(int PlayerNumber, bool value) => checkReadyUI_Script.SetReadyUI(PlayerNumber, value);

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        int count = 0;

        foreach (var item in PhotonNetwork.CurrentRoom.Players)
        {
            if (item.Value.CustomProperties.TryGetValue("IsSkip", out object value))
            {
                if ((bool)value)
                {
                    count++;
                }
                else
                {
                    count--;
                }
            }
        }

        if (count.Equals(PhotonNetwork.CurrentRoom.PlayerCount))
            photonView.RPC("SetSkip", RpcTarget.All, true);
    }

    IEnumerator SandWind_Active()
    {
        photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.EMPTY);

        yield return waitUI;

        // 모래 바람이 처음 활성화 되었을 때
        photonView.RPC("SandWind_Ture", RpcTarget.All, null);

        yield return StartCoroutine(RemotePlayerController.cowboy_Script.BeTransparent());
        //RemotePlayerController.gameObject.SetActive(false);
        photonView.RPC("RPCRemotePlayerActive", RpcTarget.All, false);
        yield return new WaitForSeconds(1.75f);

        photonView.RPC("HideText_False", RpcTarget.All, null);

        //photonView.RPC("SetUIView", RpcTarget.All, ViewEnum.PLAYERPOSITION);
    }

    public void Surrender()
    {
        popUp_Script.WhosWin(-100, 100);
        popUp_Script.PunWhosWind(100,-100);
    }

    #endregion

    #region PunRPC

    [PunRPC]
    public void SetActiveDice(bool value)
    {
        foreach (var item in m_Dices)
        {
            item.gameObject.SetActive(value);
        }
    }

    [PunRPC]
    public void SetActiveDice(int number, bool value)
    {
        m_Dices[number].gameObject.SetActive(value);
    }

    [PunRPC]
    public void SetSkip(bool value)
    {
        TogglesController.isOn = false;
        skipButton.isOn = false;
        TogglesController.isOn = true;

        if (timerText.text == "0.0")
            return;

        isSkip = value;
    }

    [PunRPC]
    void BoolIsSkip(bool value) => isSkip = value;

    [PunRPC]
    public void SandWind_Ture()
    {
        hide_View.SetActive(true);
        hide_View_Text.SetActive(true);
        sand_Wind.SetActive(true);
    }
    [PunRPC]
    public void HideText_False() => hide_View_Text.SetActive(false);

    [PunRPC]
    void RPCRemotePlayerActive(bool value) => RemotePlayerController.gameObject.SetActive(value);

    [PunRPC]
    void OnlySetActive_RemotePlayer_Cowboy(int index, bool value) => RemotePlayerController.SetActivePlayerObj(index, value);

    [PunRPC]
    void PRC_SetActive_RemotePlayer_Blood(bool value) => RemotePlayerController.cowboy_Script.blood.gameObject.SetActive(value);

    #endregion

}
