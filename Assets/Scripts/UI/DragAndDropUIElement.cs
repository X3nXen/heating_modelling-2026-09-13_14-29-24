using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropUIElement: UIObject, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    protected LayerMask raycastLayerMask;
    protected float maxRaycastDistance = 1000f;
    private Vector3 itemHeightOffset;
    private Vector3 itemWidthOffset;
    protected GameObject dragItem;
    protected GameObject prefab;
    protected ASimulationObject componentToCopy;
    protected bool isStarted;

    public override void InitUIObject(Action menuCloser)
    {
        this.transform.Find("Image").gameObject.GetComponent<Image>().sprite = this.displayImage;
        this.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = this.displayName;
        this.transform.localScale = Vector3.one;
        this.menuCloser = menuCloser;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!isStarted)
        {
            this.menuCloser();
            this.itemHeightOffset = new Vector3(0, ((IDragAndDropObject)this.componentToCopy).GetHeightOffset(), 0);
            this.itemWidthOffset = new Vector3(((IDragAndDropObject)this.componentToCopy).GetWidthOffset(), 0, ((IDragAndDropObject)this.componentToCopy).GetWidthOffset());
            this.dragItem = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
            this.dragItem.SetActive(true);

            UpdateItemPosition(eventData);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (this.dragItem != null)
        {
            UpdateItemPosition(eventData);
        }
    }


    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if(this.dragItem != null)
        {
            this.dragItem.GetComponent<ASimulationObject>().SetEqualTo(componentToCopy);
            this.dragItem = null;
        }
    }
    private void UpdateItemPosition(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, this.maxRaycastDistance, this.raycastLayerMask))
        {
            if (((IDragAndDropObject)componentToCopy).GetDirectional())
            {
                if (hit.normal != Vector3.zero)
                {
                    this.dragItem.transform.rotation = Quaternion.LookRotation(-hit.normal);
                }
            }

            Vector3 rotatedWidthOffset = this.dragItem.transform.rotation * itemWidthOffset;

            this.dragItem.transform.position = hit.point + itemHeightOffset + rotatedWidthOffset;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float distance))
            {
                this.dragItem.transform.position = ray.GetPoint(distance);
            }
        }
    }

    public void SetPrefab(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public void SetLayerMask(LayerMask layerMask)
    {
        this.raycastLayerMask = layerMask;
    }

    public void SetComponentToCopy(ASimulationObject simulationObject)
    {
        this.componentToCopy = simulationObject;
    }

    public void SetCanDrag(bool canDrag)
    {
        this.isStarted = canDrag;
    }
}
