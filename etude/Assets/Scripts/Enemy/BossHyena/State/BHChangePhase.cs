using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHChangePhase : StateMachineBehaviour
{
    BossHyena bh; 
    bool isChange;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bh = animator.GetComponent<BossHyena>();
       bh.tag = "EnemyUnbeatable";
       isChange = false;
       animator.speed = 1;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f && !isChange){
            bh.KnockbackToPlayer();
            isChange = true;
        }        
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bh.tag = "Enemy";
    }
}
