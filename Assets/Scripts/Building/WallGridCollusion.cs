using UnityEngine;

public class WallGridCollusion : MonoBehaviour
{
    private float wallDiffusivity = 0.00000052f;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponentInParent<GridPoint>();
                gridPoint.SetDiffusivity(wallDiffusivity);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
    public void SetWallDiffusivity(float wallDiffusivity)
    {
        this.wallDiffusivity = wallDiffusivity;
    }
}
