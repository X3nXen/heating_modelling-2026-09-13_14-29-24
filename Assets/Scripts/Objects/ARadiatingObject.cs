using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ARadiatingObject : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    protected string displayName;
    protected Sprite displayImage;
    protected GameObject prefab;
    protected float radiationHeat;

    protected GameObject dragItem;

    protected LayerMask raycastLayerMask;
    protected float maxRaycastDistance = 100f;
    private Vector3 itemHeighOffset;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GridPoint")
        {
            try
            {
                GridPoint gridPoint = other.GetComponentInParent<GridPoint>();
                gridPoint.SetShouldUpdate(false);
                gridPoint.SetCurrentHeat(this.radiationHeat);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public abstract void Init();

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemHeighOffset = new Vector3(0, this.prefab.transform.localScale.y / 2, 0);
        dragItem = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
        dragItem.SetActive(true);

        UpdateItemPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragItem != null)
        {
            UpdateItemPosition(eventData);
        }
    }

    private void UpdateItemPosition(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, raycastLayerMask))
        {
            dragItem.transform.position = hit.point + itemHeighOffset;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float distance))
            {
                dragItem.transform.position = ray.GetPoint(distance);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragItem = null;
    }
}
