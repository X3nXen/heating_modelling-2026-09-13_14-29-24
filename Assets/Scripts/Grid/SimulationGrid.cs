using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SimulationGrid : MonoBehaviour
{
    private int verticalLength = 31;
    private int scaledVerticalLength = 31;
    private int verticalPointCount;
    private int horizontalLength = 31;
    private int scaledHorizontalLength = 31;
    private int horizontalPointCount;
    private int depth = 10;
    private int scaledDepth = 10;
    private int depthPointCount;
    private float sectionSize = 1f;
    private GameObject pointRepresentation;
    private GridPoint[,,] gridPoints;
    private List<GameObject> pointObjects = new List<GameObject>();
    private float sumForAverage = 0;
    private int insidePointCount = 0;
    private float shownAverageTemp;
    private bool isGridView;
    

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
            if (point.IsInside() && !point.GetIsLocked())
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

    public void StartSim(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener, float insideHeat, float outsideHeat)
    {
        if (sectionSize <= 0)
            return;

        this.verticalPointCount = (int)(scaledVerticalLength / sectionSize);
        this.horizontalPointCount = (int)(scaledDepth / sectionSize);
        this.depthPointCount = (int)(scaledHorizontalLength / sectionSize);
        pointObjects = new List<GameObject>();
        List<GridPoint> gridPointsNeighbours;

        gridPoints = new GridPoint[verticalPointCount, horizontalPointCount, depthPointCount];
        for (int verticalIndex = 0; verticalIndex < verticalPointCount; verticalIndex++)
        {
            float xCoord = verticalIndex * sectionSize;
            for (int horizontalIndex = 0; horizontalIndex < horizontalPointCount; horizontalIndex++)
            {
                float yCoord = horizontalIndex * sectionSize;
                for (int depthIndex = 0; depthIndex < depthPointCount; depthIndex++)
                {
                    float zCoord = depthIndex * sectionSize;
                    Vector3 pointPosition = new Vector3(xCoord, yCoord, zCoord);

                    GridPoint newGridPoint = GridPoint.CreateGridpointGameobject(pointRepresentation, pointPosition, this);
                    pointObjects.Add(newGridPoint.gameObject);
                    newGridPoint.SetCurrentHeat(outsideHeat);
                    newGridPoint.SetOutsideHeat(outsideHeat);
                    newGridPoint.SetInsideHeat(insideHeat);
                    newGridPoint.Init(panelOpener);
                    newGridPoint.SwitchLook(this.isGridView);
                    gridPoints[verticalIndex, horizontalIndex, depthIndex] = newGridPoint;
                }
            }
        }
        this.sumForAverage = insideHeat;
        this.shownAverageTemp = this.sumForAverage;
        this.insidePointCount = 0;
        for (int verticalIndex = 0; verticalIndex < verticalPointCount; verticalIndex++)
        {
            for (int horizontalIndex = 0; horizontalIndex < horizontalPointCount; horizontalIndex++)
            {
                for (int depthIndex = 0; depthIndex < depthPointCount; depthIndex++)
                {
                    gridPointsNeighbours = new List<GridPoint>(6);
                    if ((verticalIndex - 1) >= 0)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex - 1, horizontalIndex, depthIndex]);
                    }
                    if ((verticalIndex + 1) < verticalPointCount)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex + 1, horizontalIndex, depthIndex]);
                    }
                    if ((horizontalIndex - 1) >= 0)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex - 1, depthIndex]);
                    }
                    if ((horizontalIndex + 1) < horizontalPointCount)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex + 1, depthIndex]);
                    }
                    if ((depthIndex - 1) >= 0)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex, depthIndex - 1]);
                    }
                    if ((depthIndex + 1) < depthPointCount)
                    {
                        gridPointsNeighbours.Add(gridPoints[verticalIndex, horizontalIndex, depthIndex + 1]);
                    }
                    gridPoints[verticalIndex, horizontalIndex, depthIndex].SetNeighbors(gridPointsNeighbours);
                }
            }
        }
    }

    public void CleanUp()
    {
        if (pointObjects != null)
        {
            foreach (GameObject pointObject in pointObjects)
            {
                if (pointObject != null)
                {
                    Destroy(pointObject);
                }
            }
            pointObjects.Clear();
            pointObjects = null;
        }

        gridPoints = null;

        sumForAverage = 0f;
        shownAverageTemp = 0f;
        insidePointCount = 0;
        verticalPointCount = 0;
        horizontalPointCount = 0;
        depthPointCount = 0;
    }

    public float GetAverageInsideTemperature()
    {
        return this.shownAverageTemp;
    }

    public void SetView(bool isGridView)
    {
        this.isGridView = isGridView;
        if(this.gridPoints != null)
        {
            foreach(GridPoint gridpoint in this.gridPoints)
            {
                gridpoint.SwitchLook(isGridView);
            }
        }
    }

    public void SetVerticalLength(int length)
    {
        this.scaledVerticalLength = this.verticalLength + (length - 20);
    }

    public void SetHorizontalLength(int length)
    {
        this.scaledHorizontalLength = this.horizontalLength + (length - 20);
    }

    public void SetDepth(int height)
    {
        this.scaledDepth = this.depth + (height - 7);
    }

    public void SetPointRepresentation(GameObject pointRepresentation)
    {
        this.pointRepresentation = pointRepresentation;
    }
}
