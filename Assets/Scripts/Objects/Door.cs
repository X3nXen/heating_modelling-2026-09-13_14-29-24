using System;
using System.Collections.Generic;
using UnityEngine;

public class Door: ASimulationObject, IDragAndDropObject
{
    private DoorMaterials material;

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.displayName = "Ajtó";
        this.displayImage = ResourceLoader.Get("Images/door") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Fenestration/Door_prefab") as GameObject;
        this.layerMask = LayerMask.GetMask("Wall_layer");

        this.material = DoorMaterials.WOOD;
        this.diffusivity = EnumHelper.GetDoorMaterialDiffusivity(material);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.editableDefaultValues.Add(InformationFlag.DOOR_MATERIAL, ((int)this.material).ToString());
        this.directional = true;
        this.heightOffset = 0f;
        this.widthOffset = 0.5f;

        this.originalScale = this.prefab.transform.localScale;
        this.currentScale = new Vector3(1, 1, 1);
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY, InformationFlag.DOOR_MATERIAL, InformationFlag.DELETE_ACTION, InformationFlag.RESIZE_ACTION }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Ajtó");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
    }

    public void SetMaterial(DoorMaterials material)
    {
        this.material = material;
        this.diffusivity = EnumHelper.GetDoorMaterialDiffusivity(material);
        if (this.editableDefaultValues != null)
        {
            this.editableDefaultValues[InformationFlag.DOOR_MATERIAL] = ((int)this.material).ToString();
        }
    }

    public override void SetEqualTo(ASimulationObject simulationObject)
    {
        this.diffusivity = ((Door)simulationObject).diffusivity;
        this.informationFlags = simulationObject.informationFlags;
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.layerMask = ((Door)simulationObject).layerMask;
        this.editableDefaultValues = new Dictionary<InformationFlag, string>(simulationObject.GetEditableValues());
        this.originalScale = simulationObject.GetOriginalScale();
        this.currentScale = simulationObject.GetCurrentScale();
    }
}
