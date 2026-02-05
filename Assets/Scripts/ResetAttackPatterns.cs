using UnityEngine;

public class ResetAttackPatterns : StateMachineBehaviour
{
    public string isUsingRootMotionBool;
    public bool isUsingRootMotionStatus;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isUsingRootMotionBool, isUsingRootMotionStatus);
    }
}
