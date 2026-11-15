using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IInformationPanelOpener : IPointerDownHandler
{
    List<InformationFlag> informationFlags { get; set; }
    Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener { get; set; }
    void InitInfoPanel(List<InformationFlag> information, Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener);
    

    void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater);
}
