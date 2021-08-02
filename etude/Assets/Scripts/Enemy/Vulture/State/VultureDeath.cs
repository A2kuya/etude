using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureDeath : StateMachineBehaviour
{
    Vulture vulture;
    bool temp;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>();
        vulture.StopAllCoroutines();
        animator.speed = 1;
        temp = false;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f && !temp){
            temp = true;
            animator.speed = 0;
            vulture.Death();
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 0;
    }
}
