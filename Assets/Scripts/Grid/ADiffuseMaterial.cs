using UnityEngine;

public abstract class ADiffuseMaterial: MonoBehaviour
{
    [SerializeField]
    protected float diffusivity;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponent<GridPoint>();
                gridPoint.SetDiffusivity(diffusivity);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
