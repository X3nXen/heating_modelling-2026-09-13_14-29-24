using System;
using System.Collections.Generic;
using UnityEngine;

public class Wall: ASimulationObject, IDragAndDropObject
{
    private WallMaterial material;
    private bool isInsulated;

    public LayerMask layerMask { get; set; }
    public bool directional { get; set; }
    public float heightOffset { get; set; }
    public float widthOffset { get; set; }

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.isInsulated = false;
        this.material = WallMaterial.BRICK_COMMON;
        this.diffusivity = EnumHelper.GetWallMaterialDiffusivity(material);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.displayName = "Fal";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Drawobjects/Wall") as GameObject;
        this.layerMask = LayerMask.GetMask("House_layer");
        this.directional = false;
        this.heightOffset = 0;
        this.widthOffset = 0;
        this.editableDefaultValues.Add(InformationFlag.WALL_MATERIAL, ((int)this.material).ToString());
        this.editableDefaultValues.Add(InformationFlag.WALL_INSULATED, this.isInsulated.ToString());
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY, InformationFlag.WALL_MATERIAL, InformationFlag.WALL_INSULATED }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Épület fal");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
    }

    public void SetMaterial(WallMaterial material)
    {
        this.material = material;
        this.diffusivity = EnumHelper.GetWallMaterialDiffusivity(material);
        if(this.editableDefaultValues != null)
        {
            this.editableDefaultValues[InformationFlag.WALL_MATERIAL] = ((int)this.material).ToString();
        }
    }

    public bool SetInsulated()
    {
        this.isInsulated = !isInsulated;
        this.editableDefaultValues[InformationFlag.WALL_INSULATED] = this.isInsulated.ToString();
        return this.isInsulated;
    }

    public LayerMask GetLayerMask()
    {
        return this.layerMask;
    }

    public bool GetDirectional()
    {
        return this.directional;
    }

    public float GetHeightOffset()
    {
        return this.heightOffset;
    }

    public float GetWidthOffset()
    {
        return this.widthOffset;
    }
}
