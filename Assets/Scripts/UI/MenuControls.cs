using UnityEngine;

public class MenuControls : MonoBehaviour
{
    [SerializeField]
    private GameObject[] elementsToClose;

    public void SetMenuActive()
    {
        Animator animator = this.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            bool isOpen = animator.GetBool("isElementOpen");
            animator.SetBool("isElementOpen", !isOpen);
            if (isOpen)
            {
                foreach (GameObject element in elementsToClose)
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
}
