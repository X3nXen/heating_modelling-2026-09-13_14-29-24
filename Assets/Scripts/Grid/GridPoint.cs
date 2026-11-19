using System;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : ASimulationObject
{
    public static GameObject CreateGridpointGameobject(GameObject prefab, Vector3 position, SimulationGrid parent)
    {
        Material barelyVisibleMaterial = ResourceLoader.Get("Materials/Invisible") as Material;
        Material gradientMaterial = ResourceLoader.Get("Materials/Gradient") as Material;
        Material visibleMaterial = ResourceLoader.Get("Materials/Visible") as Material;
        GameObject newPoint = Instantiate(prefab, position, Quaternion.identity);
        newPoint.name = $"Gridpoint_{position.x}_{position.y}_{position.z}";
        newPoint.transform.SetParent(parent.transform);
        newPoint.tag = "GridPoint";
        newPoint.GetComponent<MeshRenderer>().SetMaterials(new List<Material>() { barelyVisibleMaterial });
        GridPoint pointComponent = newPoint.AddComponent<GridPoint>();
        pointComponent.SetBarelyVisibleMaterial(barelyVisibleMaterial);
        pointComponent.SetHeatmapMaterial(gradientMaterial);
        pointComponent.SetVisibleMaterial(visibleMaterial);

        newPoint.SetActive(true);
        return newPoint;
    }

    [SerializeField]
    private float currentHeat;
    private float nextHeat;
    private bool isLocked = false;
    private Material visibleMaterial;
    private Material barelyVisibleMaterial;
    private float outsideHeat;
    private float insideHeat;
    private bool isInside = false;
    private List<GridPoint> neighbors = new();

    private Material heatmapMaterial;
    private bool isGridView = true;
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    private Gradient heatGradient;
    private static readonly int ColorPropertyID = Shader.PropertyToID("_BaseColor");

    public void CalculateNextHeat(float sectionSize, float timeStep)
    {
        if (isLocked)
        {
            return;
        }

        float sum = 0f;
        foreach (GridPoint point in neighbors)
        {
            sum += point.GetCurrentHeat();
        }

        float delta = this.diffusivity * (timeStep / (sectionSize * sectionSize)) * (sum - neighbors.Count * currentHeat);
        nextHeat = Mathf.Max(currentHeat + delta, outsideHeat);
    }

    public float ApplyNextHeat()
    {
        currentHeat = nextHeat;
        SwitchLook();
        return currentHeat;
    }

    public void SwitchLook(bool isGridView)
    {
        this.isGridView = isGridView;
        SwitchLook();
    }

    public void SwitchLook()
    {
        if (!isInside)
        {
            meshRenderer.SetSharedMaterials(new List<Material>() { barelyVisibleMaterial });
            meshRenderer.SetPropertyBlock(null);
            return;
        }

        if (!isGridView)
        {
            meshRenderer.SetSharedMaterials(new List<Material>() { heatmapMaterial });

            float normalizedHeat = Mathf.InverseLerp(outsideHeat, insideHeat + 15f, currentHeat);
            Color targetColor = heatGradient.Evaluate(normalizedHeat);

            meshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(ColorPropertyID, targetColor);
            meshRenderer.SetPropertyBlock(propBlock);
        }
        else
        {
            meshRenderer.SetSharedMaterials(new List<Material>() { visibleMaterial });
            meshRenderer.SetPropertyBlock(null);
        }
    }
    public void SetCurrentHeat(float currentHeat)
    {
        this.currentHeat = currentHeat;
        this.nextHeat = currentHeat;
    }

    public float GetCurrentHeat()
    {
        return currentHeat;
    }

    public void SetIsLocked(bool isLocked)
    {
        this.isLocked = isLocked;
    }

    public void SetDiffusivity(float diffusivity)
    {
        if(diffusivity < this.diffusivity)
        {
            this.diffusivity = diffusivity;
        }
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
        this.diffusivity = 0.000019f;
        this.editableDefaultValues = null;
        this.meshRenderer = GetComponent<MeshRenderer>();
        this.propBlock = new MaterialPropertyBlock();

        this.heatGradient = new Gradient();
        this.heatGradient.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(new Color(0.0f, 0.0f, 0.5f), 0.0f),
            new GradientColorKey(new Color(0.0f, 0.7f, 1.0f), 0.25f),
            new GradientColorKey(new Color(1.0f, 0.9f, 0.0f), 0.5f),
            new GradientColorKey(new Color(1.0f, 0.4f, 0.0f), 0.75f),
            new GradientColorKey(new Color(0.6f, 0.0f, 0.0f), 1.0f)
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(1.0f, 0.0f),
            new GradientAlphaKey(1.0f, 1.0f)
            }
        );
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

    public void SetNeighbors(List<GridPoint> neighbors)
    {
        this.neighbors = neighbors;
    }

    public void SetBarelyVisibleMaterial(Material barelyVisibleMaterial)
    {
        this.barelyVisibleMaterial = barelyVisibleMaterial;
    }

    public void SetHeatmapMaterial(Material heatmapMaterial)
    {
        this.heatmapMaterial = heatmapMaterial;
    }

    public void SetVisibleMaterial(Material visibleMaterial)
    {
        this.visibleMaterial = visibleMaterial;
    }
}
