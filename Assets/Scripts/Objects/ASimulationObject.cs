using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * ASimulationObject osztály
 * Minden a szimulációban használt objektum őse. Absztrakciós szint, a kódismétlés elkerülésére.
 */
public abstract class ASimulationObject : MonoBehaviour, IInformationPanelOpener, IPlaceableObject, IBeginDragHandler, IEndDragHandler, IDragHandler, IDragAndDropObject
{
    public List<InformationFlag> informationFlags { get; set; }
    public Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener { get; set; }
    public string displayName { get; set; }
    public Sprite displayImage { get; set; }
    public GameObject prefab { get; set; }
    public LayerMask layerMask { get; set; }
    public bool directional { get; set; }
    public float heightOffset { get; set; }
    public float widthOffset { get; set; }
    protected LayerMask originalLayer;

    protected float diffusivity;
    protected Vector3 itemWidthOffset;
    protected Vector3 itemHeightOffset;

    protected bool canMove = true;

    public Dictionary<InformationFlag, string> editableDefaultValues;


    public abstract void Init(Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> infoPanelOpen);

    public void InitInfoPanel(List<InformationFlag> information, Action<List<InformationFlag>, IInformationPanelOpener, Dictionary<InformationFlag, string>> panelOpener)
    {
        this.informationFlags = information;
        this.panelOpener = panelOpener;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
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
        this.diffusivity = simulationObject.diffusivity;
        this.canMove = simulationObject.canMove;
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

    public float GetDiffusivity()
    {
        return diffusivity;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canMove)
        {
            this.originalLayer = this.gameObject.layer;
            this.gameObject.layer = LayerMask.NameToLayer("Default");
            this.itemHeightOffset = new Vector3(0, this.heightOffset, 0);
            this.itemWidthOffset = new Vector3(this.widthOffset, 0, this.widthOffset);

            UpdateItemPosition(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canMove)
        {
            UpdateItemPosition(eventData);
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    { 
        this.gameObject.layer = this.originalLayer;
    }
    public virtual void UpdateItemPosition(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, this.layerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.normal != Vector3.zero)
            {
                if (this.directional)
                {
                    this.transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
                }
            }

            Vector3 rotatedWidthOffset = this.transform.rotation * itemWidthOffset;
            this.transform.position = hit.point + itemHeightOffset + rotatedWidthOffset;
        }
    }

    public LayerMask GetLayerMask()
    {
        return this.layerMask;
    }

    public bool GetDirectional()
    {
        return this.directional;
    }

    public float GetHeightOffset()
    {
        return this.heightOffset;
    }

    public float GetWidthOffset()
    {
        return this.widthOffset;
    }
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
}
