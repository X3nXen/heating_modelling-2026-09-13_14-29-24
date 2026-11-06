using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;

public class WallOcclusion : MonoBehaviour
{
    public List<GameObject> walls;
    public Transform cameraPos;
    public float minDistance;
    public GameObject minDistanceWall;
    [SerializeField]
    public Material baseMaterial;
    [SerializeField]
    public Material invisibleMaterial;
    void Start()
    {
        cameraPos = Camera.main.transform;
        
    }

    void Update()
    {
        minDistance = 1000;
        cameraPos = Camera.main.transform;
        foreach(GameObject wall in this.walls)
        {
            float distance = (cameraPos.position - wall.transform.position).magnitude;
            if(distance < minDistance)
            {
                minDistance = distance;
                minDistanceWall = wall;
            }
            wall.GetComponent<MeshRenderer>().material = baseMaterial;
        }
        minDistanceWall.GetComponent<MeshRenderer>().material = invisibleMaterial;
    }
}
