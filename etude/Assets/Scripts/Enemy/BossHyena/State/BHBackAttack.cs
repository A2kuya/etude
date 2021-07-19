using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHBackAttack : StateMachineBehaviour
{
    BossHyena bh;
    bool summon;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bh = animator.GetComponent<BossHyena>();
        summon = false;
    }    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5 && !summon){
            bh.BackAttack();
            summon = true;
        }
    }    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
