using System;
using System.Collections.Generic;
using UnityEngine;

public class Wall: ADiffuseMaterial
{
    public override void Init(Action<List<InformationFlag>, AInformationPanelOpener> informationPanelOpener)
    {
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.DIFFUSIVITY }, informationPanelOpener);
    }

    public override void UpdateInformation(Action<InformationFlag, float> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.DIFFUSIVITY, this.diffusivity);
    }

    private void Awake()
    {
        this.diffusivity = 0.00000052f;
    }
}
