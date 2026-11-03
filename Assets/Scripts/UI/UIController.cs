using System;
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

    private Transform slidingMenu;
    private Transform heatersScrollableMenu;
    private Transform emittersScrollableMenu;

    public void Init(Image startButton, Image speedButton, Text timeDisplay)
    {
        this.startButton = startButton;
        this.speedButton = speedButton;
        this.timeDisplay = timeDisplay;
        startButtonSprites = new Sprite[2];
        startButtonSprites[0] = Resources.Load<Sprite>("Images/Pause_icon");
        startButtonSprites[1] = Resources.Load<Sprite>("Images/Play_icon");

        speedButtonSprites = new Sprite[3];
        speedButtonSprites[0] = Resources.Load<Sprite>("Images/Bar-2");
        speedButtonSprites[1] = Resources.Load<Sprite>("Images/Bar-3");
        speedButtonSprites[2] = Resources.Load<Sprite>("Images/Bar-1");

        slidingMenu = this.transform.Find("Sliding menu");
        heatersScrollableMenu = this.transform.Find("Heaters scrollable");
        emittersScrollableMenu = this.transform.Find("Emitters scrollable");
        slidingMenu.GetComponent<ToggleUIElement>().SetCollapseElements(new Transform[] { heatersScrollableMenu, emittersScrollableMenu });
        heatersScrollableMenu.GetComponent<ToggleUIElement>().SetCollapseElements(new Transform[] { emittersScrollableMenu });
        emittersScrollableMenu.GetComponent<ToggleUIElement>().SetCollapseElements(new Transform[] { heatersScrollableMenu });
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
        slidingMenu.GetComponent<ToggleUIElement>().ToggleElement();
    }

    public void ToggleHeatersMenu()
    {
        heatersScrollableMenu.GetComponent<ToggleUIElement>().ToggleElement();
    }

    public void ToggleEmittersMenu()
    {
        emittersScrollableMenu.GetComponent<ToggleUIElement>().ToggleElement();
    }
}
