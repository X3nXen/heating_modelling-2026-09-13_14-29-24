using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuUIElement: MonoBehaviour
{
    private GameObject[] collapseElements;
    private GameObject[] expandElements;
    private GameObject uiElementPrefab;

    public void Setup(GameObject[] collapseElements) 
    { 
        this.collapseElements = collapseElements;
    }
    public void Setup(GameObject[] collapseElements, List<ASimulationObject> objectElements, GameObject parent, Action<ASimulationObject> clickEvent = null, bool clickable = false)
    {
        this.uiElementPrefab = clickable ? ResourceLoader.Get("Prefabs/Clickable_Content_Element") as GameObject : ResourceLoader.Get("Prefabs/Content_Element") as GameObject;
        this.collapseElements = collapseElements;
        foreach (ASimulationObject element in objectElements)
        {
            GameObject newObject = Instantiate(uiElementPrefab);
            newObject.name = element.displayName + "_choice";
            newObject.transform.SetParent(parent.transform);
            if (clickable && clickEvent != null)
            {
                UIObject newUIObject = newObject.AddComponent<UIObject>();
                newUIObject.GetComponent<Button>().onClick.AddListener(new UnityAction(() => clickEvent(element)));
                newUIObject.SetDisplayName(element.GetDisplayName());
                newUIObject.SetDisplayImage(element.GetDisplayImage());
                newUIObject.InitUIObject();
            }
            else
            {
                DragAndDropUIElement newUIObject = newObject.AddComponent<DragAndDropUIElement>();
                newUIObject.SetPrefab(element.GetPrefab());
                newUIObject.SetDisplayName(element.GetDisplayName());
                newUIObject.SetDisplayImage(element.GetDisplayImage());
                newUIObject.SetLayerMask(((IDragAndDropObject)element).GetLayerMask());
                newUIObject.SetComponentToCopy(element);
                newUIObject.InitUIObject();
            }
        }
    }
    public void ToggleElement(bool expectedChildBehavior)
    {
        Animator animator = this.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            bool isOpen = animator.GetBool("isElementOpen");
            animator.SetBool("isElementOpen", !isOpen);
            if (!isOpen == expectedChildBehavior)
            {
                foreach (GameObject element in collapseElements)
                {
                    Animator otherAnimator = element.GetComponent<Animator>();
                    if (otherAnimator != null)
                    {
                        otherAnimator.SetBool("isElementOpen", false);
                    }
                }
            }
        }
    }

    public void SetCollapseElements(GameObject[] collapseElements)
    {
        this.collapseElements = collapseElements;
    }

    public void SetExpandElements(GameObject[] expandElements)
    {
        this.expandElements = expandElements;
    }
}
