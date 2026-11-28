using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/**
 * UIController osztály
 * A UI nevezetű objektumon élő komponens, ami felelős az összes dinamikus felhasználói felület módosításért
 */
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
    private GameObject buildingsScrollableMenu;
    private GameObject fenestrationMenu;
    private GameObject drawingMenu;

    private InformationPanelController informationPanelController;
    private TextMeshProUGUI globalInformationAverageDisplay;
    private TMP_InputField globalInformationOutsideTempInput;
    private TMP_InputField globalInformationInsideTempInput;

    private GameObject resizePanel;
    private TMP_InputField x_scale;
    private TMP_InputField y_scale;
    private TMP_InputField z_scale;

    /**
     * A SimulationManager osztály által hívott metódus, ami inicializálja az összes dinamikus komponenst 
     * param - heaters: az összes felálláskor beolvasott hőfejlesztő prefab
     * param - buildings: az összes felálláskor beolvasott épület prefab
     * param - fenestration: az összes felálláskor beolvasott nyílászáró prefab
     * param - drawing: az összes felálláskor beolvasott rajzoló eszköz 
     * param - onBuildingClick: az épület menün klikkeléskor meghívott metódus
     * param - onToolClick: az eszközök menün klikkeléskor meghívott metódus
     */
    public void Init(
        List<ASimulationObject> heaters,
        List<ASimulationObject> buildings,
        List<ASimulationObject> fenestration,
        List<ASimulationObject> drawing,
        Action<ASimulationObject> onBuildingClick,
        Action<ASimulationObject> onToolClick,
        Action<ASimulationObject> addToObjects)
    {
        this.startButton = this.transform.Find("Start stop").Find("Start icon").Find("Icon").gameObject.GetComponent<Image>();
        this.speedButton = this.transform.Find("Speed control").Find("Fastforward icon").Find("Icon").gameObject.GetComponent<Image>();
        this.timeDisplay = this.transform.Find("Time Display").Find("Time").gameObject.GetComponent<Text>();
        this.viewButton = this.transform.Find("Grid view button").Find("Grid view icon").Find("Icon").gameObject.GetComponent<Image>();
        startButtonSprites = new Sprite[2];
        startButtonSprites[0] = ResourceLoader.Get("Images/Pause_icon") as Sprite;
        startButtonSprites[1] = ResourceLoader.Get("Images/Play_icon") as Sprite;

        speedButtonSprites = new Sprite[3];
        speedButtonSprites[0] = ResourceLoader.Get("Images/Bar-1") as Sprite;
        speedButtonSprites[1] = ResourceLoader.Get("Images/Bar-2") as Sprite;
        speedButtonSprites[2] = ResourceLoader.Get("Images/Bar-3") as Sprite;

        viewButtonSprites = new Sprite[2];
        viewButtonSprites[0] = ResourceLoader.Get("Images/Grid_icon") as Sprite;
        viewButtonSprites[1] = ResourceLoader.Get("Images/Graph_icon") as Sprite;

        slidingMenu = this.transform.Find("Sliding menu").gameObject;
        heatersScrollableMenu = this.transform.Find("Heaters scrollable").gameObject;
        buildingsScrollableMenu = this.transform.Find("Buildings scrollable").gameObject;
        fenestrationMenu = this.transform.Find("Fenestration menu").gameObject;
        drawingMenu = this.transform.Find("Tool menu").gameObject;

        resizePanel = this.transform.Find("Resize panel").gameObject;
        x_scale = resizePanel.transform.Find("SCALE_X").Find("Input").gameObject.GetComponent<TMP_InputField>();
        y_scale = resizePanel.transform.Find("SCALE_Y").Find("Input").gameObject.GetComponent<TMP_InputField>();
        z_scale = resizePanel.transform.Find("SCALE_Z").Find("Input").gameObject.GetComponent<TMP_InputField>();

        MenuUIElement slidingMenuElement = slidingMenu.AddComponent<MenuUIElement>();
        slidingMenuElement.Setup(new GameObject[] { heatersScrollableMenu, buildingsScrollableMenu, fenestrationMenu, drawingMenu });

        MenuUIElement heatersScrollableElement = heatersScrollableMenu.AddComponent<MenuUIElement>();
        GameObject heaterParent = heatersScrollableMenu.transform.Find("View").Find("Content").gameObject;
        heatersScrollableElement.Setup(new GameObject[] { buildingsScrollableMenu, fenestrationMenu, drawingMenu }, heaters, heaterParent, null, false, TurnMenusOff, addToObjects);

        MenuUIElement buildingsScrollableElement = buildingsScrollableMenu.AddComponent<MenuUIElement>();
        GameObject buildingParent = buildingsScrollableMenu.transform.Find("View").Find("Content").gameObject;
        buildingsScrollableElement.Setup(new GameObject[] { heatersScrollableMenu, fenestrationMenu, drawingMenu }, buildings, buildingParent, onBuildingClick ,true, TurnMenusOff);

        MenuUIElement fenestrationElement = fenestrationMenu.AddComponent<MenuUIElement>();
        GameObject fenestrationParent = fenestrationMenu.transform.Find("Content").gameObject;
        fenestrationElement.Setup(new GameObject[] { heatersScrollableMenu, buildingsScrollableMenu, drawingMenu }, fenestration, fenestrationParent, null, false, TurnMenusOff, addToObjects);

        MenuUIElement drawingElement = drawingMenu.AddComponent<MenuUIElement>();
        GameObject drawingParent = drawingElement.transform.Find("Content").gameObject;
        drawingElement.Setup(new GameObject[] {heatersScrollableMenu, buildingsScrollableMenu, fenestrationMenu}, drawing, drawingParent, onToolClick, true, TurnMenusOff, addToObjects);

        GameObject informationPanel = this.transform.Find("Information Panel").gameObject;
        this.informationPanelController = informationPanel.AddComponent<InformationPanelController>();
        this.informationPanelController.Init();

        Transform globalInformationPanel = this.transform.Find("Global Information");
        this.globalInformationAverageDisplay = globalInformationPanel.transform.Find("INFORMATION.AVG_TEMPERATURE").Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
        this.globalInformationInsideTempInput = globalInformationPanel.transform.Find("INFORMATION.INSIDE_TEMPERATURE").Find("Value").GetComponent<TMP_InputField>();
        this.globalInformationOutsideTempInput = globalInformationPanel.transform.Find("INFORMATION.OUTSIDE_TEMPERATURE").Find("Value").GetComponent<TMP_InputField>();
    }

    /**
     * A globális információs panel értékeinek frissítésére szolgáló metódus
     * param - averageTemperature, insideTemperature, outsideTemperature: szöveggé konvertált hőmérsékleti érték (float)
     */
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

    public int GetInformationPanelDoorMaterialValue()
    {
        return this.informationPanelController.GetDoorMaterialInputValue();
    }

    public int GetInformationPanelWindowMaterialValue()
    {
        return this.informationPanelController.GetWindowMaterialInputValue();
    }

    public void UpdateInformationPanelValue(InformationFlag informationFlag, string value)
    {
        this.informationPanelController.UpdateValue(informationFlag, value);
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

    public void ToggleBuildingsMenu()
    {
        buildingsScrollableMenu.GetComponent<MenuUIElement>().ToggleElement(true);
    }

    public void BlockOnSimulationStart(bool isStarted)
    {
        heatersScrollableMenu.GetComponent<MenuUIElement>().BlockDragObjects(isStarted);
        fenestrationMenu.GetComponent<MenuUIElement>().BlockDragObjects(isStarted);
        this.globalInformationInsideTempInput.interactable = !isStarted;
        this.globalInformationOutsideTempInput.interactable = !isStarted;
        this.informationPanelController.BlockOnSimulationStart(isStarted);
    }

    public void ToggleFenestrationMenu()
    {
        fenestrationMenu.GetComponent<MenuUIElement>().ToggleElement(true);
    }

    public void ToggleToolsMenu()
    {
        drawingMenu.GetComponent<MenuUIElement>().ToggleElement(true);
    }

    public void TurnMenusOff()
    {
        slidingMenu.GetComponent<MenuUIElement>().ToggleElement(false);
    }

    public void ToggleResizePanel(bool open)
    {
        this.resizePanel.SetActive(open);
    }

    public bool IsResizePanelOpen()
    {
        return this.resizePanel.activeSelf;
    }

    public void UpdateResizePanelValues(float xScale, float yScale, float zScale)
    {
        this.x_scale.SetTextWithoutNotify(xScale.ToString());
        this.y_scale.SetTextWithoutNotify(yScale.ToString());
        this.z_scale.SetTextWithoutNotify(zScale.ToString());
    }

    public string GetXScaleValue()
    {
        return this.x_scale.text;
    }

    public string GetYScaleValue()
    {
        return this.y_scale.text;
    }

    public string GetZScaleValue()
    {
        return this.z_scale.text;
    }
}
