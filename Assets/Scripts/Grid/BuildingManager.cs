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

    private void Awake()
    {
        cameraPos = Camera.main.transform;
        baseMaterial = ResourceLoader.Get("Materials/BrickWall") as Material;
        invisibleMaterial = ResourceLoader.Get("Materials/Invisible") as Material;
        buildingObject = null;
    }

    public void SetBuildingObject(GameObject buildingObject)
    {
        if(this.buildingObject != null)
        {
            Destroy(this.buildingObject);
        }
        this.buildingObject = Instantiate(buildingObject, placePosition, Quaternion.identity);
        this.walls = new List<GameObject>();
        foreach (Transform child in this.buildingObject.transform)
        {
            if (child.gameObject.tag == "Wall")
            {
                walls.Add(child.gameObject);
                
            }
            if(child.gameObject.tag != "Building")
            {
                child.gameObject.AddComponent<Wall>();
            }
        }
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
        }
        minDistanceWall.GetComponent<MeshRenderer>().material = invisibleMaterial;
    }
}
