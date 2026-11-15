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
                    float changedHeat = gridPoints[verticalIndex, horizontalIndex, depthIndex].RecalculateHeat(left, right, up, down, front, behind, sectionSize, timeStep);
                    if(gridPoints[verticalIndex, horizontalIndex, depthIndex].IsInside())
                    {
                        this.sumForAverage += changedHeat;
                        insidePointCount++;
                    }
                }
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
}
