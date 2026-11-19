using System;
using System.Collections.Generic;
using UnityEngine;

public class Boiler : ASimulationObject, IRadiatingObject, IDragAndDropObject
{
    public float radiationHeat { get ; set; }
    public LayerMask layerMask { get; set; }
    public bool directional { get; set; }

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

    public LayerMask GetLayerMask()
    {
        return layerMask;
    }

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> infoPanelOpen)
    {
        this.displayName = "Kazán";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Heaters/Boiler_prefab") as GameObject;
        this.layerMask = LayerMask.GetMask("House_layer");

        this.radiationHeat = 35;
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.editableDefaultValues.Add(InformationFlag.OBJECT_TEMPERATURE, this.radiationHeat.ToString());
        this.directional = false;
        this.heightOffset = this.prefab.transform.localScale.y / 2;
        this.widthOffset = 0f;
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.TEMPERATURE, InformationFlag.OBJECT_TEMPERATURE }, infoPanelOpen);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponentInParent<GridPoint>();
                gridPoint.SetIsLocked(true);
                gridPoint.SetCurrentHeat(this.radiationHeat);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, this.displayName);
        panelInfoUpdater(InformationFlag.TEMPERATURE, this.radiationHeat.ToString());
    }

    public override void SetEqualTo(ASimulationObject simulationObject)
    {
        this.informationFlags = simulationObject.informationFlags;
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.radiationHeat = ((Boiler)simulationObject).radiationHeat;
        this.layerMask = ((Boiler)simulationObject).layerMask;
        this.editableDefaultValues = new Dictionary<InformationFlag, string>(simulationObject.editableDefaultValues);
    }

    public void SetRadiationHeat(float radiationHeat)
    {
        this.radiationHeat = radiationHeat;
        this.editableDefaultValues[InformationFlag.OBJECT_TEMPERATURE] = this.radiationHeat.ToString();
    }

    public bool GetDirectional()
    {
        return this.directional;
    }
}
