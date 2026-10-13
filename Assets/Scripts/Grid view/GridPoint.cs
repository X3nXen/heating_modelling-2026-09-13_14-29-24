using UnityEngine;
using System.Collections.Generic;

public class GridPoint : MonoBehaviour
{
    private GameObject representation;
    private Vector3 position;
    private float diffusivity = 0.000019f;
    [SerializeField]
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

    public void RecalculateHeat(float left, float right, float up, float down, float front, float behind, float sectionSize)
    {
        float heatChange = diffusivity * (1 / Mathf.Pow(sectionSize, 2)) * (left + right + front + behind + up + down - 6 * currentHeat);
        currentHeat = currentHeat + heatChange;
    }

    public void SetHeat(float currentHeat)
    {
        this.currentHeat = currentHeat;
    }

    public float GetCurrentHeat()
    {
        return currentHeat;
    }
}
