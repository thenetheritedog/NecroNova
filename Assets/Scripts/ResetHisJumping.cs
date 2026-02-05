using UnityEngine;

public class ResetHisJumping : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool("isJumping", false);
    }
}
