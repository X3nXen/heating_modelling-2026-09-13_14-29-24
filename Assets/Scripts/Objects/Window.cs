using System;
using System.Collections.Generic;
using UnityEngine;

public class Window : ASimulationObject, IDragAndDropObject
{
    private WindowMaterials material;
    public bool directional { get; set; }

    public LayerMask layerMask { get; set; }

    public float heightOffset { get; set; }
    public float widthOffset { get; set; }
    public float GetHeightOffset()
    {
        return heightOffset;
    }
    public float GetWidthOffset()
    {
        return widthOffset;
    }

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.displayName = "Ablak";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Fenestration/Window_prefab") as GameObject;
        this.layerMask = LayerMask.GetMask("Wall_layer");

        this.material = WindowMaterials.GLASS;
        this.diffusivity = EnumHelper.GetWindowMaterialDiffusivity(material);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.editableDefaultValues.Add(InformationFlag.WINDOW_MATERIAL, ((int)this.material).ToString());
        this.directional = true;
        this.heightOffset = 0f;
        this.widthOffset = 0.5f;
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY, InformationFlag.WINDOW_MATERIAL }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Ablak");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
    }

    public void SetMaterial(WindowMaterials material)
    {
        this.material = material;
        this.diffusivity = EnumHelper.GetWindowMaterialDiffusivity(material);
        if (this.editableDefaultValues != null)
        {
            this.editableDefaultValues[InformationFlag.WINDOW_MATERIAL] = ((int)this.material).ToString();
        }
    }

    public override void SetEqualTo(ASimulationObject simulationObject)
    {
        this.diffusivity = ((Window)simulationObject).diffusivity;
        this.informationFlags = simulationObject.informationFlags;
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.layerMask = ((Window)simulationObject).layerMask;
        this.editableDefaultValues = new Dictionary<InformationFlag, string>(simulationObject.editableDefaultValues);
    }

    public LayerMask GetLayerMask()
    {
        return this.layerMask;
    }

    public bool GetDirectional()
    {
        return this.directional;
    }
}
