using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHRushReady : StateMachineBehaviour
{
    BossHyena bh;
    float readytime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bh = animator.GetComponent<BossHyena>();
        bh.rushStart();
        readytime = bh.rushReadyTime;
    }    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(readytime <= 0f){
            animator.SetTrigger("rush");
        }
        readytime -= Time.fixedDeltaTime;
    }    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
