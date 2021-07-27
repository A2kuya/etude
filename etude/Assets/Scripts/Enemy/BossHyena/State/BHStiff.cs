using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHStiff : StateMachineBehaviour
{
    BossHyena bh;
    float stiffTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bh = animator.GetComponent<BossHyena>();
        bh.StiffStart();
        bh.StopAllCoroutines();
    }    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(bh.CheckStiffTime()){
            bh.StiffEnd();
            bh.Trigger("stiffEnd");
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
