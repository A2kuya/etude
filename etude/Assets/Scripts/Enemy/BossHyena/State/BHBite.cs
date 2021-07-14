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
        bh.Dash();
    }    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(bh.EndDash()){
            animator.speed = 0;
        }
        else{
            bh.Flip();
            animator.speed = 1;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f){
            bh.atkCollider[0].SetActive(true);
        }
    }    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bh.atkCollider[0].SetActive(false);   
    }
}
