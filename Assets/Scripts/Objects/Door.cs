using System;
using System.Collections.Generic;
using UnityEngine;

public class Door: ASimulationObject, IDragAndDropObject
{
    private DoorMaterials material;
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
        this.displayName = "Ajtó";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Fenestration/Door_prefab") as GameObject;
        this.layerMask = LayerMask.GetMask("Wall_layer");

        this.material = DoorMaterials.WOOD;
        this.diffusivity = EnumHelper.GetDoorMaterialDiffusivity(material);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.editableDefaultValues.Add(InformationFlag.DOOR_MATERIAL, ((int)this.material).ToString());
        this.directional = true;
        this.heightOffset = 0f;
        this.widthOffset = 0.5f;
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY, InformationFlag.DOOR_MATERIAL }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Ajtó");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponentInParent<GridPoint>();
                gridPoint.SetDiffusivity(this.diffusivity);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
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
