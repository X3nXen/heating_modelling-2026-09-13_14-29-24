using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private int verticalLength = 20;
    [SerializeField]
    private int horizontalLength = 20;
    [SerializeField]
    private int depthLength = 10;
    [SerializeField]
    private float sectionSize = 1f;
    [SerializeField]
    private GameObject pointRepresentation;
    private GridPoint[,,] gridPoints;
    private List<GameObject> pointObjects = new List<GameObject>();

    public void RecalculatePoints()
    {
        for(int i = 0; i < verticalLength; i++)
        {
            for(int j = 0; j < horizontalLength; j++)
            {
                for(int k = 0; k < depthLength; k++)
                {
                    float left = (i - 1) < 0 ? 0 : gridPoints[i - 1, j, k].GetCurrentHeat();
                    float right = (i + 1) >= verticalLength ? 0 : gridPoints[i + 1, j, k].GetCurrentHeat();
                    float behind = (j-1) < 0 ? 0 : gridPoints[i, j-1, k].GetCurrentHeat();
                    float front = (j + 1) >= horizontalLength ? 0 : gridPoints[i, j + 1, k].GetCurrentHeat();
                    float down = (k - 1) < 0 ? 0 : gridPoints[i, j, k-1].GetCurrentHeat();
                    float up = (k + 1) >= depthLength ? 0 : gridPoints[i, j, k + 1].GetCurrentHeat();
                    gridPoints[i, j, k].RecalculateHeat(left, right, up, down, front, behind, sectionSize);
                }
            }
        }
    }

    public void Setup()
    {
        if (sectionSize <= 0)
            return;

        int verticalPointCount = (int)(verticalLength / sectionSize);
        int horizontalPointCount = (int) (depthLength / sectionSize);
        int depthPointCount = (int)(horizontalLength / sectionSize);

        gridPoints = new GridPoint[verticalPointCount, horizontalPointCount, depthPointCount];
        for(int verticalIndex = 0; verticalIndex < verticalPointCount; verticalIndex++)
        {
            for(int horizontalIndex = 0; horizontalIndex < horizontalPointCount; horizontalIndex++)
            {
                for(int depthIndex = 0;  depthIndex < depthPointCount; depthIndex++)
                {
                    float xCoord = verticalIndex * sectionSize;
                    float yCoord = (horizontalIndex * sectionSize);
                    float zCoord = depthIndex * sectionSize;
                    Vector3 pointPosition = new Vector3(xCoord, yCoord, zCoord);

                    GameObject newPoint = new GameObject($"Gridpoint_{xCoord}_{yCoord}_{zCoord}");
                    newPoint.transform.SetParent(this.transform);
                    newPoint.transform.localPosition = pointPosition;
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
            gridPoint.SetHeat(outsideHeat);
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
