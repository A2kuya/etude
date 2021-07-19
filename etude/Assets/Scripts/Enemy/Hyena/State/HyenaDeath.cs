using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaDeath : StateMachineBehaviour
{    
    Hyena hyena;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       hyena = animator.GetComponent<Hyena>();
       hyena.Stop();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f){
           hyena.Death();
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       hyena.Death();
    }
}