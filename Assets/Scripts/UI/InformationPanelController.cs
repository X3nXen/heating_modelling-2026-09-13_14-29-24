using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanelController : MonoBehaviour
{
    private Dictionary<InformationFlag, GameObject> flagToGameobject = new Dictionary<InformationFlag, GameObject>();
    private TMP_Dropdown wallMaterialInput;
    private Toggle insulatedInput;
    private TMP_InputField objectTemperatureInput;
    private TMP_Dropdown doorMaterialInput;
    private TMP_Dropdown windowMaterialInput;
    private TMP_Dropdown pipeMaterialInput;

    public void Init()
    {
        Transform content = this.transform.Find("Content");
        Transform generalContent = content.Find("General");
        Transform editableContent = content.Find("Editable");
        
        GameObject nameObject = this.transform.Find("Object name").gameObject;
        GameObject temperatureObject = generalContent.Find("INFORMATION.TEMPERATURE").gameObject;
        GameObject diffusivityObject = generalContent.Find("INFORMATION.DIFFUSIVITY").gameObject;

        GameObject objectTemperatureObject = editableContent.Find("INFORMATION.OBJECT_TEMPERATURE").gameObject;
        GameObject wallMaterialObject = editableContent.Find("INFORMATION.WALL_MATERIAL").gameObject;
        GameObject insulationObject = editableContent.Find("INFORMATION.WALL_INSULATED").gameObject;
        GameObject doorMaterialObject = editableContent.Find("INFORMATION.DOOR_MATERIAL").gameObject;
        GameObject windowMaterialObject = editableContent.Find("INFORMATION.WINDOW_MATERIAL").gameObject;
        GameObject pipeMaterialObject = editableContent.Find("INFORMATION.PIPE_MATERIAL").gameObject;


        flagToGameobject.Add(InformationFlag.OBJECT_NAME, nameObject);
        flagToGameobject.Add(InformationFlag.TEMPERATURE, temperatureObject);
        flagToGameobject.Add(InformationFlag.DIFFUSIVITY, diffusivityObject);

        flagToGameobject.Add(InformationFlag.OBJECT_TEMPERATURE, objectTemperatureObject);
        flagToGameobject.Add(InformationFlag.WALL_MATERIAL, wallMaterialObject);
        flagToGameobject.Add(InformationFlag.WALL_INSULATED, insulationObject);
        flagToGameobject.Add(InformationFlag.DOOR_MATERIAL, doorMaterialObject);
        flagToGameobject.Add(InformationFlag.WINDOW_MATERIAL, windowMaterialObject);
        flagToGameobject.Add(InformationFlag.PIPE_MATERIAL, pipeMaterialObject);

        List<TMP_Dropdown.OptionData> wallMaterialOptions = new List<TMP_Dropdown.OptionData>();
        foreach(WallMaterial item in Enum.GetValues(typeof(WallMaterial)))
        {
            wallMaterialOptions.Add(new TMP_Dropdown.OptionData(EnumHelper.GetWallMaterialName(item)));
        }
        List<TMP_Dropdown.OptionData> doorMaterialOptions = new List<TMP_Dropdown.OptionData>();
        foreach (DoorMaterials item in Enum.GetValues(typeof(DoorMaterials)))
        {
            doorMaterialOptions.Add(new TMP_Dropdown.OptionData(EnumHelper.GetDoorMaterialName(item)));
        }
        List<TMP_Dropdown.OptionData> windowMaterialOptions = new List<TMP_Dropdown.OptionData>();
        foreach (WindowMaterials item in Enum.GetValues(typeof(WindowMaterials)))
        {
            doorMaterialOptions.Add(new TMP_Dropdown.OptionData(EnumHelper.GetWindowMaterialName(item)));
        }
        List<TMP_Dropdown.OptionData> pipeMaterialOptions = new List<TMP_Dropdown.OptionData>();
        foreach (PipeMaterial item in Enum.GetValues(typeof(PipeMaterial)))
        {
            pipeMaterialOptions.Add(new TMP_Dropdown.OptionData(EnumHelper.GetPipeMaterialName(item)));
        }

        this.wallMaterialInput = wallMaterialObject.transform.Find("Value").gameObject.GetComponent<TMP_Dropdown>();
        this.insulatedInput = insulationObject.transform.Find("Value").gameObject.GetComponent<Toggle>();
        this.objectTemperatureInput = objectTemperatureObject.transform.Find("Value").gameObject.GetComponent<TMP_InputField>();
        this.doorMaterialInput = doorMaterialObject.transform.Find("Value").gameObject.GetComponent<TMP_Dropdown>();
        this.windowMaterialInput = windowMaterialObject.transform.Find("Value").gameObject.GetComponent<TMP_Dropdown>();
        this.pipeMaterialInput = pipeMaterialObject.transform.Find("Value").gameObject.GetComponent<TMP_Dropdown>();

        this.wallMaterialInput.AddOptions(wallMaterialOptions);
        this.doorMaterialInput.AddOptions(doorMaterialOptions);
        this.windowMaterialInput.AddOptions(windowMaterialOptions);
        this.pipeMaterialInput.AddOptions(pipeMaterialOptions);
    }

    public int GetWallMaterialInputValue()
    {
        return this.wallMaterialInput.value;
    }

    public int GetDoorMaterialInputValue()
    {
        return this.doorMaterialInput.value;
    }

    public int GetWindowMaterialInputValue()
    {
        return this.windowMaterialInput.value;
    }
    
    public int GetPipeMaterialInputValue()
    {
        return this.pipeMaterialInput.value;
    }
    public float GetObjectTemperatureInputValue()
    {
        return float.Parse(this.objectTemperatureInput.text);
    }
    public void ToggleVisibility(List<InformationFlag> informationFlags, Dictionary<InformationFlag, string> editableDefaultValues)
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach (InformationFlag flag in flagToGameobject.Keys)
        {
            if (informationFlags.Contains(flag) || flag == InformationFlag.OBJECT_NAME)
            {
                flagToGameobject[flag].SetActive(true);
            } else
            {
                flagToGameobject[flag].SetActive(false);
            }
        }
        if (editableDefaultValues != null)
        {
            foreach (InformationFlag flag in editableDefaultValues.Keys)
            {
                switch (flag)
                {
                    case InformationFlag.WALL_MATERIAL:
                        this.wallMaterialInput.SetValueWithoutNotify(Int32.Parse(editableDefaultValues[flag]));
                        break;
                    case InformationFlag.WALL_INSULATED:
                        this.insulatedInput.SetIsOnWithoutNotify(Boolean.Parse(editableDefaultValues[flag]));
                        break;
                    case InformationFlag.OBJECT_TEMPERATURE:
                        this.objectTemperatureInput.SetTextWithoutNotify(editableDefaultValues[flag]);
                        break;
                    case InformationFlag.DOOR_MATERIAL:
                        this.doorMaterialInput.SetValueWithoutNotify(Int32.Parse(editableDefaultValues[flag]));
                        break;
                    case InformationFlag.WINDOW_MATERIAL:
                        this.wallMaterialInput.SetValueWithoutNotify(Int32.Parse(editableDefaultValues[flag]));
                        break;
                    case InformationFlag.PIPE_MATERIAL:
                        this.pipeMaterialInput.SetValueWithoutNotify(Int32.Parse(editableDefaultValues[flag]));
                        break;
                }
            }
        }
        this.gameObject.SetActive(true);
        }

    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
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
            if(informationFlag == InformationFlag.OBJECT_NAME)
            {
                flagToGameobject[informationFlag].GetComponent<TextMeshProUGUI>().text = value;
            } else
            {
                flagToGameobject[informationFlag].transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>().text = value;
            }
        }
    }

    public void BlockOnSimulationStart(bool isStarted)
    {
        this.wallMaterialInput.interactable = !isStarted;
        this.insulatedInput.interactable = !isStarted;
        this.objectTemperatureInput.interactable = !isStarted;
        this.doorMaterialInput.interactable = !isStarted;
        this.windowMaterialInput.interactable = !isStarted;
        this.pipeMaterialInput.interactable = !isStarted;
    }
}
