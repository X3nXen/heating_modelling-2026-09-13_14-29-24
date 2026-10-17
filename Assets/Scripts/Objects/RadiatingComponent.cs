using UnityEngine;

public class RadiationPointOcclusion : MonoBehaviour
{
    [SerializeField]
    private RadiatingObject radiatingObject;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponentInParent<GridPoint>();
                gridPoint.SetShouldUpdate(false);
                gridPoint.SetCurrentHeat(radiatingObject.surfaceHeat);
            }
            catch (System.Exception e) 
            { 
                Debug.LogException(e);
            }
        }
    }
}
