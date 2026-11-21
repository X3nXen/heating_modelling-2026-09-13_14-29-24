using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 * BuildingManager osztály
 * Magától nem létezik, a SimulationManager hozza létre.
 * Az osztály felelős az összes épületeket érintő műveletért
 * 
*/
public class BuildingManager: MonoBehaviour
{
    private List<GameObject> walls;
    private GameObject minDistanceWall;
    private Material baseMaterial;
    private Material invisibleMaterial;
    private GameObject buildingObject;
    
    private List<GameObject> xLengthWalls;
    private Dictionary<GameObject, Vector3> wallOriginalPositions;
    private Dictionary<GameObject, Vector3> wallOriginalScales;

    private Vector3 placePosition = new Vector3(15f, 1.5f, 15f);
    private Transform cameraPos;

    private GameObject floorObject;
    private ClickDrawingObject floorClickDrawingObject;
    private List<ClickDrawingObject> wallClickDrawingObjects;
    private Vector3 initialScale;
    private Vector3 initialCenter;
    
    private GameObject ceiling;
    private Vector3 ceilingOriginalPosition;
    
    private GameObject collisionBox;
    private Vector3 collisionBoxOriginalScale;
    private Vector3 collisionBoxOriginalPosition;

    private float xScale;
    private float yScale;
    private float zScale;

    private void Awake()
    {
        cameraPos = Camera.main.transform;
        baseMaterial = ResourceLoader.Get("Materials/BrickWall") as Material;
        invisibleMaterial = ResourceLoader.Get("Materials/Invisible") as Material;
        buildingObject = null;
    }

    /**
     * Beállít egy teljesen új épületet 
     * param: buildingObject - GameObject - egy létrehozott épület prefab ellátva egy ASimulationObject-et kiterjesztő komponenssel
     * param: infoPanelOpen - Action - az InformationPanel megnyitó metódusa
    */
    public void SetBuildingObject(GameObject buildingObject, Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> infoPanelOpen)
    {
        if(this.buildingObject != null)
        {
            Destroy(this.buildingObject);
        }

        xScale = 1f;
        yScale = 1f;
        zScale = 1f;

        this.buildingObject = Instantiate(buildingObject, placePosition, Quaternion.identity);

        this.walls = new List<GameObject>();
        this.floorObject = this.buildingObject.transform.Find("Floor").gameObject;
        this.floorClickDrawingObject = this.floorObject.AddComponent<ClickDrawingObject>();
        this.wallClickDrawingObjects = new List<ClickDrawingObject>();

        Floor floorComponent = this.floorObject.AddComponent<Floor>();
        floorComponent.Init(infoPanelOpen);

        this.xLengthWalls = new List<GameObject>();
        this.wallOriginalPositions = new Dictionary<GameObject, Vector3>();
        this.wallOriginalScales = new Dictionary<GameObject, Vector3>();
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
                    if(child.gameObject.transform.localScale.x > child.gameObject.transform.localScale.z)
                    {
                        this.xLengthWalls.Add(child.gameObject);
                    }
                    this.wallOriginalPositions.Add(child.gameObject, child.gameObject.transform.localPosition + placePosition);
                    this.wallOriginalScales.Add(child.gameObject, child.gameObject.transform.localScale);
                } else
                {
                    wallComponent.SetMaterial(WallMaterial.BRICK_COMMON);
                }
            }
        }

        this.initialCenter = placePosition;
        this.initialScale = this.floorObject.transform.localScale;
        this.ceiling = this.buildingObject.transform.Find("Ceiling").gameObject;
        this.collisionBox = this.buildingObject.transform.Find("House Occlusion Box").gameObject;
        this.collisionBoxOriginalScale = this.collisionBox.transform.localScale;
        this.collisionBoxOriginalPosition = this.collisionBox.transform.position;
        this.ceilingOriginalPosition = this.ceiling.transform.position;

        UpdateWallOpacity();
    }

    /**
     * Update-enként meghívható metódus ami a kamerához legközelebbi falat áttetszővé teszi
     */
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


    /**
     * Az épület X irányú skálázásáért felelős metódus
     * param: x - float: szorzó, ennyiszeresére skálázódik az x irányú kiterjedése az épületnek
     */
    public void ScaleBuildingX(float x)
    {
        this.xScale = x;
        // T1 - padló, tető, collision növelés
        
        this.floorObject.transform.localScale = new Vector3(initialScale.x * x, this.floorObject.transform.localScale.y, this.floorObject.transform.localScale.z);
        this.ceiling.transform.localScale = new Vector3(initialScale.x * x, this.ceiling.transform.localScale.y, this.ceiling.transform.localScale.z);
        this.collisionBox.transform.localScale = new Vector3(initialScale.x * x, this.collisionBox.transform.localScale.y, this.collisionBox.transform.localScale.z);

        // T2 - egész objektum eltolása

        float deltaX = (((initialScale.x * x) - initialScale.x) / 2);
        Vector3 moveVector = new Vector3(deltaX, 0, 0);
        this.buildingObject.transform.position = initialCenter + moveVector + new Vector3(0, 0, this.buildingObject.transform.position.z - initialCenter.z);

        // T3 - falak kitolása, amik z szerint nyúlnak

        foreach (GameObject wall in walls)
        {
            if (!xLengthWalls.Contains(wall))
            {
                //T1 - T2 után számolt koordináta
                Vector3 currentPosition = wallOriginalPositions[wall] + new Vector3(deltaX, 0, 0);

                int direction = (wallOriginalPositions[wall].x > initialCenter.x ? 1 : -1);
                // T3 - fal kitolás
                wall.transform.position = currentPosition + new Vector3(deltaX * direction, wall.transform.position.y - wallOriginalPositions[wall].y, wall.transform.position.z - wallOriginalPositions[wall].z);
            } else
            {
                // T4 - x szerint nyúló falak nyújtása
                wall.transform.localScale = new Vector3(wallOriginalScales[wall].x * x, wall.transform.localScale.y, wall.transform.localScale.z);
            }
        }
    }
    /**
     * Az épület Y irányú skálázásáért felelős metódus
     * param: Y - float: szorzó, ennyiszeresére skálázódik az y irányú kiterjedése az épületnek
     */
    public void ScaleBuildingY(float y)
    {
        this.yScale = y;
        float deltaY = -900;
        foreach (GameObject wall in walls) 
        {
            // T1 - scale Y on walls
            wall.transform.localScale = new Vector3(wall.transform.localScale.x, wallOriginalScales[wall].y * y, wall.transform.localScale.z);
            // T2 - move up by the amount it got scaled
            if(deltaY == -900)
            {
                deltaY = (((wallOriginalScales[wall].y * y) - wallOriginalScales[wall].y) / 2);
            }
            Debug.Log("Y delta: " + deltaY);
            wall.transform.position = new Vector3(wall.transform.position.x, wallOriginalPositions[wall].y + deltaY, wall.transform.position.z);
        }

        // T3 - collision box és ceiling felpozicionálása

        this.collisionBox.transform.localScale = new Vector3(this.collisionBox.transform.localScale.x, this.collisionBoxOriginalScale.y * y, this.collisionBox.transform.localScale.z);
        this.collisionBox.transform.position = new Vector3(this.collisionBox.transform.position.x, this.collisionBoxOriginalPosition.y + deltaY, this.collisionBox.transform.position.z);

        this.ceiling.transform.position = new Vector3(this.ceiling.transform.position.x, this.ceilingOriginalPosition.y + 2 * deltaY, this.ceiling.transform.position.z);
    }

    /**
     * Az épület Z irányú skálázásáért felelős metódus
     * param: Z - float: szorzó, ennyiszeresére skálázódik az Z irányú kiterjedése az épületnek
     */
    public void ScaleBuildingZ(float z)
    {
        this.zScale = z;
        // T1 - padló, tető, collision növelés

        this.floorObject.transform.localScale = new Vector3(this.floorObject.transform.localScale.x, this.floorObject.transform.localScale.y, initialScale.z * z);
        this.ceiling.transform.localScale = new Vector3(this.ceiling.transform.localScale.x, this.ceiling.transform.localScale.y, initialScale.z * z);
        this.collisionBox.transform.localScale = new Vector3(this.collisionBox.transform.localScale.x, this.collisionBox.transform.localScale.y, initialScale.z * z);

        // T2 - egész objektum eltolása

        float deltaZ = (((initialScale.z * z) - initialScale.z) / 2);
        Vector3 moveVector = new Vector3(0, 0, deltaZ);
        this.buildingObject.transform.position = initialCenter + moveVector + new Vector3(this.buildingObject.transform.position.x - initialCenter.x, 0, 0);

        // T3 - falak kitolása, amik z szerint nyúlnak

        foreach (GameObject wall in walls)
        {
            if (xLengthWalls.Contains(wall))
            {
                //T1 - T2 után számolt koordináta
                Vector3 currentPosition = wallOriginalPositions[wall] + new Vector3(0, 0, deltaZ);

                int direction = (wallOriginalPositions[wall].z > initialCenter.z ? 1 : -1);
                // T3 - fal kitolás
                wall.transform.position = currentPosition + new Vector3(wall.transform.position.x - wallOriginalPositions[wall].x, wall.transform.position.y - wallOriginalPositions[wall].y, deltaZ * direction);
            }
            else
            {
                // T4 - x szerint nyúló falak nyújtása
                wall.transform.localScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, wallOriginalScales[wall].z * z);
            }
        }
    }

    public float GetFloorXLength()
    {
        return this.floorObject.transform.localScale.x;
    }

    public float GetFloorZLength()
    {
        return this.floorObject.transform.localScale.z;
    }

    public float GetWallHeight()
    {
        return this.walls[0].transform.localScale.y;
    }

    public float GetXScale()
    {
        return this.xScale;
    }

    public float GetYScale()
    {
        return this.yScale;
    }

    public float GetZScale()
    {
        return this.zScale;
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
}
