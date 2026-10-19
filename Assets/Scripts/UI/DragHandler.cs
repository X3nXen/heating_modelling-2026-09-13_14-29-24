using UnityEngine;
using UnityEngine.EventSystems;
public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject dragAndDropObject;
    private GameObject dragItem;
    private Vector3 itemHeighOffset;

    public LayerMask raycastLayerMask;
    public float maxRaycastDistance = 100f;
    public void OnBeginDrag(PointerEventData eventData)
    {
        itemHeighOffset = new Vector3(0, dragAndDropObject.transform.localScale.y / 2, 0);
        dragItem = Instantiate(dragAndDropObject, Vector3.zero, Quaternion.identity);
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

    public void OnEndDrag(PointerEventData eventData)
    {
        dragItem = null;
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
    }
