using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    [SerializeField]
    private GameObject[] otherToClose;
    public void SetElementActive()
    {
        Animator animator = this.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            bool isOpen = animator.GetBool("isElementOpen");
            animator.SetBool("isElementOpen", !isOpen);
            foreach (GameObject go in otherToClose)
            {
                Animator otherAnimator = go.GetComponent<Animator>();
                if (otherAnimator != null && !isOpen)
                {
                    otherAnimator.SetBool("isElementOpen", false);
                }
            }
        } 
    }
}
