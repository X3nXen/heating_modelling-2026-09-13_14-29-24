using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AInformationPanelOpener : MonoBehaviour, IPointerDownHandler
{
    protected List<InformationFlag> informationFlags = new List<InformationFlag>();
    protected Action<List<InformationFlag>, AInformationPanelOpener> panelOpener = null;
    public void InitInfoPanel(List<InformationFlag> information, Action<List<InformationFlag>, AInformationPanelOpener> panelOpener)
    {
        this.informationFlags = information;
        this.panelOpener = panelOpener;
    }

    public abstract void UpdateInformation(Action<InformationFlag, float> panelInfoUpdater);
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (informationFlags.Count > 0 && panelOpener != null)
        {
            Debug.Log("Click successfully caught! " + this.gameObject.name);
            panelOpener(informationFlags, this);
        }
    }
}
