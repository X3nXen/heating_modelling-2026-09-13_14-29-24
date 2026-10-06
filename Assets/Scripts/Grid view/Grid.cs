using UnityEngine;

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
    void Start()
    {
        Setup();
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

                    GridPoint newGridPoint = newPoint.AddComponent<GridPoint>();
                    newGridPoint.Init(pointRepresentation);

                    gridPoints[verticalIndex, horizontalIndex, depthIndex] = newGridPoint;
                }
            }
        }
    }
}
