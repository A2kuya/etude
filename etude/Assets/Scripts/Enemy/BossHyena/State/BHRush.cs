using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHRush : StateMachineBehaviour
{
    BossHyena bh;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bh = animator.GetComponent<BossHyena>();
        bh.rushStart();
        Debug.Log("rush");
    }    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!bh.Rush()){
            animator.SetTrigger("rushend");
        }
    }    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
