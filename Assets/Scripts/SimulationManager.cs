using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsWorld;

public class SimulationManager : MonoBehaviour
{
    public float timeStep = 1;
    private float nextUpdate = 1;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Text timeDisplay;
    private float outsideHeat = 21f;
    private bool isStarted = false;
    private float simulationTime = 0f;
    void Start()
    {
        
    }

    void Update()
    {
        if (!isStarted)
            return;

        simulationTime += Time.deltaTime * timeStep;

        var ts = TimeSpan.FromSeconds(simulationTime);
        timeDisplay.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

        if (Time.time >= nextUpdate)
        {
            nextUpdate = (float)Math.Floor(Time.time) + 1;
            grid.RecalculatePoints(timeStep);
        }
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
        grid.StartSim(outsideHeat);
        simulationTime = 0;
        timeDisplay.text = string.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);
    }

    public void CycleSimulationSpeed()
    {
        switch (timeStep)
        {
            case 1:
                timeStep = 2;
                break;
            case 2:
                timeStep = 10;
                break;
            case 10:
                timeStep = 1;
                break;
            default:
                break;
        }
    }
}
