using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager: MonoBehaviour
{
    private List<GameObject> walls;
    private Transform cameraPos;
    private GameObject minDistanceWall;

    private Material baseMaterial;
    private Material invisibleMaterial;
    private GameObject buildingObject;
    private Vector3 placePosition = new Vector3(15f, 1.5f, 15f);
    private GameObject floorObject;
    private ClickDrawingObject floorClickDrawingObject;
    private List<ClickDrawingObject> wallClickDrawingObjects;
    private Vector3 initialScale;
    private Vector3 initialCenter;
    private Vector3 dimensions;
    private void Awake()
    {
        cameraPos = Camera.main.transform;
        baseMaterial = ResourceLoader.Get("Materials/BrickWall") as Material;
        invisibleMaterial = ResourceLoader.Get("Materials/Invisible") as Material;
        buildingObject = null;
    }

    public void SetBuildingObject(GameObject buildingObject, Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> infoPanelOpen)
    {
        if(this.buildingObject != null)
        {
            Destroy(this.buildingObject);
        }
        this.buildingObject = Instantiate(buildingObject, placePosition, Quaternion.identity);
        this.walls = new List<GameObject>();
        this.floorObject = this.buildingObject.transform.Find("Floor").gameObject;
        this.floorClickDrawingObject = this.floorObject.AddComponent<ClickDrawingObject>();
        this.wallClickDrawingObjects = new List<ClickDrawingObject>();
        Floor floorComponent = this.floorObject.AddComponent<Floor>();
        floorComponent.Init(infoPanelOpen);
        foreach (Transform child in this.buildingObject.transform)
        {
            if (child.gameObject.tag == "Wall")
            {
                Wall wallComponent = child.gameObject.AddComponent<Wall>();
                if(child.gameObject.layer != LayerMask.NameToLayer("Ceiling"))
                {
                    walls.Add(child.gameObject);
                    wallComponent.Init(infoPanelOpen);
                    ClickDrawingObject clickDrawingObject = child.AddComponent<ClickDrawingObject>();
                    wallClickDrawingObjects.Add(clickDrawingObject);
                } else
                {
                    wallComponent.SetMaterial(WallMaterial.BRICK_COMMON);
                }
            }
        }
        UpdateWallOpacity();
        this.initialCenter = this.buildingObject.transform.position;
        this.initialScale = this.buildingObject.transform.localScale;
        this.dimensions = this.floorObject.transform.localScale;
    }

    public void UpdateWallOpacity()
    {
        cameraPos = Camera.main.transform;
        float minDistance = 1000;
        foreach (GameObject wall in this.walls)
        {
            float distance = (cameraPos.position - wall.transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                minDistanceWall = wall;
            }
            wall.GetComponent<MeshRenderer>().material = baseMaterial;
            wall.layer = LayerMask.NameToLayer("Wall_layer");
        }
        minDistanceWall.GetComponent<MeshRenderer>().material = invisibleMaterial;
        minDistanceWall.layer = LayerMask.NameToLayer("Default");
    }

    public GameObject GetFloorObject()
    {
        return this.floorObject;
    }

    public ClickDrawingObject GetFloorClickDrawingObject()
    {
        return this.floorClickDrawingObject;
    }

    public List<ClickDrawingObject> GetWallClickDrawingObjects()
    {
        return this.wallClickDrawingObjects;
    }

    public void ScaleBuildingX(float x)
    {
        Vector3 currentScale = this.buildingObject.transform.localScale;
        this.buildingObject.transform.localScale = new Vector3(initialScale.x * x, currentScale.y, currentScale.z);
        this.buildingObject.transform.position = new Vector3(initialCenter.x + (dimensions.x * (x-1) / 2) , this.buildingObject.transform.position.y, this.buildingObject.transform.position.z);
    }
    public void ScaleBuildingY(float y)
    {
        Vector3 currentScale = this.buildingObject.transform.localScale;
        this.buildingObject.transform.localScale = new Vector3(currentScale.x, initialScale.y * y, currentScale.z);
    }
    public void ScaleBuildingZ(float z)
    {
        Vector3 currentScale = this.buildingObject.transform.localScale;
        this.buildingObject.transform.localScale = new Vector3(currentScale.x, currentScale.y, initialScale.z * z);
        this.buildingObject.transform.position = new Vector3(this.buildingObject.transform.position.x, this.buildingObject.transform.position.y, initialCenter.z + (dimensions.z * (z-1) / 2));
    }
}
