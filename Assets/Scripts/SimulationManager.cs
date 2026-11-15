using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private float timeStep = 1;
    private float nextUpdate = 1;
    private bool isStarted = false;
    private float simulationTime = 0f;

    private BuildingManager buildingManager;
    [SerializeField]
    private SimulationGrid grid;
    [SerializeField]
    private GameObject ui;
    private UIController uiController;

    private List<ASimulationObject> heaters;
    private List<ASimulationObject> emitters;
    private List<ASimulationObject> buildings;

    private IInformationPanelOpener panelOwner;

    private float insideHeat = 21;
    private float outsideHeat = 15;
    void Awake()
    {
        this.uiController = ui.GetComponent<UIController>();
        var heaterPrefabs = ResourceLoader.GetAll("Prefabs/Heaters");
        heaters = new List<ASimulationObject>();
        emitters = new List<ASimulationObject>(); //Resources.LoadAll("Prefabs/Emitters/") as APlaceableObject[];
        var buildingPrefabs = ResourceLoader.GetAll("Prefabs/Buildings");
        buildings = new List<ASimulationObject>();
        foreach (var go in heaterPrefabs)
        {
            if (go != null)
            {
                ASimulationObject temp = (go as GameObject).GetComponent<ASimulationObject>();
                temp.Init(OpenInformationPanel);
                heaters.Add(temp);
            }
        }
        foreach(var go in buildingPrefabs)
        {
            if (go != null)
            {
                ASimulationObject temp = (go as GameObject).GetComponent<ASimulationObject>();
                temp.Init(OpenInformationPanel);
                buildings.Add(temp);
            }
        }
        this.uiController.Init(heaters, emitters, buildings, new Action<ASimulationObject>((ASimulationObject buildingObject) => { this.SetNewBuilding(buildingObject.GetPrefab()); }));
        this.uiController.UpdateGlobalInformation(this.insideHeat.ToString(), this.insideHeat.ToString(), this.outsideHeat.ToString());
        this.buildingManager = this.gameObject.AddComponent<BuildingManager>();
        this.buildingManager.SetBuildingObject(buildingPrefabs[0] as GameObject, OpenInformationPanel);
    }

    void Update()
    {
        buildingManager.UpdateWallOpacity();
        UpdateInformationPanel();
        
        if (!isStarted)
            return;

        simulationTime += Time.deltaTime * timeStep;

        this.uiController.UpdateTimeDisplay(simulationTime);
        this.uiController.UpdateGlobalInformation(grid.GetAverageInsideTemperature().ToString(), this.insideHeat.ToString(), this.outsideHeat.ToString());
        if (Time.time >= nextUpdate)
        {
            nextUpdate = (float)Math.Floor(Time.time) + 1;
            grid.RecalculatePoints(timeStep);
        }
    }

    public void ToggleSimulation()
    {
        if (isStarted)
        {
            isStarted = false;
            grid.CleanUp();
           this.uiController.ToggleStartButtonSprite();
            return;
        }
        isStarted = true;
        grid.StartSim(OpenInformationPanel, insideHeat, outsideHeat);
        simulationTime = 0;
        this.uiController.ToggleStartButtonSprite();
        this.uiController.UpdateTimeDisplay(0f);
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
        this.uiController.CycleSpeedButtonSprite();
    }

    public void OpenInformationPanel(List<InformationFlag> informationFlag, IInformationPanelOpener newOwner, Dictionary<InformationFlag, string> editableDefaultValues)
    {
        if(this.panelOwner != null && this.panelOwner != newOwner)
        {
            this.uiController.ResetInformationPanel();
        }
        this.uiController.OpenInformationPanel(informationFlag, editableDefaultValues);
        this.panelOwner = newOwner;
    }

    public void CloseInformationPanel()
    {
        this.uiController.CloseInformationPanel();
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
        this.buildingManager.SetBuildingObject(building, OpenInformationPanel);
    }

    public void SetInsideHeat(float insideHeat)
    {
        this.insideHeat = this.uiController.GetGlobalInformationInsideTemperature();
        this.uiController.UpdateGlobalInformation(this.insideHeat.ToString(), this.insideHeat.ToString(), this.outsideHeat.ToString());
    }

    public void SetOutsideHeat(float outsideHeat)
    {
        this.outsideHeat = this.uiController.GetGlobalInformationOutsideTemperature();
        this.uiController.UpdateGlobalInformation(this.insideHeat.ToString(), this.insideHeat.ToString(), this.outsideHeat.ToString());
    }

    public void SetWallMaterial(int _)
    {
        if(panelOwner != null)
        {
            WallMaterial material = (WallMaterial)this.uiController.GetInformationPanelWallMaterialValue();
            (this.panelOwner as Wall).SetMaterial(material);
        }
    }

    public void SetInsulation(bool _)
    {

        if(panelOwner != null)
        {
            bool result = (this.panelOwner as Wall).SetInsulated();
        }
    }

    public void SetObjectHeat(string _)
    {
        if(panelOwner != null)
        {
            float objectHeat = this.uiController.GetObjectTemperatureInputValue();
            (this.panelOwner as IRadiatingObject).SetRadiationHeat(objectHeat);
        }
    }
}
