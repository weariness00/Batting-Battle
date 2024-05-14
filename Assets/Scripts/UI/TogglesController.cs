using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglesController : MonoBehaviour
{
    public Toggle[] toggles;
    [Tooltip("버튼을 누를때 얼마만큼 작아지는지")]public Vector3 diminish;

    bool isOnAni = false;
    bool isOnScale_Diminish = false;
    GameObject toggle_Obj;

    public static bool isOn = false;

    private void Update()
    {
        if(isOnAni)
        {
            OnAni();      
        }
    }

    private void OnDisable()
    {
        isOn = false;

        foreach (var item in toggles)
        {
            item.isOn = false;
            item.gameObject.transform.localScale = Vector3.one;
        }
        isOnAni = false;

        isOn = true;
    }


    public void OnClickButton(int togglesIndex)
    {
        toggle_Obj = toggles[togglesIndex].gameObject;

        isOnAni = true;
        isOnScale_Diminish = true;
    }

    void OnAni()
    {
        if (isOnScale_Diminish)
        {
            toggle_Obj.transform.localScale = diminish;
            isOnScale_Diminish = false;
        }

        Vector3 buttonScale = toggle_Obj.transform.localScale;

        buttonScale += (Vector3.one / 1000);
        toggle_Obj.transform.localScale = buttonScale;

        if (buttonScale == Vector3.one)
        {
            isOnAni = false;
            toggle_Obj.transform.localScale = Vector3.one;
        }
    }
}