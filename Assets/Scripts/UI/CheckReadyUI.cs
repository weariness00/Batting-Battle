using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckReadyUI : MonoBehaviour
{
    public Image[] images;
    public Sprite[] sprites;

    private void Awake()
    {
        foreach (var item in images)
        {
            item.sprite = sprites[0];
        }
    }

    public void SetReadyUI(int PlayerNumber, bool value)
    {
        if (value)
            images[PlayerNumber].sprite = sprites[1];   // 준비 완료
        else
            images[PlayerNumber].sprite = sprites[0];
    }
}
