using System;
using System.Collections.Generic;
using UnityEngine;

public class LHouse: ASimulationObject
{
    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> _)
    {
        this.displayName = "L ház";
        this.displayImage = ResourceLoader.Get("Images/boxhouse") as Sprite;
        this.canMove = false;

        this.prefab = ResourceLoader.Get("Prefabs/Buildings/L_house") as GameObject;
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        return;
    }
}
