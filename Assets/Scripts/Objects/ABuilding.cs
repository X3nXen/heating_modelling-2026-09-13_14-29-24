using System;
using UnityEngine;

public abstract class ABuilding : ASimulationObject
{
    private Vector3 referencePoints;
    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        return;
    }


}
