using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickDrawingObject : MonoBehaviour, IPointerDownHandler
{
    private ASimulationObject objectToDraw;
    private bool isDrawing = false;
    private bool isDragging = false;
    private Vector3 firstPos;
    private Vector3 secondPos;
    private GameObject placedObject;
    private ASimulationObject objectToCopy;

    private Vector3 heightOffset;
    private Vector3 widthOffset;

    private bool shouldSetClickableBack = false;
    private bool shouldSetWallLayerBack = false;
    private Action<ClickDrawingObject> onWallAdd;
    private Vector3 startSurfaceNormal;
    private Action<ASimulationObject> addObject;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDrawing)
        {
            if (!isDragging)
            {
                if (Mouse.current == null) return;

                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ((IDragAndDropObject)objectToDraw).GetLayerMask()))
                {
                    isDragging = true;
                    firstPos = hit.point;
                    startSurfaceNormal = hit.normal;

                    heightOffset = new Vector3(0, ((IDragAndDropObject)objectToDraw).GetHeightOffset(), 0);

                    float widthVal = ((IDragAndDropObject)objectToDraw).GetWidthOffset();
                    widthOffset = startSurfaceNormal * widthVal;

                    placedObject = Instantiate(objectToDraw.gameObject, firstPos, Quaternion.identity);
                    Vector3 initialScale = placedObject.transform.localScale;
                    if (initialScale.z == 0) initialScale.z = 0.1f;
                    placedObject.transform.localScale = initialScale;
                    placedObject.SetActive(false);

                    if (LayerMask.LayerToName(placedObject.layer) == "Clickable")
                    {
                        placedObject.layer = LayerMask.NameToLayer("Default");
                        shouldSetClickableBack = true;
                    }
                    if (LayerMask.LayerToName(placedObject.layer) == "Wall_layer")
                    {
                        placedObject.layer = LayerMask.NameToLayer("Default");
                        shouldSetWallLayerBack = true;
                    }
                }
            }
            else
            {
                secondPos = eventData.pointerCurrentRaycast.worldPosition;

                Vector3 direction = secondPos - firstPos;
                Quaternion targetRotation = Quaternion.identity;

                if (direction != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(direction);
                }

                Vector3 rotatedHeightOffset = targetRotation * heightOffset;

                Vector3 center = (firstPos + secondPos) / 2 + rotatedHeightOffset + widthOffset;
                float length = Vector3.Distance(firstPos, secondPos);
                if (length < 0.01f) length = 0.1f;

                placedObject.transform.position = center;
                placedObject.transform.rotation = targetRotation;

                Vector3 scale = placedObject.transform.localScale;
                scale.z = length;
                placedObject.transform.localScale = scale;

                if (shouldSetClickableBack)
                {
                    placedObject.layer = LayerMask.NameToLayer("Clickable");
                    shouldSetClickableBack = false;
                }
                if (shouldSetWallLayerBack)
                {
                    placedObject.layer = LayerMask.NameToLayer("Wall_layer");
                    shouldSetWallLayerBack = false;
                }

                isDragging = false;
                placedObject.GetComponent<ASimulationObject>().SetEqualTo(objectToCopy);
                addObject(placedObject.GetComponent<ASimulationObject>());
                placedObject.GetComponent<ASimulationObject>().SetOriginalScale(scale);
                if (objectToCopy is Wall)
                {
                    placedObject.GetComponent<Wall>().SetCanDelete(true);
                    ClickDrawingObject clickDrawingObject = placedObject.AddComponent<ClickDrawingObject>();
                    onWallAdd(clickDrawingObject);
                }
                this.placedObject = null;
                this.firstPos = Vector3.zero;
                this.secondPos = Vector3.zero;
                this.objectToDraw = null;
                this.isDrawing = false;
                this.heightOffset = Vector3.zero;
                this.widthOffset = Vector3.zero;
                this.startSurfaceNormal = Vector3.zero;
            }
        }
    }

    public void UpdateSecondPosition()
    {
        if (isDrawing && isDragging && placedObject != null)
        {
            if (Mouse.current == null) return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ((IDragAndDropObject)objectToDraw).GetLayerMask()))
            {
                Vector3 currentPos = hit.point;

                Vector3 direction = currentPos - firstPos;
                Quaternion targetRotation = Quaternion.identity;

                if (direction != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(direction);
                }

                Vector3 rotatedHeightOffset = targetRotation * heightOffset;

                Vector3 center = (firstPos + currentPos) / 2 + rotatedHeightOffset + widthOffset;
                float length = Vector3.Distance(firstPos, currentPos);

                placedObject.transform.position = center;
                placedObject.transform.rotation = targetRotation;

                Vector3 scale = placedObject.transform.localScale;
                scale.z = length;
                placedObject.transform.localScale = scale;
                if (!placedObject.activeSelf && placedObject.transform.localScale.z > 0)
                    placedObject.SetActive(true);
            }
        }
    }

    public void SetIsDrawing(bool isDrawing)
    {
        this.isDrawing = isDrawing;
        if (!isDrawing)
        {
            this.isDragging = false;
        }
    }

    public void SetObjectToDraw(ASimulationObject aSimulationObject)
    {
        this.objectToDraw = aSimulationObject;
    }

    public bool GetIsDrawing()
    {
        return this.isDrawing;
    }

    public bool GetIsDragging()
    {
        return this.isDragging;
    }

    public void SetObjectToCopy(ASimulationObject aSimulationObject)
    {
        this.objectToCopy = aSimulationObject;
    }

    public void SetOnWallAdd(Action<ClickDrawingObject> onWallAdd)
    {
        this.onWallAdd = onWallAdd;
    }

    public void DestroySpawn()
    {
        if(this.placedObject != null)
        {
            Destroy(this.placedObject); 
        }
    }

    public void SetAddObject(Action<ASimulationObject> onAddObject)
    {
        this.addObject = onAddObject;
    }
}
