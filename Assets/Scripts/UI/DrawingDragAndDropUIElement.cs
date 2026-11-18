using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingDragAndDropUIElement: DragAndDropUIElement
{
        private Vector3 startPoint;
        private bool isDrawingLine = false;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!isStarted)
            {
                this.menuCloser();

                Ray ray = Camera.main.ScreenPointToRay(eventData.position);
                if (Physics.Raycast(ray, out RaycastHit hit, this.maxRaycastDistance, this.raycastLayerMask))
                {
                    startPoint = hit.point;
                    this.dragItem = Instantiate(this.prefab, startPoint, Quaternion.identity);
                    this.dragItem.SetActive(true);
                    isDrawingLine = true;
                }
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (this.dragItem != null && isDrawingLine)
            {
                Ray ray = Camera.main.ScreenPointToRay(eventData.position);
                if (Physics.Raycast(ray, out RaycastHit hit, this.maxRaycastDistance, this.raycastLayerMask))
                {
                    Vector3 currentPoint = hit.point;

                    Vector3 center = (startPoint + currentPoint) / 2f;
                    float length = Vector3.Distance(startPoint, currentPoint);

                    this.dragItem.transform.position = center;

                    if (currentPoint != startPoint)
                    {
                        this.dragItem.transform.rotation = Quaternion.LookRotation(currentPoint - startPoint);
                    }

                    Vector3 scale = this.dragItem.transform.localScale;
                    scale.z = length;
                    this.dragItem.transform.localScale = scale;
                }
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (this.dragItem != null)
            {
                this.dragItem.GetComponent<ASimulationObject>().SetEqualTo(componentToCopy);

                this.dragItem = null;
                isDrawingLine = false;
            }
        }
}
