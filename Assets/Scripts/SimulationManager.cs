using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsWorld;

public class SimulationManager : MonoBehaviour
{
    private float timeStep = 1;
    private float nextUpdate = 1;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private float outsideHeat = 21f;

    private bool isStarted = false;
    private float simulationTime = 0f;

    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject speedButton;
    [SerializeField]
    private GameObject timeDisplay;

    void Start()
    {
        ui.GetComponent<UIController>().Init(startButton.GetComponent<Image>(), speedButton.GetComponent<Image>(), timeDisplay.GetComponent<Text>());
    }

    void Update()
    {
        if (!isStarted)
            return;

        simulationTime += Time.deltaTime * timeStep;

        ui.GetComponent<UIController>().UpdateTimeDisplay(simulationTime);

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
            ui.GetComponent<UIController>().ToggleStartButtonSprite();
            return;
        }
        isStarted = true;
        grid.StartSim(outsideHeat);
        simulationTime = 0;
        ui.GetComponent<UIController>().ToggleStartButtonSprite();
        ui.GetComponent<UIController>().UpdateTimeDisplay(0f);
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
        ui.GetComponent<UIController>().CycleSpeedButtonSprite();
    }
}
