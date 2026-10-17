using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private int verticalLength = 20;
    private int verticalPointCount;
    [SerializeField]
    private int horizontalLength = 20;
    private int horizontalPointCount;
    [SerializeField]
    private int depth = 10;
    private int depthPointCount;
    [SerializeField]
    private float sectionSize = 1f;
    [SerializeField]
    private GameObject pointRepresentation;
    private GridPoint[,,] gridPoints;
    private List<GameObject> pointObjects = new List<GameObject>();

    public void RecalculatePoints()
    {
        for(int verticalIndex = 0; verticalIndex < verticalPointCount; verticalIndex++)
        {
            for(int horizontalIndex = 0; horizontalIndex < horizontalPointCount; horizontalIndex++)
            {
                for(int depthIndex = 0; depthIndex < depthPointCount; depthIndex++)
                {
                    float left = (verticalIndex - 1) < 0 ? 0 : gridPoints[verticalIndex - 1, horizontalIndex, depthIndex].GetCurrentHeat();
                    float right = (verticalIndex + 1) >= verticalPointCount ? 0 : gridPoints[verticalIndex + 1, horizontalIndex, depthIndex].GetCurrentHeat();
                    float behind = (horizontalIndex-1) < 0 ? 0 : gridPoints[verticalIndex, horizontalIndex-1, depthIndex].GetCurrentHeat();
                    float front = (horizontalIndex + 1) >= horizontalPointCount ? 0 : gridPoints[verticalIndex, horizontalIndex + 1, depthIndex].GetCurrentHeat();
                    float down = (depthIndex - 1) < 0 ? 0 : gridPoints[verticalIndex, horizontalIndex, depthIndex-1].GetCurrentHeat();
                    float up = (depthIndex + 1) >= depthPointCount ? 0 : gridPoints[verticalIndex, horizontalIndex, depthIndex + 1].GetCurrentHeat();
                    gridPoints[verticalIndex, horizontalIndex, depthIndex].RecalculateHeat(left, right, up, down, front, behind, sectionSize);
                }
            }
        }
    }

    public void Setup()
    {
        if (sectionSize <= 0)
            return;

        this.verticalPointCount = (int)(verticalLength / sectionSize);
        this.horizontalPointCount = (int) (depth / sectionSize);
        this.depthPointCount = (int)(horizontalLength / sectionSize);

        gridPoints = new GridPoint[verticalPointCount, horizontalPointCount, depthPointCount];
        for(int verticalIndex = 0; verticalIndex < verticalPointCount; verticalIndex++)
        {
            for(int horizontalIndex = 0; horizontalIndex < horizontalPointCount; horizontalIndex++)
            {
                for(int depthIndex = 0;  depthIndex < depthPointCount; depthIndex++)
                {
                    float xCoord = verticalIndex * sectionSize;
                    float yCoord = horizontalIndex * sectionSize;
                    float zCoord = depthIndex * sectionSize;
                    Vector3 pointPosition = new Vector3(xCoord, yCoord, zCoord);

                    GameObject newPoint = new GameObject($"Gridpoint_{xCoord}_{yCoord}_{zCoord}");
                    newPoint.transform.SetParent(this.transform);
                    newPoint.transform.localPosition = pointPosition;
                    newPoint.tag = "GridPoint";
                    pointObjects.Add(newPoint);

                    GridPoint newGridPoint = newPoint.AddComponent<GridPoint>();
                    newGridPoint.Init(pointRepresentation);

                    gridPoints[verticalIndex, horizontalIndex, depthIndex] = newGridPoint;
                }
            }
        }
    }

    public void StartSim(float outsideHeat)
    {
        Setup();
        foreach(GridPoint gridPoint in gridPoints)
        {
            gridPoint.SetCurrentHeat(outsideHeat);
        }
    }

    public void CleanUp()
    {
        foreach(GameObject pointObject in pointObjects)
        {
            Destroy(pointObject);
        }
        foreach(GridPoint gridpoint in gridPoints)
        {
            Destroy(gridpoint);
        }
        gridPoints = null;

    }
}
