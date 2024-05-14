using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BattleZone : MonoBehaviour
{
    [SerializeField] Vector3 localPlayerPosition;
    [SerializeField] Vector3 remotePlayerPosition;

    [SerializeField] GameObject[] changeSpriteObject; // 게임 시작시 스프라이트를 바꿀 게임 오브젝트
    [SerializeField] Sprite[] sprites;
    SpriteRenderer[] spriteRenderers;

    public GameObject[] areas;

    [HideInInspector] public PhotonView photonView;

    enum ObjectsType
    {
        Left,
        Middle,
        Right,
        Background,
        MaxIndex
    }

    public enum SpritesType
    {
        Basic_Section,
        Attack_Section,
        Aim_Position,
        Position_Section,
        MaxIndex
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        Init_Sprite();

        if (photonView.IsMine)
        {
            transform.SetPositionAndRotation(localPlayerPosition, Quaternion.identity);
            gameManager.LocalBattleZone = this;
        }
        else
        {
            transform.SetPositionAndRotation(remotePlayerPosition, Quaternion.Euler(0f, 0f, 180f));
            gameManager.RemoteBattleZone = this;
        }
    }

    void Init_Sprite()
    {
        spriteRenderers = new SpriteRenderer[changeSpriteObject.Length];
        for (int i = 0; i < changeSpriteObject.Length; i++)
        {
            spriteRenderers[i] = changeSpriteObject[i].GetComponent<SpriteRenderer>();
            spriteRenderers[i].sprite = sprites[(int)SpritesType.Aim_Position];
            changeSpriteObject[i].gameObject.SetActive(false);
        }
        changeSpriteObject[(int)ObjectsType.Background].gameObject.SetActive(true);

        spriteRenderers[(int)ObjectsType.Background].sprite = sprites[(int)SpritesType.Basic_Section];
    }

    public void SetSectionSprite(SpritesType spritesType)
    {
        switch (spritesType)
        {
            case SpritesType.Basic_Section:
                spriteRenderers[(int)ObjectsType.Background].sprite = sprites[(int)SpritesType.Basic_Section];
                break;
            case SpritesType.Attack_Section:
                spriteRenderers[(int)ObjectsType.Background].sprite = sprites[(int)SpritesType.Attack_Section];
                break;
            case SpritesType.Aim_Position:
                for (int i = 0; i < 3; i++)
                    spriteRenderers[i].sprite = sprites[(int)SpritesType.Aim_Position];
                break;
            case SpritesType.Position_Section:
                spriteRenderers[(int)ObjectsType.Background].sprite = sprites[(int)SpritesType.Position_Section];
                break;
            default:
                break;
        }
    }

    #region PunRPC

    // 배틀존 구역 3개를 동시에 다루는 RPC
    [PunRPC]
    public void SetActiveAreas(bool value)
    {
        foreach (var item in areas)
        {
            item.SetActive(value);
        }
    }

    #endregion
}