using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ADiffuseMaterial: MonoBehaviour, IInformationPanelOpener
{
    [SerializeField]
    protected float diffusivity;
    protected Dictionary<InformationFlag, string> editableDefaultValues;

    public List<InformationFlag> informationFlags { get; set; }
    public Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener { get; set; }

    public abstract void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> informationPanelOpener);

    public void InitInfoPanel(List<InformationFlag> information, Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener)
    {
        this.informationFlags = information;
        this.panelOpener = panelOpener;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (informationFlags.Count > 0 && panelOpener != null)
        {
            panelOpener(informationFlags, this, this.editableDefaultValues);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponent<GridPoint>();
                gridPoint.SetDiffusivity(diffusivity);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public abstract void UpdateInformation(Action<InformationFlag, string> panelInfoUpdater);
}
