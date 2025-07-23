using UnityEngine;

public class SkipAnimation : MonoBehaviour
{
    public Animator animator;

    public void SkipToEnd()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(stateInfo.fullPathHash, 0, 0.999f); 
        animator.Update(0f);
    }

}
