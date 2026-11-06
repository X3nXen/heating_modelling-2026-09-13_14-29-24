using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridPoint : ADiffuseMaterial
{
    public static GameObject CreateGridpointGameobject(GameObject prefab, Vector3 position, SimulationGrid parent)
    {
        Material barelyVisibleMaterial = Resources.Load("Materials/Invisible") as Material;
        GameObject newPoint = Instantiate(prefab, position, Quaternion.identity);
        newPoint.name = $"Gridpoint_{position.x}_{position.y}_{position.z}";
        newPoint.transform.SetParent(parent.transform);
        newPoint.tag = "GridPoint";
        newPoint.GetComponent<MeshRenderer>().SetMaterials(new List<Material>() { barelyVisibleMaterial });
        newPoint.AddComponent<GridPoint>();
        newPoint.SetActive(true);
        return newPoint;
    }

    private Vector3 position;
    [SerializeField]
    private float currentHeat;
    private bool shouldUpdate = true;
    private Material visibleMaterial;

    public void Awake()
    {
        this.visibleMaterial = Resources.Load("Materials/Visible") as Material;
        this.diffusivity = 0.000019f;
    }
    public void RecalculateHeat(float left, float right, float up, float down, float front, float behind, float sectionSize, float timeStep)
    {
        if (!shouldUpdate)
            return;
        float heatChange = diffusivity * (timeStep / Mathf.Pow(sectionSize, 2)) * (left + right + front + behind + up + down - 6 * currentHeat);
        currentHeat = currentHeat + heatChange;
    }

    public void SetCurrentHeat(float currentHeat)
    {
        this.currentHeat = currentHeat;
    }

    public float GetCurrentHeat()
    {
        return currentHeat;
    }

    public void SetShouldUpdate(bool shouldUpdate)
    {
        this.shouldUpdate = shouldUpdate;
    }

    public void SetDiffusivity(float diffusivity)
    {
        this.diffusivity = diffusivity;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building")
        {
            this.gameObject.GetComponent<MeshRenderer>().SetMaterials(new List<Material>() { visibleMaterial });
        }
    }
}
