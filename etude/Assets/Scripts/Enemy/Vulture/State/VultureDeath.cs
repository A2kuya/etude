using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureDeath : StateMachineBehaviour
{
    Vulture vulture;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>();
        vulture.StopAllCoroutines();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f){
            vulture.Death();
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
