using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * SimulationManager osztály
 * A szimuláció kontrollerje, innen ered minden interakció és minden változás
 */
public class SimulationManager : MonoBehaviour
{
    private float timeStep = 1;
    private float nextUpdate = 1;
    private bool isStarted = false;
    private float simulationTime = 0f;
    private float insideHeat = 21;
    private float outsideHeat = 15;

    [SerializeField]
    private SimulationGrid grid;
    [SerializeField]
    private GameObject ui;

    private BuildingManager buildingManager;
    private UIController uiController;
    private CameraMovement cameraController;

    private List<ASimulationObject> heaters;
    private List<ASimulationObject> buildings;
    private List<ASimulationObject> fenestration;
    private List<ASimulationObject> drawingObjects;

    private IInformationPanelOpener panelOwner;

    private bool isGridView = true;
    
    private ClickDrawingObject floorClickDrawingObject;
    
    private List<ClickDrawingObject> wallClickDrawingObjects;
    ClickDrawingObject currentDrawingWall;
    private GameObject preloadedBuilding;
    private Vector3 preloadedBuildingScale;
    
    void Awake()
    {
        LoadFromFile loader = this.gameObject.AddComponent<LoadFromFile>();
        loader.Init(this);
        bool loaded = loader.OnStart();

        SaveOnExit exitComponent = this.gameObject.AddComponent<SaveOnExit>();
        exitComponent.Init(this);
        this.uiController = ui.GetComponent<UIController>();
        this.cameraController = Camera.main.gameObject.GetComponent<CameraMovement>();
        GameObject gridRepresentation = ResourceLoader.Get("Prefabs/GridPoint") as GameObject;
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
        this.buildingManager.SetBuildingObject(loaded ? this.preloadedBuilding : buildingPrefabs[0] as GameObject, OpenInformationPanel);
        if (loaded)
        {
            this.buildingManager.ScaleBuildingX(preloadedBuildingScale.x);
            this.buildingManager.ScaleBuildingY(preloadedBuildingScale.y);
            this.buildingManager.ScaleBuildingZ(preloadedBuildingScale.z);
        }
        this.floorClickDrawingObject = this.buildingManager.GetFloorClickDrawingObject();
        List<ClickDrawingObject> wallClickDrawingObjects = this.buildingManager.GetWallClickDrawingObjects();
        foreach(ClickDrawingObject wallClickDrawingObject in wallClickDrawingObjects)
        {
            this.wallClickDrawingObjects.Add(wallClickDrawingObject);
        }
        
        this.cameraController.SetFreeMovement(false);
        this.grid.SetPointRepresentation(gridRepresentation);
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

    /**
     * A szimuláció elindító gombja által használt metódus.
     * Ha nincs elindítva még a szimuláció, akkor elindítja a szükséges felállási folyamatokat,
     * ha pedig már el van indítva a szimuláció, akkor leállítja a  szükséges tisztító folyamatokkal.
     */
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

    /**
     * A szimuláció sebesség állító gombja által hívott metódus. 3 érték között ciklikusan változtatja a
     * frissítésenként eltelő másodperceket, és a grafikus felületet értesíti a változásról.
     */
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

    /**
     * A nézet állítás gombja által hívott metódus. Ha rács nézeten van, vált a hőmérséklet színezett nézetre,
     * ha pedig azon, akkor vált a rácsra.
     */
    public void ToggleView()
    {
        this.isGridView = !this.isGridView;
        this.uiController.ToggleViewDisplay(this.isGridView);
        this.grid.SetView(isGridView);
    }

    /**
     * A központi, minden objektum által használt metódus ami megnyitja az információs panelt, és beállítja itt
     * azt, hogy ki a "birtokosa" az információs panelnek
     * param - informationFlag: egy listányi InformationFlag, amik megmondják mely információknak kell aktívnak lenni a panelen
     * param - newOwner: a hívó objektum maga
     * param - editableDefaultValues: a módosító inputok InformationFlagjét és értékét tartalmazó map/dictionary
     */
    public void OpenInformationPanel(List<InformationFlag> informationFlag, IInformationPanelOpener newOwner, Dictionary<InformationFlag, string> editableDefaultValues)
    {
        if(this.panelOwner != null && this.panelOwner != newOwner)
        {
            this.uiController.ResetInformationPanel();
        }
        this.uiController.OpenInformationPanel(informationFlag, editableDefaultValues);
        this.panelOwner = newOwner;
        this.uiController.ToggleResizePanel(false);
    }

    /**
     * Az információs panel bezáró gombja által hívott metódus
     */
    public void CloseInformationPanel()
    {
        this.uiController.CloseInformationPanel();
    }

    /**
     * Minden egyes frissítésnél meghívott metódus, ami frissíti az információs panelen az összes értéket az aktuálisra.
     */
    public void UpdateInformationPanel()
    {
        if(this.panelOwner != null)
        {
            panelOwner.UpdateInformation(this.uiController.UpdateInformationPanelValue);
        }
    }

    /**
     * Az épület menüben kattintott épület által hívott metódus, ami beállítja a használt épületet és bezárja a skálázó menüt
     * param - building: maga a kattintott épület
     */
    public void SetNewBuilding(GameObject building)
    {
        if (this.isStarted)
            return;
        this.uiController.ToggleResizePanel(false);
        this.buildingManager.SetBuildingObject(building, OpenInformationPanel);
        this.wallClickDrawingObjects = this.buildingManager.GetWallClickDrawingObjects();
        this.floorClickDrawingObject = this.buildingManager.GetFloorClickDrawingObject();
    }

    /**
     * A rajzoló eszközök által hívott metódus, ami inicializálja a rajzoló komponenst.
     * param - simulationObject: a rajzolni kívánt, ASimulationObject-ből leszármazó objektum
     */
    public void SetUsedTool(ASimulationObject simulationObject)
    {
        if (!isStarted)
        {
            if(simulationObject is Wall)
            {
                floorClickDrawingObject.SetObjectToCopy(simulationObject);
                floorClickDrawingObject.SetObjectToDraw(simulationObject);
                floorClickDrawingObject.SetOnWallAdd((ClickDrawingObject clickDrawingObject) => { this.wallClickDrawingObjects.Add(clickDrawingObject); });
                floorClickDrawingObject.SetIsDrawing(true);
            } 
            else
            {
                foreach(ClickDrawingObject clickDrawingObject in wallClickDrawingObjects)
                {
                    clickDrawingObject.SetObjectToCopy(simulationObject);
                    clickDrawingObject.SetObjectToDraw(simulationObject);
                    clickDrawingObject.SetIsDrawing(true);
                }
            }
        }
    }
    
    /**
     * A skálázó menü által hívott metódus, ami az x komponensű skálázást végzi el, ha az objektum egyszerű (ASimulationObject),
     * illetve az épület esetén az erre hivatott BuildingManagernek adja át a feladatot.
     */
    public void SetObjectXScale()
    {
        if(this.panelOwner != null && !this.isStarted)
        {
            if(this.panelOwner is Floor)
            {
                this.buildingManager.ScaleBuildingX(float.Parse(this.uiController.GetXScaleValue()));
                this.grid.SetVerticalLength((int)this.buildingManager.GetFloorXLength());
            }
            else
            {
                ((ASimulationObject)this.panelOwner).HandleXScale(float.Parse(this.uiController.GetXScaleValue()));
            }
        }
    }

    /**
     * A skálázó menü által hívott metódus, ami az y komponensű skálázást végzi el, ha az objektum egyszerű (ASimulationObject),
     * illetve az épület esetén az erre hivatott BuildingManagernek adja át a feladatot.
     */
    public void SetObjectYScale()
    {
        if (this.panelOwner != null && !this.isStarted)
        {
            if (this.panelOwner is Floor)
            {
                this.buildingManager.ScaleBuildingY(float.Parse(this.uiController.GetYScaleValue()));
                this.grid.SetDepth((int)this.buildingManager.GetWallHeight());
            }
            else
            {
                
                ((ASimulationObject)this.panelOwner).HandleYScale(float.Parse(this.uiController.GetYScaleValue()));
            }
        }
    }

    /**
     * A skálázó menü által hívott metódus, ami az z komponensű skálázást végzi el, ha az objektum egyszerű (ASimulationObject),
     * illetve az épület esetén az erre hivatott BuildingManagernek adja át a feladatot.
     */
    public void SetObjectZScale()
    {
        if (this.panelOwner != null && !this.isStarted)
        {
            if (this.panelOwner is Floor)
            {
                this.buildingManager.ScaleBuildingZ(float.Parse(this.uiController.GetZScaleValue()));
                this.grid.SetHorizontalLength((int)this.buildingManager.GetFloorZLength());
            } else
            {
                ((ASimulationObject)this.panelOwner).HandleZScale(float.Parse(this.uiController.GetZScaleValue()));
            }
        }
    }

    public void LoadInsideHeat(float insideHeat)
    {
        this.insideHeat = insideHeat;
    }

    public void LoadOutsideHeat(float outsideHeat)
    {
        this.outsideHeat = outsideHeat;
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
            DoorMaterial doorMaterial = (DoorMaterial)this.uiController.GetInformationPanelDoorMaterialValue();
            (this.panelOwner as Door).SetMaterial(doorMaterial);
        }
    }

    public void SetWindowMaterial(int _)
    {
        if(panelOwner != null)
        {
            WindowMaterial windowMaterial = (WindowMaterial)this.uiController.GetInformationPanelWindowMaterialValue();
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
            if(this.panelOwner is Wall)
            {
                (this.panelOwner as Wall).SetInsulated();
            }
            else
            {
                (this.panelOwner as Pipe).SetInsulated();
            }
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

    public void DeleteObject()
    {
        if(this.panelOwner != null && !this.isStarted) 
        {
            Destroy(((ASimulationObject)panelOwner).gameObject);
            this.panelOwner = null;
            this.CloseInformationPanel();
        }
    }

    public void OpenResizePanel()
    {
        if(this.panelOwner != null && !this.isStarted) 
        {
            bool isOpen = this.uiController.IsResizePanelOpen();
            this.uiController.ToggleResizePanel(!isOpen);
            if (!isOpen)
            {
                if(this.panelOwner is Floor)
                {
                    this.uiController.UpdateResizePanelValues(this.buildingManager.GetXScale(), this.buildingManager.GetYScale(), this.buildingManager.GetZScale());
                }
                else
                {
                    Vector3 currentScale = ((ASimulationObject)this.panelOwner).GetCurrentScale();
                    this.uiController.UpdateResizePanelValues(currentScale.x, currentScale.y, currentScale.z);
                }
            }
        }
    }

    public BuildingManager GetBuildingManager()
    {
        return this.buildingManager;
    }

    public float GetInsideHeat()
    {
        return this.insideHeat;
    }

    public float GetOutsideHeat()
    {
        return this.outsideHeat;
    }

    public void SetPreloadedBuilding(GameObject gameObject, float[] preloadedScale)
    {
        this.preloadedBuilding = gameObject;
        this.preloadedBuildingScale = new Vector3(preloadedScale[0], preloadedScale[1], preloadedScale[2]);
    }

    public void SetWallClickDrawingObjects(List<ClickDrawingObject> drawingObjects)
    {
        this.wallClickDrawingObjects = new List<ClickDrawingObject>(drawingObjects);
    }
}
