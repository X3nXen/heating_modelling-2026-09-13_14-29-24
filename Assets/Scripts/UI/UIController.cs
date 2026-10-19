using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Sprite[] startButtonSprites;
    public Image startButtonIcon;
    private bool isStarted = false;

    public Sprite[] speedButtonSprites;
    public Image speedButtonIcon;
    private int speedButtonIndex = 0;
    public void CycleSpeedButtonSprite()
    {
        speedButtonIndex = (speedButtonIndex+1) % speedButtonSprites.Length;
        speedButtonIcon.sprite = speedButtonSprites[speedButtonIndex];
    }

    public void ToggleStartButtonSprite()
    {
        isStarted = !isStarted;
        startButtonIcon.sprite = startButtonSprites[isStarted ? 0 : 1];
    }
}
