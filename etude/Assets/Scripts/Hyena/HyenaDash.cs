using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaDash : StateMachineBehaviour
{
    Hyena hyena;
    float dashTime;
    float curDashTime;
    Vector2 target;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       hyena = animator.GetComponent<Hyena>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena.Attack();
    }

}
