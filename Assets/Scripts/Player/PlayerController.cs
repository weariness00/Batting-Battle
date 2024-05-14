using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] GameObject[] player_objects;

    [Space][SerializeField] Sprite[] player_Sprites;

    [SerializeField] float size = 216;
    [SerializeField] float spacing = 32;
    [SerializeField] float speed = 100;

    [SerializeField] Vector3 localPlayerPosition;
    [SerializeField] Vector3 remotePlayerPosition;

    [HideInInspector] public HpBar HpBar_Script;
    [HideInInspector] public PlayerStatText statText_Script;
    [HideInInspector] public BulletController bullet_Script;
    [HideInInspector] public CowboyController cowboy_Script;

    GameManager gameManager;
    [HideInInspector] public CameraManager cameraManager;

    [HideInInspector] public bool IsAttackMotion = false;
    [HideInInspector] public int attackPosition;

    public enum Object_Type
    {
        Cowboy,
        Bullet,
        Aim,
        HpBar,
        Text,
        MaxIndex
    }

    private void Awake()
    {
        transform.SetSiblingIndex(10);

        HpBar_Script = player_objects[(int)Object_Type.HpBar].GetComponent<HpBar>();
        statText_Script = player_objects[(int)Object_Type.Text].GetComponent<PlayerStatText>();
        bullet_Script = player_objects[(int)Object_Type.Bullet].GetComponent<BulletController>();
        cowboy_Script = player_objects[(int)Object_Type.Cowboy].GetComponent<CowboyController>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cameraManager = Camera.main.GetComponent<CameraManager>();

        if (photonView.IsMine)
            gameManager.LocalPlayerController = this;
        else
            gameManager.RemotePlayerController = this;

        player_objects[(int)Object_Type.Bullet].SetActive(false);
        Init_Position();
    }

    void Init_Position()
    {
        Vector3 pos;

        if (photonView.IsMine)
        {
            pos = gameManager.LocalBattleZone.areas[1].transform.position;
            HpBar_Script.SetPositon(pos, photonView.IsMine);
            cowboy_Script.Init_Cowboy(true);
        }
        else
        {
            pos = gameManager.RemoteBattleZone.areas[1].transform.position;
            HpBar_Script.SetPositon(pos, photonView.IsMine);
            cowboy_Script.Init_Cowboy(false);
        }      
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.IsMine)
            transform.SetPositionAndRotation(localPlayerPosition, Quaternion.identity);
        else
            transform.SetPositionAndRotation(remotePlayerPosition, Quaternion.Euler(0f, 0f, 180f));

        Debug.LogError("Player Instantiate");
    }

    [PunRPC]
    public void AttackMotion(int attackPosIndex)
    {
        if (photonView.IsMine)
            StartCoroutine(AttackMotionCoroutine(gameManager.RemoteBattleZone.areas[attackPosIndex]));
        else
            StartCoroutine(AttackMotionCoroutine(gameManager.LocalBattleZone.areas[attackPosIndex]));
    }

    public IEnumerator AttackMotionCoroutine(GameObject battleZone_area)
    {
        Vector3 attackPosition = battleZone_area.transform.position;
        IsAttackMotion = false;

        player_objects[(int)Object_Type.Aim].SetActive(false);
        battleZone_area.SetActive(true);

        yield return StartCoroutine(cowboy_Script.OnAni("Attack", 0.01f));   // CowboyÀÇ Attack Ani

        yield return StartCoroutine(bullet_Script.MoveBullet(attackPosition));

        //battleZone_area.SetActive(false);

        if (photonView.IsMine)
            yield return StartCoroutine(cameraManager.WaveMotionCoroutine());

        IsAttackMotion = true;
    }

    public IEnumerator SetBloodEffact()
    {
        if (photonView.IsMine)
        {
            cameraManager.blood.enabled = true;
            yield return new WaitForSeconds(1);
            cameraManager.blood.enabled = false;
        }
    }

    public void SetActivePlayerObj(int index_Obj, bool value) => player_objects[index_Obj].SetActive(value);

    #region PunRPC_Function

    [PunRPC]
    public void SetLocalPositionIndex(int value)
    {
        positionIndex = value;
        Vector3 pos;
        if (photonView.IsMine)
        {
            pos = gameManager.LocalBattleZone.areas[positionIndex].transform.position;

            transform.SetPositionAndRotation(pos, Quaternion.identity);
            HpBar_Script.SetPositon(pos, photonView.IsMine);
        }
    }

    [PunRPC]
    public void SetRemotePositionIndex(int value)
    {
        positionIndex = value;
        Vector3 pos;
        if (!photonView.IsMine)
        {
            pos = gameManager.RemoteBattleZone.areas[positionIndex].transform.position;

            transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, 0f, 180f));
            HpBar_Script.SetPositon(pos, photonView.IsMine);
        }
    }


    #endregion

    public void SetAimObject(int AttackPositionIndex, bool value)
    {
        if (photonView.IsMine)
        {
            player_objects[(int)Object_Type.Aim].SetActive(value);
            player_objects[(int)Object_Type.Aim].transform.position = gameManager.RemoteBattleZone.areas[-AttackPositionIndex + 2].transform.position;
        }
    }

    #region PunRPC

    public int LocalPositionIndex
    {
        get => positionIndex;
        set => photonView.RPC("SetLocalPositionIndex", RpcTarget.AllBuffered, value);
    }

    public int RemotePositionIndex
    {
        get => positionIndex;
        set
        {
            photonView.RPC("SetRemotePositionIndex", RpcTarget.AllBuffered, value);
        }
    }
    int positionIndex;


    [PunRPC]
    void PRC_SetActive_Blood(bool value) => cowboy_Script.blood.gameObject.SetActive(value);

    #endregion

}