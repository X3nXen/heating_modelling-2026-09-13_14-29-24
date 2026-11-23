using System;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : ASimulationObject, IRadiatingObject, IDragAndDropObject
{
    public float radiationHeat { get; set; }

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> infoPanelOpen)
    {
        this.displayName = "Kályha";
        this.displayImage = ResourceLoader.Get("Images/fireplace") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Heaters/Fireplace_prefab") as GameObject;
        this.layerMask = LayerMask.GetMask("House_layer");

        this.radiationHeat = 60;
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.editableDefaultValues.Add(InformationFlag.OBJECT_TEMPERATURE, this.radiationHeat.ToString());
        this.directional = false;
        this.heightOffset = this.prefab.transform.localScale.y / 2;
        this.widthOffset = 0f;

        this.originalScale = this.prefab.transform.localScale;
        this.currentScale = new Vector3(1, 1, 1);
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.TEMPERATURE, InformationFlag.OBJECT_TEMPERATURE, InformationFlag.DELETE_ACTION, InformationFlag.RESIZE_ACTION }, infoPanelOpen);
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
        this.radiationHeat = ((Fireplace)simulationObject).radiationHeat;
        this.layerMask = ((Fireplace)simulationObject).layerMask;
        this.editableDefaultValues = new Dictionary<InformationFlag, string>(simulationObject.editableDefaultValues);
        this.heightOffset = ((Fireplace)simulationObject).GetHeightOffset();
        this.originalScale = simulationObject.GetOriginalScale();
        this.currentScale = simulationObject.GetCurrentScale();
    }

    public void SetRadiationHeat(float radiationHeat)
    {
        this.radiationHeat = radiationHeat;
        this.editableDefaultValues[InformationFlag.OBJECT_TEMPERATURE] = this.radiationHeat.ToString();
    }
}
