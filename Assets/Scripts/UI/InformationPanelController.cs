using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationPanelController : MonoBehaviour
{
    private Dictionary<InformationFlag, GameObject> flagToGameobject = new Dictionary<InformationFlag, GameObject>();

    public void Init()
    {
        Transform content = this.transform.Find("Content");
        GameObject temperatureObject = content.Find("INFORMATION.TEMPERATURE").gameObject;
        GameObject diffusivityObject = content.Find("INFORMATION.DIFFUSIVITY").gameObject;

        flagToGameobject.Add(InformationFlag.TEMPERATURE, temperatureObject);
        flagToGameobject.Add(InformationFlag.DIFFUSIVITY, diffusivityObject);
    }
    public void ToggleVisibility(List<InformationFlag> informationFlags)
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach (InformationFlag flag in flagToGameobject.Keys)
        {
            if (informationFlags.Contains(flag))
            {
                flagToGameobject[flag].SetActive(true);
            } else
            {
                flagToGameobject[flag].SetActive(false);
            }
        }
        this.gameObject.SetActive(true);
    }

    public void ResetObject()
    {
        foreach(InformationFlag flag in flagToGameobject.Keys)
        {
            flagToGameobject[flag].SetActive(false);
        }
        this.gameObject.SetActive(false);
    }

    public void UpdateValue(InformationFlag informationFlag, string value)
    {
        if (flagToGameobject.ContainsKey(informationFlag))
        {
            flagToGameobject[informationFlag].transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>().text = value;
        }
    }
}
