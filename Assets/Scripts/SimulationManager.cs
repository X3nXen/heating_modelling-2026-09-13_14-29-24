using System;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    private float startTime;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Text timeDisplay;
    private float outsideHeat = 21f;
    private bool isStarted = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (!isStarted)
            return;
        var ts = TimeSpan.FromSeconds(Time.time - startTime);
        timeDisplay.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        grid.RecalculatePoints();
    }

    public void SetOutsideHeat(float outsideHeat)
    {
        this.outsideHeat = outsideHeat;
    }

    public void ToggleSimulation()
    {
        if (isStarted)
        {
            isStarted = false;
            grid.CleanUp();
            return;
        }
        isStarted = true;
        startTime = Time.time;
        grid.StartSim(outsideHeat);
        var ts = TimeSpan.FromSeconds(startTime - Time.time);
        timeDisplay.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
    }
}
