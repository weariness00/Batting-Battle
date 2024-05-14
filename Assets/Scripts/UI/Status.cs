using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    [SerializeField] string displayName;
    
    Text text;
    int value;

    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            text.text = displayName + "\n+" + value.ToString();
        }
    }

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
    }
}
