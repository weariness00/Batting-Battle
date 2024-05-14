using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDebug : MonoBehaviour
{
    public bool isTransform = false;
    void Start()
    {
        if(isTransform)
            Debug.LogError(gameObject.name + " = Transform = " + gameObject.GetComponent<RectTransform>().anchoredPosition);
    }

    private void OnEnable()
    {
        if (isTransform)
            Debug.LogError(gameObject.name + " = Transform = " + gameObject.GetComponent<RectTransform>().anchoredPosition);
    }
}
