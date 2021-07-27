using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureChangePhase : StateMachineBehaviour
{
    Vulture vulture;
    bool isChange;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>();
        animator.speed = 0.5f;
        vulture.tag = "EnemyUnbeatable";

    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0.2f && !isChange){
            vulture.KnockbackToPlayer();
            isChange = true;
        }  
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 1;
        vulture.tag = "Enemy";
    }
}
