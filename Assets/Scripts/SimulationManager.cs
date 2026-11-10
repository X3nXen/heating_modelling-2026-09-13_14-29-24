using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private float timeStep = 1;
    private float nextUpdate = 1;
    [SerializeField]
    private float outsideHeat = 21f;
    private bool isStarted = false;
    private float simulationTime = 0f;

    private BuildingManager buildingManager;
    [SerializeField]
    private SimulationGrid grid;
    [SerializeField]
    private GameObject ui;
    private UIController uiController;

    private List<IPlaceableObject> heaters;
    private List<IPlaceableObject> emitters;
    private List<IPlaceableObject> buildings;

    private AInformationPanelOpener panelOwner;
    void Awake()
    {
        this.uiController = ui.GetComponent<UIController>();
        var heaterPrefabs = ResourceLoader.GetAll("Prefabs/Heaters");
        heaters = new List<IPlaceableObject>();
        emitters = new List<IPlaceableObject>(); //Resources.LoadAll("Prefabs/Emitters/") as APlaceableObject[];
        var buildingPrefabs = ResourceLoader.GetAll("Prefabs/Buildings");
        buildings = new List<IPlaceableObject>();
        foreach (var go in heaterPrefabs)
        {
            if (go != null)
            {
                IPlaceableObject temp = (go as GameObject).GetComponent<IPlaceableObject>();
                temp.Init(OpenInformationPanel);
                heaters.Add(temp);
            }
        }
        foreach(var go in buildingPrefabs)
        {
            if (go != null)
            {
                IPlaceableObject temp = (go as GameObject).GetComponent<IPlaceableObject>();
                temp.Init(OpenInformationPanel);
                buildings.Add(temp);
            }
        }
        this.uiController.Init(heaters, emitters, buildings, new Action<IPlaceableObject>((IPlaceableObject buildingObject) => { this.SetNewBuilding(buildingObject.GetPrefab()); }));
        this.buildingManager = this.gameObject.AddComponent<BuildingManager>();
    }

    void Update()
    {
        if (!isStarted)
            return;

        simulationTime += Time.deltaTime * timeStep;

        ui.GetComponent<UIController>().UpdateTimeDisplay(simulationTime);
        buildingManager.UpdateWallOpacity();
        UpdateInformationPanel();

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
        grid.StartSim(outsideHeat, OpenInformationPanel);
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

    public void OpenInformationPanel(List<InformationFlag> informationFlag, AInformationPanelOpener newOwner)
    {
        if(this.panelOwner != null && this.panelOwner != newOwner)
        {
            this.uiController.ResetInformationPanel();
        }
        this.uiController.OpenInformationPanel(informationFlag);
        this.panelOwner = newOwner;
    }

    public void UpdateInformationPanel()
    {
        if(this.panelOwner != null)
        {
            panelOwner.UpdateInformation(this.uiController.UpdateInformationPanelValue);
        }
    }

    public void SetNewBuilding(GameObject building)
    {
        this.buildingManager.SetBuildingObject(building);
    }
}
