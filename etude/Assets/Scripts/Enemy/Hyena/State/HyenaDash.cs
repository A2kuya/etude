using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaDash : StateMachineBehaviour
{
    Hyena hyena;
    Vector2 target;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena = animator.GetComponent<Hyena>();  
        hyena.isDash = true;
        hyena.isMoving = true;
        hyena.dashCurtime = hyena.dashCooltime;
        hyena.canDash = false;
        hyena.startTime = Time.time;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena.Flip();
        if(hyena.isDash)
            animator.speed = 0f;
        else
            animator.speed = 1f;
        
        hyena.Attack();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena.isDash = false;
        hyena.isMoving = false;
        hyena.Stop();
        hyena.Attack();
    }

}
