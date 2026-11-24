using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Collections.Generic;

public class LoadFromFile : MonoBehaviour
{
    private SimulationManager simulationManager;

    public void Init(SimulationManager simulationManager)
    {
        this.simulationManager = simulationManager;
    }
    public bool OnStart()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        if (args.Length > 1)
        {
            string filePath = args[1];
            if (File.Exists(filePath) && filePath.EndsWith(".preset"))
            {
                LoadSceneState(filePath);
                return true;
            }
        }
        return false;
    }

    void LoadSceneState(string path)
    {
        string json = File.ReadAllText(path);
        SceneStateData stateData = JsonUtility.FromJson<SceneStateData>(json);
        List<ClickDrawingObject> clickDrawingObjects = new List<ClickDrawingObject>();

        foreach (ASimulationObjectPrefab data in stateData.objects)
        {
            /**/
            if (data.objectType != "global")
            {
                if(data.objectType == "lhouse" || data.objectType == "boxhouse")
                {
                    var prefab = ResourceLoader.Get(data.prefabPath);
                    this.simulationManager.SetPreloadedBuilding(prefab as GameObject, data.buildingScale);
                } else
                {
                    Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
                    Vector3 rotation = new Vector3(data.rotation[0], data.rotation[1], data.rotation[2]);
                    Vector3 scale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);
                    GameObject spawned = Instantiate(ResourceLoader.Get(data.prefabPath) as GameObject, position, Quaternion.Euler(rotation));
                    spawned.transform.localScale = scale;
                    ASimulationObject restored = spawned.GetComponent<ASimulationObject>();
                    switch (data.objectType)
                    {
                        case "pipe":
                            ((Pipe)restored).Init(this.simulationManager.OpenInformationPanel);
                            ((Pipe)restored).SetInsulated(data.isInsulated);
                            ((Pipe)restored).SetMaterial((PipeMaterial)data.pipeMaterial);
                            ((Pipe)restored).SetRadiationHeat(data.radiationHeat);
                            break;
                        case "acunit":
                            ((ACUnit)restored).Init(this.simulationManager.OpenInformationPanel);
                            ((ACUnit)restored).SetRadiationHeat(data.radiationHeat);
                            break;
                        case "boiler":
                            ((Boiler)restored).Init(this.simulationManager.OpenInformationPanel);
                            ((Boiler)restored).SetRadiationHeat(data.radiationHeat);
                            break;
                        case "door":
                            ((Door)restored).Init(this.simulationManager.OpenInformationPanel);
                            ((Door)restored).SetMaterial((DoorMaterial)data.doorMaterial);
                            break;
                        case "fireplace":
                            ((Fireplace)restored).Init(this.simulationManager.OpenInformationPanel);
                            ((Fireplace)restored).SetRadiationHeat(data.radiationHeat);
                            break;
                        case "radiator":
                            ((Radiator)restored).Init(this.simulationManager.OpenInformationPanel);
                            ((Radiator)restored).SetRadiationHeat(data.radiationHeat);
                            break;
                        case "window":
                            ((Window)restored).Init(this.simulationManager.OpenInformationPanel);
                            ((Window)restored).SetMaterial((WindowMaterial)data.windowMaterial);
                            break;
                        case "wall":
                            Wall wallComponent = spawned.GetComponent<Wall>();
                            wallComponent.Init(this.simulationManager.OpenInformationPanel);
                            ClickDrawingObject clickDrawingObject = spawned.AddComponent<ClickDrawingObject>();
                            clickDrawingObjects.Add(clickDrawingObject);
                            wallComponent.SetInsulated(data.isInsulated);
                            wallComponent.SetMaterial((WallMaterial)data.wallMaterial);
                            wallComponent.SetCanDelete(data.canDelete);
                            break;
                    }
                }
            }
            else
            {
                this.simulationManager.LoadInsideHeat(data.insideHeat);
                this.simulationManager.LoadOutsideHeat(data.outsideHeat);
            }
        }
        this.simulationManager.SetWallClickDrawingObjects(clickDrawingObjects);
    }
}