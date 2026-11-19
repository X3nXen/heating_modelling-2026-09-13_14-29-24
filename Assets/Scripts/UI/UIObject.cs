using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIObject: MonoBehaviour
{
    protected Sprite displayImage;
    protected string displayName;
    protected Action menuCloser;

    public virtual void InitUIObject(Action menuCloser)
    {
        this.transform.Find("Image").gameObject.GetComponent<Image>().sprite = this.displayImage;
        this.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = this.displayName;
        this.transform.localScale = Vector3.one;
    }

    public void SetDisplayImage(Sprite displayImage)
    {
        this.displayImage = displayImage;
    }

    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}
