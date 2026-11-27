using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Wall: ASimulationObject
{
    private WallMaterial material;
    private bool isInsulated;
    private bool canDelete = false;

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.isInsulated = false;
        this.material = WallMaterial.BRICK_COMMON;
        this.diffusivity = EnumHelper.GetWallMaterialDiffusivity(material, false);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.displayName = "Fal";
        this.displayImage = ResourceLoader.Get("Images/wall") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Drawobjects/Wall") as GameObject;
        this.layerMask = LayerMask.GetMask("House_layer");
        this.directional = false;
        this.heightOffset = this.prefab.transform.localScale.y / 2 - 1;
        this.widthOffset = 0.5f;

        this.originalScale = this.prefab.transform.localScale;
        this.currentScale = new Vector3(1, 1, 1);
        this.editableDefaultValues.Add(InformationFlag.WALL_MATERIAL, ((int)this.material).ToString());
        this.editableDefaultValues.Add(InformationFlag.WALL_INSULATED, this.isInsulated.ToString());
        List<InformationFlag> informationFlags = new List<InformationFlag>();
        this.canMove = false;
        informationFlags.Add(InformationFlag.DIFFUSIVITY);
        informationFlags.Add(InformationFlag.WALL_MATERIAL);
        informationFlags.Add(InformationFlag.WALL_INSULATED);
        InitInfoPanel(informationFlags, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Épület fal");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!this.gameObject.GetComponent<ClickDrawingObject>().GetIsDrawing())
        {
            if (informationFlags.Count > 0 && panelOpener != null)
            {
                panelOpener(informationFlags, this, editableDefaultValues);
            }
        } 
    }

    public void SetCanDelete(bool canDelete)
    {
        this.canDelete = canDelete;
        this.informationFlags.Add(InformationFlag.DELETE_ACTION);
        this.informationFlags.Add(InformationFlag.RESIZE_ACTION);
        this.canMove = true;
    }

    public override void SetEqualTo(ASimulationObject simulationObject)
    {
        this.informationFlags = simulationObject.informationFlags;
        if (this.canDelete)
        {
            this.informationFlags.Add(InformationFlag.DELETE_ACTION);
            this.informationFlags.Add(InformationFlag.RESIZE_ACTION);
            this.canMove = true;
        }
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.editableDefaultValues = simulationObject.GetEditableValues();
        this.diffusivity = simulationObject.GetDiffusivity();
        this.layerMask = simulationObject.layerMask;
        this.heightOffset = simulationObject.heightOffset;
        this.widthOffset = simulationObject.widthOffset;
        this.directional = simulationObject.directional;

        this.originalScale = simulationObject.GetOriginalScale();
        this.currentScale = simulationObject.GetCurrentScale();
    }

    public WallMaterial GetWallMaterial()
    {
        return this.material;
    }

    public bool GetIsInsulated()
    {
        return this.isInsulated;
    }

    public bool GetCanDelete()
    {
        return this.canDelete;
    }
    public void SetMaterial(WallMaterial material)
    {
        this.material = material;
        this.diffusivity = EnumHelper.GetWallMaterialDiffusivity(material, this.isInsulated);
        if(this.editableDefaultValues != null)
        {
            this.editableDefaultValues[InformationFlag.WALL_MATERIAL] = ((int)this.material).ToString();
        }
    }

    public bool SetInsulated()
    {
        this.isInsulated = !isInsulated;
        this.editableDefaultValues[InformationFlag.WALL_INSULATED] = this.isInsulated.ToString();
        this.diffusivity = EnumHelper.GetWallMaterialDiffusivity(this.material, this.isInsulated);
        return this.isInsulated;
    }

    public void SetInsulated(bool isInsulated)
    {
        this.isInsulated = isInsulated;
        this.editableDefaultValues[InformationFlag.WALL_INSULATED] = this.isInsulated.ToString();
        this.diffusivity = EnumHelper.GetWallMaterialDiffusivity(this.material, this.isInsulated);
    }
}
