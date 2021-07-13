using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaWalk : StateMachineBehaviour
{
    Hyena hyena;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena = animator.GetComponent<Hyena>();
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena.yFlip();
        if(hyena.KeepDistance() == 2)
            hyena.isMoving = true;
        else{
            animator.SetBool("isWalk", false);
            hyena.isMoving = false;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
