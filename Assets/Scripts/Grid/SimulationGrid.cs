using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SimulationGrid : MonoBehaviour
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
    private float sumForAverage = 0;
    private int insidePointCount = 0;
    private float shownAverageTemp;
    

    public void RecalculatePoints(float timeStep)
    {
        this.sumForAverage = 0;
        this.insidePointCount = 0;
        foreach (GridPoint point in gridPoints)
        {
            point.CalculateNextHeat(sectionSize, timeStep);
        }

        foreach (GridPoint point in gridPoints)
        {
            if (point.IsInside())
            {
                this.sumForAverage += point.ApplyNextHeat();
                this.insidePointCount++;
            }
            else
            {
                point.ApplyNextHeat();
            }
        }
        shownAverageTemp = this.sumForAverage / insidePointCount;
    }

    public void Setup(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener, float insideHeat, float outsideHeat)
    {
        if (sectionSize <= 0)
            return;

        this.verticalPointCount = (int)(verticalLength / sectionSize);
        this.horizontalPointCount = (int) (depth / sectionSize);
        this.depthPointCount = (int)(horizontalLength / sectionSize);
        List<GridPoint> gridPointsNeighbours;

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

                    GameObject newGridPoint = GridPoint.CreateGridpointGameobject(pointRepresentation, pointPosition, this);
                    pointObjects.Add(newGridPoint);
                    GridPoint gridPointComponent = newGridPoint.GetComponent<GridPoint>();
                    gridPointComponent.SetCurrentHeat(outsideHeat);
                    gridPointComponent.SetOutsideHeat(outsideHeat);
                    gridPointComponent.SetInsideHeat(insideHeat);
                    gridPointComponent.Init(panelOpener);
                    gridPoints[verticalIndex, horizontalIndex, depthIndex] = gridPointComponent;
                }
            }
        }
        this.sumForAverage = insideHeat;
        this.shownAverageTemp = this.sumForAverage;
        this.insidePointCount = 0;
        for(int verticalIndex = 0; verticalIndex < verticalPointCount; verticalIndex++)
        {
            for(int horizontalIndex = 0; horizontalIndex < horizontalPointCount; horizontalIndex++)
            {
                for (int depthIndex = 0; depthIndex < depthPointCount; depthIndex++)
                {
                    gridPointsNeighbours = new List<GridPoint>();
                    if ((verticalIndex - 1) >= 0)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex - 1, horizontalIndex, depthIndex]);
                    }
                    if((verticalIndex + 1) < verticalPointCount)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex + 1, horizontalIndex, depthIndex]);
                    }
                    if((horizontalIndex - 1) >= 0)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex - 1, depthIndex]);
                    }
                    if((horizontalIndex + 1) < horizontalPointCount)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex + 1, depthIndex]);
                    }
                    if((depthIndex - 1) >= 0)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex, depthIndex - 1]);
                    }
                    if((depthIndex + 1) < depthPointCount)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex, depthIndex + 1]);
                    }
                    gridPoints[verticalIndex, horizontalIndex, depthIndex].SetNeighbors(gridPointsNeighbours);
                }
            }
        }
    }

    public void StartSim(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener, float insideHeat, float outsideHeat)
    {
        Setup(panelOpener, insideHeat, outsideHeat);
    }

    public void CleanUp()
    {
        foreach(GameObject pointObject in this.pointObjects)
        {
            Destroy(pointObject);
        }
        foreach(GridPoint gridpoint in this.gridPoints)
        {
            Destroy(gridpoint);
        }
        this.gridPoints = null;
        this.sumForAverage = 0;
        this.insidePointCount = 0;
    }

    public float GetAverageInsideTemperature()
    {
        return this.shownAverageTemp;
    }

    public void SetView(bool isGridView)
    {
        foreach(GridPoint gridpoint in this.gridPoints)
        {
            gridpoint.SwitchLook(isGridView);
        }
    }
}
