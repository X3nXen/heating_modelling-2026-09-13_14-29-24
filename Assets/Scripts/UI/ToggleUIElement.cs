using UnityEngine;

public class ToggleUIElement: MonoBehaviour
{
    private Transform[] collapseElements;
    private Transform[] expandElements;
    public void ToggleElement()
    {
        Animator animator = this.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            bool isOpen = animator.GetBool("isElementOpen");
            animator.SetBool("isElementOpen", !isOpen);
            if (!isOpen)
            {
                foreach (Transform element in collapseElements)
                {
                    Animator otherAnimator = element.GetComponent<Animator>();
                    if (otherAnimator != null)
                    {
                        Debug.Log("Close other");
                        otherAnimator.SetBool("isElementOpen", false);
                    }
                }
            }
        }
    }

    public void SetCollapseElements(Transform[] collapseElements)
    {
        this.collapseElements = collapseElements;
    }

    public void SetExpandElements(Transform[] expandElements)
    {
        this.expandElements = expandElements;
    }
}
