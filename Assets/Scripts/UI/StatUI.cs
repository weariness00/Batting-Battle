using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    public string displayName;

    public string color;

    Toggle toggle;
    Text text;
    int value;

    public int Value
    {
        get => value;
        set
        {
            this.value = value;
            toggle.isOn = false;
            text.text = displayName + color + " + " + value.ToString() + "</color>";
        }
    }

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        text = GetComponentInChildren<Text>();
    }
}
