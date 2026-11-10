using System;
using System.Collections.Generic;
using System.Linq;
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

    private Text timeDisplay;

    private GameObject slidingMenu;
    private GameObject heatersScrollableMenu;
    private GameObject emittersScrollableMenu;
    private GameObject buildingsScrollableMenu;

    private InformationPanelController informationPanelController;

    public void Init(
        List<IPlaceableObject> heaters,
        List<IPlaceableObject> emitters,
        List<IPlaceableObject> buildings,
        Action<IPlaceableObject> onBuildingClick)
    {
        this.startButton = this.transform.Find("Start stop").Find("Start icon").Find("Icon").gameObject.GetComponent<Image>();
        this.speedButton = this.transform.Find("Speed control").Find("Fastforward icon").Find("Icon").gameObject.GetComponent<Image>();
        this.timeDisplay = this.transform.Find("Time Display").Find("Time").gameObject.GetComponent<Text>(); ;
        startButtonSprites = new Sprite[2];
        startButtonSprites[0] = ResourceLoader.Get("Images/Pause_icon") as Sprite;
        startButtonSprites[1] = ResourceLoader.Get("Images/Play_icon") as Sprite;

        speedButtonSprites = new Sprite[3];
        speedButtonSprites[0] = ResourceLoader.Get("Images/Bar-2") as Sprite;
        speedButtonSprites[1] = ResourceLoader.Get("Images/Bar-3") as Sprite;
        speedButtonSprites[2] = ResourceLoader.Get("Images/Bar-1") as Sprite;

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
    }

    public void OpenInformationPanel(List<InformationFlag> informationFlags)
    {
        this.informationPanelController.ToggleVisibility(informationFlags);
    }

    public void ResetInformationPanel()
    {
        this.informationPanelController.ResetObject();
    }

    public void UpdateInformationPanelValue(InformationFlag informationFlag, float value)
    {
        Debug.Log("Information Update");
        this.informationPanelController.UpdateValue(informationFlag, value.ToString());
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
