using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaBackWalk : StateMachineBehaviour
{
    Hyena hyena;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       hyena = animator.GetComponent<Hyena>();
       hyena.isMoving = true;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena.Flip(true);
        if(hyena.canDash && hyena.canAttack){
            hyena.KeepDistance();
            hyena.isClose = false;
            hyena.isMoving = false;
            hyena.Flip();
            animator.SetTrigger("isDash");
        }
        else if(hyena.KeepDistance() == 1){
            hyena.isClose = false;
            hyena.isMoving = false;
            hyena.Flip();
            animator.SetBool("isBackWalk", false);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
