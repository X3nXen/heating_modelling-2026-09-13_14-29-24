using System;
using System.Collections.Generic;
using UnityEngine;

public class Building_1: ASimulationObject
{
    public LayerMask layerMask { get; set; }


    public LayerMask GetLayerMask()
    {
        return LayerMask.NameToLayer("Default");
    }

    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> _)
    {
        this.displayName = "Kocka ház";
        this.displayImage = ResourceLoader.Get("Images/Boiler_display_icon") as Sprite;

        this.prefab = ResourceLoader.Get("Prefabs/Buildings/Box_house") as GameObject;
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        return;
    }
}
