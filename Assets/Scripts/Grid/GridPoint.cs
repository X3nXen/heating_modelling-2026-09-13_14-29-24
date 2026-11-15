using System;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : ADiffuseMaterial
{
    public static GameObject CreateGridpointGameobject(GameObject prefab, Vector3 position, SimulationGrid parent)
    {
        Material barelyVisibleMaterial = ResourceLoader.Get("Materials/Invisible") as Material;
        GameObject newPoint = Instantiate(prefab, position, Quaternion.identity);
        newPoint.name = $"Gridpoint_{position.x}_{position.y}_{position.z}";
        newPoint.transform.SetParent(parent.transform);
        newPoint.tag = "GridPoint";
        newPoint.GetComponent<MeshRenderer>().SetMaterials(new List<Material>() { barelyVisibleMaterial });
        newPoint.AddComponent<GridPoint>();
        newPoint.SetActive(true);
        return newPoint;
    }

    [SerializeField]
    private float currentHeat;
    private bool shouldUpdate = true;
    private Material visibleMaterial;
    private float outsideHeat;
    private float insideHeat;
    private bool isInside = false;
    public float RecalculateHeat(float left, float right, float up, float down, float front, float behind, float sectionSize, float timeStep)
    {
        if (!shouldUpdate)
            return 0;
        float heatChange = this.diffusivity * (timeStep / Mathf.Pow(sectionSize, 2)) * (left + right + front + behind + up + down - 6 * this.currentHeat);
        this.currentHeat = Math.Max(currentHeat + heatChange, outsideHeat);
        return currentHeat;
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
            this.isInside = true;
            this.currentHeat = insideHeat;
        }
    }

    public void SetInsideHeat(float insideHeat)
    {
        this.insideHeat = insideHeat;
    }

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.visibleMaterial = ResourceLoader.Get("Materials/Visible") as Material;
        this.diffusivity = 0.000019f;
        this.editableDefaultValues = null;
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.TEMPERATURE, InformationFlag.DIFFUSIVITY }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Rácspont");
        panelInfoUpdater(InformationFlag.TEMPERATURE, this.currentHeat.ToString());
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
    }

    public void SetOutsideHeat(float outsideHeat)
    {
        this.outsideHeat = outsideHeat;
    }

    public float GetOutsideHeat()
    {
        return this.outsideHeat;
    }

    public bool IsInside()
    {
        return this.isInside;
    }
}
