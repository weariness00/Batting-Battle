using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AlphaController : MonoBehaviour
{
    public Toggle_Alpha_Controller[] toggles_Alpha;
    public Image_Alpha_Controller[] images_Alpha;
    public Text_Alpha_Controller[] text_Alpha;

    private void Awake()
    {
        foreach (var item in toggles_Alpha)
            item.Init();
        foreach (var item in images_Alpha)
            item.Init();
        foreach (var item in text_Alpha)
            item.Init();
    }

    private void OnEnable()
    {
        OnExpose();
    }

    private void Update()
    {
        foreach (var item in toggles_Alpha)
        {
            item.BeExpose();
            item.BeTransparent();
        }
        foreach (var item in images_Alpha)
        {
            item.BeExpose();
            item.BeTransparent();
        }
        foreach (var item in text_Alpha)
        {
            item.BeExpose();
            item.BeTransparent();
        }
    }

    public void OnExpose()
    {
        foreach (var item in toggles_Alpha)
        {
            item.Init_OnExpose(item.toggle.colors.normalColor);
            item.gameObject.SetActive(true);
        }
        foreach (var item in images_Alpha)
        {
            item.Init_OnExpose(item.image.color);
            item.gameObject.SetActive(true);
        }
        foreach (var item in text_Alpha)
        {
            item.Init_OnExpose(item.text.color);
            item.gameObject.SetActive(true);
        }
    }

    public void OnTransparent()
    {
        foreach (var item in toggles_Alpha)
            item.Init_OnTransparent(item.toggle.colors.normalColor);
        foreach (var item in images_Alpha)
            item.Init_OnTransparent(item.image.color);
        foreach (var item in text_Alpha)
            item.Init_OnTransparent(item.text.color);
    }
}

[Serializable]
public class Toggle_Alpha_Controller : TransAlpha
{
    public Toggle toggle;
    Image background;
    Text text;

    [HideInInspector] public GameObject gameObject;

    public override void Init()
    {
        background = toggle.GetComponentInChildren<Image>();
        text = toggle.GetComponentInChildren<Text>();

        gameObject = toggle.gameObject;
    }

    public void BeExpose()
    {
        if (!isOnExpose)
            return;

        background.color = base.BeExpose();
        text.color = base.BeExpose();
    }

    public void BeTransparent()
    {
        if (!isOnTransparent)
            return;

        background.color = base.BeTransparent();
        text.color = base.BeTransparent();

        if (isOnTransparent == false)
            gameObject.SetActive(false);
    }
}

[Serializable]
public class Image_Alpha_Controller : TransAlpha
{
    public Image image;

    [HideInInspector] public GameObject gameObject;

    public override void Init()
    {
        gameObject = image.gameObject;
    }

    public void BeExpose()
    {
        if (!isOnExpose)
            return;

        image.color = base.BeExpose();
    }

    public void BeTransparent()
    {
        if (!isOnTransparent)
            return;

        image.color = base.BeTransparent();

        if (isOnTransparent == false)
            gameObject.SetActive(false);
    }
}

[Serializable]
public class Text_Alpha_Controller : TransAlpha
{
    public Text text;

    [HideInInspector] public GameObject gameObject;

    public override void Init()
    {
        gameObject = text.gameObject;
    }

    public void BeExpose()
    {
        if (!isOnExpose)
            return;

        text.color = base.BeExpose();
    }

    public void BeTransparent()
    {
        if (!isOnTransparent)
            return;

        text.color = base.BeTransparent();

        if (isOnTransparent == false)
            gameObject.SetActive(false);
    }
}

[Serializable]
public class TransAlpha
{
    Color alhpa;

    [Range(0, 1)]
    [Tooltip("Alpha 감소속도 OR 감소량")] public float decrease_Alpha;
    [Range(0, 1)]
    [Tooltip("Alpha 증가속도 OR 증가량")] public float increase_Alpha;

    [Range(0, 1)]
    public float alphaMinIndex;

    [HideInInspector] public bool isOnExpose;
    [HideInInspector] public bool isOnTransparent;

    public virtual void Init()
    {
        isOnExpose = false;
        isOnTransparent = false;
    }

    public void Init_OnExpose(Color color)
    {
        //alhpa = new Color(1, 1, 1, 0);
        alhpa = color;
        alhpa.a = 0;
        isOnExpose = true;
        isOnTransparent = false;
    }

    public void Init_OnTransparent(Color color)
    {
        //alhpa = new Color(1, 1, 1, 1);
        alhpa = color;
        alhpa.a = 1; 
        isOnExpose = false;
        isOnTransparent = true;
    }

    protected Color BeExpose()
    {
        if (alhpa.a != 1)
        {
            alhpa.a += increase_Alpha;
        }
        else
            isOnExpose = false;

        return alhpa;
    }

    protected Color BeTransparent()
    {
        if (alhpa.a >= alphaMinIndex)
        {
            alhpa.a -= decrease_Alpha;
        }
        else
            isOnTransparent = false;

        return alhpa;
    }
}