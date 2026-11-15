using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ASimulationObject : MonoBehaviour, IInformationPanelOpener, IPlaceableObject
{
    public List<InformationFlag> informationFlags { get; set; }
    public Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener { get; set; }
    public string displayName { get; set; }
    public Sprite displayImage { get; set; }
    public GameObject prefab { get; set; }

    public Dictionary<InformationFlag, string> editableDefaultValues;

    public Sprite GetDisplayImage()
    {
        return displayImage;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public abstract void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> infoPanelOpen);

    public void InitInfoPanel(List<InformationFlag> information, Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener)
    {
        this.informationFlags = information;
        this.panelOpener = panelOpener;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (informationFlags.Count > 0 && panelOpener != null)
        {
            panelOpener(informationFlags, this, editableDefaultValues);
        }
    }

    public abstract void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater);

    public virtual void SetEqualTo(ASimulationObject simulationObject)
    {
        this.informationFlags = simulationObject.informationFlags;
        this.panelOpener = simulationObject.panelOpener;
        this.displayImage = simulationObject.displayImage;
        this.displayName = simulationObject.displayName;
        this.prefab = simulationObject.prefab;
        this.editableDefaultValues = simulationObject.editableDefaultValues;
    }
}
