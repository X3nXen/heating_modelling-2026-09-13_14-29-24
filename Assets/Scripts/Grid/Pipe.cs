using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pipe: ASimulationObject, IRadiatingObject, IDragAndDropObject
{
    private PipeMaterial material;
    public float radiationHeat { get; set; }


    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.material = PipeMaterial.COPPER;
        this.diffusivity = EnumHelper.GetPipeMaterialDiffusivity(material);
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.displayName = "Cső";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;
        this.radiationHeat = 35;

        this.prefab = ResourceLoader.Get("Prefabs/Drawobjects/Pipe") as GameObject;
        this.layerMask = LayerMask.GetMask("Wall_layer");
        this.directional = true;
        this.heightOffset = 0;
        this.widthOffset = 0.5f;
        this.editableDefaultValues.Add(InformationFlag.PIPE_MATERIAL, ((int)this.material).ToString());
        this.editableDefaultValues.Add(InformationFlag.OBJECT_TEMPERATURE, this.radiationHeat.ToString());
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY, InformationFlag.TEMPERATURE, InformationFlag.OBJECT_TEMPERATURE, InformationFlag.PIPE_MATERIAL, InformationFlag.DELETE_ACTION, InformationFlag.RESIZE_ACTION }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Cső");
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity.ToString());
        panelInfoUpdater(InformationFlag.TEMPERATURE, this.radiationHeat.ToString());
    }

    public void SetMaterial(PipeMaterial material)
    {
        this.material = material;
        this.diffusivity = EnumHelper.GetPipeMaterialDiffusivity(material);
        if (this.editableDefaultValues != null)
        {
            this.editableDefaultValues[InformationFlag.PIPE_MATERIAL] = ((int)this.material).ToString();
        }
    }

    public override void SetEqualTo(ASimulationObject simulationObject)
    {
        this.informationFlags = simulationObject.informationFlags;
        this.layerMask = ((IDragAndDropObject)simulationObject).layerMask;
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.editableDefaultValues = simulationObject.editableDefaultValues;
        this.diffusivity = simulationObject.GetDiffusivity();
        this.radiationHeat = ((Pipe)simulationObject).radiationHeat;
        this.heightOffset = ((Pipe)simulationObject).GetHeightOffset();
        this.widthOffset = ((Pipe)simulationObject).GetWidthOffset();
        this.directional = ((Pipe)simulationObject).GetDirectional();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponent<GridPoint>();
                gridPoint.SetDiffusivity(diffusivity);
                gridPoint.SetCurrentHeat(this.radiationHeat);
                gridPoint.SetIsLocked(true);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void SetRadiationHeat(float radiationHeat)
    {
        this.radiationHeat = radiationHeat;
        this.editableDefaultValues[InformationFlag.OBJECT_TEMPERATURE] = this.radiationHeat.ToString();
    }

}
