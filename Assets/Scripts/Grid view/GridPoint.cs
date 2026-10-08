using UnityEngine;
using System.Collections.Generic;

public class GridPoint : MonoBehaviour
{
    private GameObject representation;
    private Vector3 position;
    private float currentHeat;

    public void Init(GameObject prefab)
    {
        if (prefab != null)
        {
            representation = Instantiate(prefab, transform);
            representation.transform.localPosition = Vector3.zero;
            representation.transform.rotation = Quaternion.identity;
            representation.AddComponent<GridPoint>();
            representation.GetComponent<MeshRenderer>().SetMaterials(new System.Collections.Generic.List<Material>());
            representation.gameObject.SetActive(true);
        }
    }

    public void SetHeat(float currentHeat)
    {
        this.currentHeat = currentHeat;
    }
}
