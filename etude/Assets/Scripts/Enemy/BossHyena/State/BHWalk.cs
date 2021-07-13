using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHWalk : StateMachineBehaviour
{
    BossHyena bh; 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bh = animator.GetComponent<BossHyena>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bh.Flip();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
