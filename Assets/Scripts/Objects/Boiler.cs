using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boiler : AInformationPanelOpener, IRadiatingObject, IDragAndDropObject
{
    public float radiationHeat { get ; set; }
    public string displayName { get; set; }
    public Sprite displayImage { get; set; }
    public GameObject prefab { get; set; }
    public LayerMask layerMask { get; set; }

    public Sprite GetDisplayImage()
    {
        return displayImage;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public LayerMask GetLayerMask()
    {
        return this.layerMask;
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public void Init(Action<List<InformationFlag>, AInformationPanelOpener> infoPanelOpen)
    {
        this.displayName = "Kazán";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Heaters/Boiler_prefab") as GameObject;
        this.layerMask = LayerMask.GetMask("House_layer");

        this.radiationHeat = 35;
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.TEMPERATURE }, infoPanelOpen);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponentInParent<GridPoint>();
                gridPoint.SetShouldUpdate(false);
                gridPoint.SetCurrentHeat(this.radiationHeat);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public override void UpdateInformation(Action<InformationFlag, float> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.TEMPERATURE, this.radiationHeat);
    }
}
