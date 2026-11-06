using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropUIElement: UIObject, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    protected LayerMask raycastLayerMask;
    protected float maxRaycastDistance = 1000f;
    private Vector3 itemHeighOffset;
    protected GameObject dragItem;
    protected GameObject prefab;
    protected IPlaceableObject mountComponent;

    public override void InitUIObject()
    {
        this.transform.Find("Image").gameObject.GetComponent<Image>().sprite = this.displayImage;
        this.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = this.displayName;
        this.transform.localScale = Vector3.one;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.itemHeighOffset = new Vector3(0, this.prefab.transform.localScale.y/2, 0);
        this.dragItem = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
        this.dragItem.SetActive(true);

        UpdateItemPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.dragItem != null)
        {
            UpdateItemPosition(eventData);
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        this.dragItem = null;
    }
    private void UpdateItemPosition(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, this.maxRaycastDistance, this.raycastLayerMask))
        {
            this.dragItem.transform.position = hit.point + itemHeighOffset;
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
}
