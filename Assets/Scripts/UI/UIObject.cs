using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIObject: MonoBehaviour
{
    protected Sprite displayImage;
    protected string displayName;

    public virtual void InitUIObject()
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
