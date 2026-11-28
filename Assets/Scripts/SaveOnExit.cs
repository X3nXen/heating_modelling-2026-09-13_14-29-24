using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveOnExit : MonoBehaviour
{
    private SimulationManager simulationManager;

    public void Init(SimulationManager simulationManager)
    {
        this.simulationManager = simulationManager;
    }
    private void OnApplicationQuit()
    {
        SaveCurrentSceneState();
    }

    private void SaveCurrentSceneState()
    {
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "presets");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = $"state_{System.DateTime.Now:yyyyMMdd_HHmmss}.preset";
        string fullPath = Path.Combine(folderPath, fileName);

        SceneStateData stateData = new SceneStateData();

        ASimulationObject[] allObjects = Object.FindObjectsByType<ASimulationObject>(FindObjectsSortMode.None).Where(obj => obj.transform.parent == null).ToArray();
        foreach (ASimulationObject obj in allObjects)
        {
            ASimulationObjectPrefab data = new ASimulationObjectPrefab();
            data.position = new float[] { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z };
            data.rotation = new float[] { obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z };
            data.scale = new float[] { obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z };
            BuildingManager buildingManager = this.simulationManager.GetBuildingManager();
            switch (obj)
            {
                case Pipe pipe:
                    data.objectType = "pipe";
                    data.prefabPath = "Prefabs/Drawobjects/Pipe";
                    data.radiationHeat = pipe.GetRadiationHeat();
                    break;
                case ACUnit unit:
                    data.objectType = "acunit";
                    data.radiationHeat = unit.radiationHeat;
                    data.prefabPath = "Prefabs/Heaters/ACUnit_prefab";
                    break;
                case Boiler boiler:
                    data.objectType = "boiler";
                    data.radiationHeat = boiler.radiationHeat;
                    data.prefabPath = "Prefabs/Heaters/Boiler_prefab";
                    break;
                case Door door:
                    data.objectType = "door";
                    data.doorMaterial = (int) door.GetDoorMaterial();
                    data.prefabPath = "Prefabs/Fenestration/Door_prefab";
                    break;
                case Fireplace fireplace:
                    data.objectType = "fireplace";
                    data.radiationHeat = fireplace.radiationHeat;
                    data.prefabPath = "Prefabs/Heaters/Fireplace_prefab";
                    break;
                case Radiator radiator:
                    data.objectType = "radiator";
                    data.radiationHeat = radiator.radiationHeat;
                    data.prefabPath = "Prefabs/Heaters/Radiator_prefab";
                    break;
                case Window window:
                    data.objectType = "window";
                    data.windowMaterial = (int) window.GetWindowMaterial();
                    data.prefabPath = "Prefabs/Fenestration/Window_prefab";
                    break;
                case Wall wall:
                    data.objectType = "wall";
                    data.wallMaterial = (int) wall.GetWallMaterial();
                    data.isInsulated = wall.GetIsInsulated();
                    data.canDelete = wall.GetCanDelete();
                    data.prefabPath = "Prefabs/Drawobjects/Wall";
                    break;
                case BoxHouse building_1:
                    data.objectType = "boxhouse";
                    data.prefabPath = "Prefabs/Buildings/Box_house";
                    data.buildingScale = new float[] { buildingManager.GetXScale(), buildingManager.GetYScale(), buildingManager.GetZScale() };
                    break;
                case LHouse lhouse:
                    data.objectType = "lhouse";
                    data.prefabPath = "Prefabs/Buildings/L_house";
                    data.buildingScale = new float[] { buildingManager.GetXScale(), buildingManager.GetYScale(), buildingManager.GetZScale() };
                    break;
            }
            stateData.objects.Add(data);
        }

        ASimulationObjectPrefab globalData = new ASimulationObjectPrefab();
        globalData.objectType = "global";
        globalData.insideHeat = this.simulationManager.GetInsideHeat();
        globalData.outsideHeat = this.simulationManager.GetOutsideHeat();
        stateData.objects.Add(globalData);

        string json = JsonUtility.ToJson(stateData, true);
        File.WriteAllText(fullPath, json);
    }
}

[System.Serializable]
public class SceneStateData
{
    public List<ASimulationObjectPrefab> objects = new List<ASimulationObjectPrefab>();
}