using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHBite : StateMachineBehaviour
{
    BossHyena bh;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bh = animator.GetComponent<BossHyena>();
        bh.Flip();
        bh.Bite();
        Debug.Log("bite");
    }    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!bh.InRange()){
            animator.speed = 0;
        }
        else{
            bh.Flip();
            animator.speed = 1;
        }
    }    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
