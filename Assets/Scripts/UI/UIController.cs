using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private Sprite[] startButtonSprites;
    private Image startButton;
    private bool isStarted = false;

    private Sprite[] speedButtonSprites;
    private Image speedButton;
    private int speedButtonIndex = 0;

    private Sprite[] viewButtonSprites;
    private Image viewButton;

    private Text timeDisplay;

    private GameObject slidingMenu;
    private GameObject heatersScrollableMenu;
    private GameObject emittersScrollableMenu;
    private GameObject buildingsScrollableMenu;

    private InformationPanelController informationPanelController;
    private TextMeshProUGUI globalInformationAverageDisplay;
    private TMP_InputField globalInformationOutsideTempInput;
    private TMP_InputField globalInformationInsideTempInput;

    public void Init(
        List<ASimulationObject> heaters,
        List<ASimulationObject> emitters,
        List<ASimulationObject> buildings,
        Action<ASimulationObject> onBuildingClick)
    {
        this.startButton = this.transform.Find("Start stop").Find("Start icon").Find("Icon").gameObject.GetComponent<Image>();
        this.speedButton = this.transform.Find("Speed control").Find("Fastforward icon").Find("Icon").gameObject.GetComponent<Image>();
        this.timeDisplay = this.transform.Find("Time Display").Find("Time").gameObject.GetComponent<Text>();
        this.viewButton = this.transform.Find("Grid view button").Find("Grid view icon").Find("Icon").gameObject.GetComponent<Image>();
        startButtonSprites = new Sprite[2];
        startButtonSprites[0] = ResourceLoader.Get("Images/Pause_icon") as Sprite;
        startButtonSprites[1] = ResourceLoader.Get("Images/Play_icon") as Sprite;

        speedButtonSprites = new Sprite[3];
        speedButtonSprites[0] = ResourceLoader.Get("Images/Bar-2") as Sprite;
        speedButtonSprites[1] = ResourceLoader.Get("Images/Bar-3") as Sprite;
        speedButtonSprites[2] = ResourceLoader.Get("Images/Bar-1") as Sprite;

        viewButtonSprites = new Sprite[2];
        viewButtonSprites[0] = ResourceLoader.Get("Images/Grid_icon") as Sprite;
        viewButtonSprites[1] = ResourceLoader.Get("Images/Graph_icon") as Sprite;

        slidingMenu = this.transform.Find("Sliding menu").gameObject;
        heatersScrollableMenu = this.transform.Find("Heaters scrollable").gameObject;
        emittersScrollableMenu = this.transform.Find("Emitters scrollable").gameObject;
        buildingsScrollableMenu = this.transform.Find("Buildings scrollable").gameObject;

        MenuUIElement slidingMenuElement = slidingMenu.AddComponent<MenuUIElement>();
        slidingMenuElement.Setup(new GameObject[] { heatersScrollableMenu, emittersScrollableMenu, buildingsScrollableMenu });

        MenuUIElement heatersScrollableElement = heatersScrollableMenu.AddComponent<MenuUIElement>();
        GameObject heaterParent = heatersScrollableMenu.transform.Find("View").Find("Content").gameObject;
        heatersScrollableElement.Setup(new GameObject[] { emittersScrollableMenu, buildingsScrollableMenu }, heaters, heaterParent);

        MenuUIElement emittersScrollableElement = emittersScrollableMenu.AddComponent<MenuUIElement>();
        GameObject emitterParent = emittersScrollableMenu.transform.Find("View").Find("Content").gameObject;
        emittersScrollableElement.Setup(new GameObject[] { heatersScrollableMenu, buildingsScrollableMenu }, emitters, emitterParent);

        MenuUIElement buildingsScrollableElement = buildingsScrollableMenu.AddComponent<MenuUIElement>();
        GameObject buildingParent = buildingsScrollableMenu.transform.Find("View").Find("Content").gameObject;
        buildingsScrollableElement.Setup(new GameObject[] { heatersScrollableMenu, emittersScrollableMenu }, buildings, buildingParent, onBuildingClick ,true);

        GameObject informationPanel = this.transform.Find("Information Panel").gameObject;
        this.informationPanelController = informationPanel.AddComponent<InformationPanelController>();
        this.informationPanelController.Init();

        Transform globalInformationPanel = this.transform.Find("Global Information");
        this.globalInformationAverageDisplay = globalInformationPanel.transform.Find("INFORMATION.AVG_TEMPERATURE").Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
        this.globalInformationInsideTempInput = globalInformationPanel.transform.Find("INFORMATION.INSIDE_TEMPERATURE").Find("Value").GetComponent<TMP_InputField>();
        this.globalInformationOutsideTempInput = globalInformationPanel.transform.Find("INFORMATION.OUTSIDE_TEMPERATURE").Find("Value").GetComponent<TMP_InputField>();
    }

    public void OpenInformationPanel(List<InformationFlag> informationFlags, Dictionary<InformationFlag, string> editableDefaultValues)
    {
        this.informationPanelController.ToggleVisibility(informationFlags, editableDefaultValues);
    }

    public void CloseInformationPanel()
    {
        this.informationPanelController.ClosePanel();
    }

    public void ResetInformationPanel()
    {
        this.informationPanelController.ResetObject();
    }

    public int GetInformationPanelWallMaterialValue()
    {
        return this.informationPanelController.GetWallMaterialInputValue();
    }

    public float GetObjectTemperatureInputValue()
    {
        return this.informationPanelController.GetObjectTemperatureInputValue();
    }

    public void UpdateInformationPanelValue(InformationFlag informationFlag, string value)
    {
        this.informationPanelController.UpdateValue(informationFlag, value);
    }

    public void UpdateGlobalInformation(string averageTemperature, string insideTemperature, string outsideTemperature)
    {
        this.globalInformationAverageDisplay.text = averageTemperature + "°C";
        if(this.globalInformationInsideTempInput.text == "" || this.globalInformationInsideTempInput == null)
        {
            this.globalInformationInsideTempInput.text = insideTemperature;
        }
        if (this.globalInformationOutsideTempInput.text == "" || this.globalInformationOutsideTempInput == null)
        {
            this.globalInformationOutsideTempInput.text = outsideTemperature;
        }
    }

    public float GetGlobalInformationInsideTemperature()
    {
        return float.Parse(this.globalInformationInsideTempInput.text);
    }

    public float GetGlobalInformationOutsideTemperature()
    {
        return float.Parse(this.globalInformationOutsideTempInput.text);
    }

    public void CycleSpeedButtonSprite()
    {
        speedButtonIndex = (speedButtonIndex+1) % speedButtonSprites.Length;
        speedButton.sprite = speedButtonSprites[speedButtonIndex];
    }

    public void ToggleStartButtonSprite()
    {
        isStarted = !isStarted;
        startButton.sprite = startButtonSprites[isStarted ? 0 : 1];
    }

    public void UpdateTimeDisplay(float time)
    {
        var ts = TimeSpan.FromSeconds(time);
        timeDisplay.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
    }

    public void ToggleViewDisplay(bool isGridView)
    {
        viewButton.sprite = viewButtonSprites[isGridView ? 1 : 0];
    }

    public void ToggleSlidingMenu()
    {
        slidingMenu.GetComponent<MenuUIElement>().ToggleElement(false);
    }

    public void ToggleHeatersMenu()
    {
        heatersScrollableMenu.GetComponent<MenuUIElement>().ToggleElement(true);
    }

    public void ToggleEmittersMenu()
    {
        emittersScrollableMenu.GetComponent<MenuUIElement>().ToggleElement(true);
    }

    public void ToggleBuildingsMenu()
    {
        buildingsScrollableMenu.GetComponent<MenuUIElement>().ToggleElement(true);
    }
}
