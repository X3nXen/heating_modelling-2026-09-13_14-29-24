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
    private CameraMovement cameraController;

    private List<ASimulationObject> heaters;
    private List<ASimulationObject> buildings;
    private List<ASimulationObject> fenestration;
    private List<ASimulationObject> drawingObjects;

    private IInformationPanelOpener panelOwner;

    private float insideHeat = 21;
    private float outsideHeat = 15;

    private bool isGridView = true;
    private ClickDrawingObject floorClickDrawingObject;
    private List<ClickDrawingObject> wallClickDrawingObjects;
    ClickDrawingObject currentDrawingWall;
    void Awake()
    {
        this.uiController = ui.GetComponent<UIController>();
        this.cameraController = Camera.main.gameObject.GetComponent<CameraMovement>();
        var heaterPrefabs = ResourceLoader.GetAll("Prefabs/Heaters");
        heaters = new List<ASimulationObject>();
        var buildingPrefabs = ResourceLoader.GetAll("Prefabs/Buildings");
        buildings = new List<ASimulationObject>();
        var fenestrationPrefabs = ResourceLoader.GetAll("Prefabs/Fenestration");
        fenestration = new List<ASimulationObject>();
        var drawingPrefabs = ResourceLoader.GetAll("Prefabs/Drawobjects");
        drawingObjects = new List<ASimulationObject>();
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
        foreach(var go in fenestrationPrefabs)
        {
            if (go != null)
            {
                ASimulationObject temp = (go as GameObject).GetComponent<ASimulationObject>();
                temp.Init(OpenInformationPanel);
                fenestration.Add(temp);
            }
        }
        foreach (var go in drawingPrefabs)
        {
            if (go != null)
            {
                ASimulationObject temp = (go as GameObject).GetComponent<ASimulationObject>();
                temp.Init(OpenInformationPanel);
                drawingObjects.Add(temp);
            }
        }
        this.uiController.Init(heaters, buildings, fenestration, drawingObjects, new Action<ASimulationObject>((ASimulationObject buildingObject) => { this.SetNewBuilding(buildingObject.GetPrefab()); }), SetUsedTool);
        this.uiController.UpdateGlobalInformation(this.insideHeat.ToString(), this.insideHeat.ToString(), this.outsideHeat.ToString());
        this.buildingManager = this.gameObject.AddComponent<BuildingManager>();
        this.buildingManager.SetBuildingObject(buildingPrefabs[0] as GameObject, OpenInformationPanel);
        this.floorClickDrawingObject = this.buildingManager.GetFloorClickDrawingObject();
        this.wallClickDrawingObjects = this.buildingManager.GetWallClickDrawingObjects();
        this.cameraController.SetFreeMovement(false);
    }

    void Update()
    {
        buildingManager.UpdateWallOpacity();
        UpdateInformationPanel();
        foreach(ClickDrawingObject clickDrawingObject in this.wallClickDrawingObjects)
        {
            if (clickDrawingObject.GetIsDragging())
            {
                currentDrawingWall = clickDrawingObject;
            }
        }
        if (currentDrawingWall != null && currentDrawingWall.GetIsDragging())
        {
            currentDrawingWall.UpdateSecondPosition();
            foreach(ClickDrawingObject clickDrawingObject in this.wallClickDrawingObjects)
            {
                if(clickDrawingObject != currentDrawingWall && clickDrawingObject.GetIsDrawing())
                {
                    clickDrawingObject.SetIsDrawing(false);
                }
            }
        }
        if (this.floorClickDrawingObject.GetIsDragging())
        {
            this.floorClickDrawingObject.UpdateSecondPosition();
        }
        
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
            this.uiController.BlockOnSimulationStart(isStarted);
            this.uiController.ToggleStartButtonSprite();
            grid.CleanUp();
            return;
        }
        isStarted = true;
        grid.StartSim(OpenInformationPanel, insideHeat, outsideHeat);
        simulationTime = 0;
        this.uiController.ToggleStartButtonSprite();
        this.uiController.UpdateTimeDisplay(0f);
        this.uiController.BlockOnSimulationStart(isStarted);
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

    public void ToggleView()
    {
        this.isGridView = !this.isGridView;
        this.uiController.ToggleViewDisplay(this.isGridView);
        this.grid.SetView(isGridView);
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
        if (this.isStarted)
            return;
        this.buildingManager.SetBuildingObject(building, OpenInformationPanel);
    }

    public void SetUsedTool(ASimulationObject simulationObject)
    {
        if (!isStarted)
        {
            if(simulationObject is Wall)
            {
                floorClickDrawingObject.SetObjectToCopy(simulationObject);
                floorClickDrawingObject.SetObjectToDraw(simulationObject);
                floorClickDrawingObject.SetIsDrawing(true);
            } 
            else
            {
                foreach(ClickDrawingObject clickDrawingObject in wallClickDrawingObjects)
                {
                    clickDrawingObject.SetObjectToCopy(simulationObject);
                    clickDrawingObject.SetObjectToDraw(simulationObject);
                    clickDrawingObject.SetIsDrawing(true);
                    Debug.Log("Drawing again");
                }
            }
        }
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

    public void SetDoorMaterial(int _)
    {
        if(panelOwner != null)
        {
            DoorMaterials doorMaterial = (DoorMaterials)this.uiController.GetInformationPanelDoorMaterialValue();
            (this.panelOwner as Door).SetMaterial(doorMaterial);
        }
    }

    public void SetWindowMaterial(int _)
    {
        if(panelOwner != null)
        {
            WindowMaterials windowMaterial = (WindowMaterials)this.uiController.GetInformationPanelWindowMaterialValue();
            (this.panelOwner as Window).SetMaterial(windowMaterial);
        }
    }

    public void SetPipeMaterial(int _)
    {
        if (panelOwner != null)
        {
            PipeMaterial pipeMaterial = (PipeMaterial)this.uiController.GetInformationPanelPipeMaterialValue();
            (this.panelOwner as Pipe).SetMaterial(pipeMaterial);
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

    public void ToggleFreeCamera()
    {
        bool isFreeMovement = this.cameraController.IsFreeMovement();
        this.cameraController.SetFreeMovement(!isFreeMovement);
    }
}
