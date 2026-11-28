using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Floor : ASimulationObject, IInformationPanelOpener
{
    public override void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener)
    {
        this.editableDefaultValues = new Dictionary<InformationFlag, string>();
        this.canMove = false;
        InitInfoPanel(new List<InformationFlag>() { InformationFlag.OBJECT_NAME, InformationFlag.RESIZE_ACTION }, informationPanelOpener);
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!this.gameObject.GetComponent<ClickDrawingObject>().GetIsDrawing())
        {
            if (informationFlags.Count > 0 && panelOpener != null)
            {
                panelOpener(informationFlags, this, this.editableDefaultValues);
            }
        }
    }

    public override void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater)
    {
        panelInfoUpdater(InformationFlag.OBJECT_NAME, "Épület");
    }

}
