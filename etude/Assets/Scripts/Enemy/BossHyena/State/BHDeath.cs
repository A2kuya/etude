using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHDeath : StateMachineBehaviour
{
    BossHyena bh; 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bh = animator.GetComponent<BossHyena>();
       bh.StopAllCoroutines();
       animator.speed = 1;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 0;
        bh.Death();
    }
}
