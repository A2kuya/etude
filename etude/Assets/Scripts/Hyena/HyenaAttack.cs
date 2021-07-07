using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaAttack : StateMachineBehaviour
{
    Hyena hyena;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena = animator.GetComponent<Hyena>();
        hyena.atkCurtime = hyena.atkCooltime;
        hyena.canAttack = false;
        hyena.Stop();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f){
            
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f){

        }
        else{
            hyena.atkCollider.SetActive(true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
