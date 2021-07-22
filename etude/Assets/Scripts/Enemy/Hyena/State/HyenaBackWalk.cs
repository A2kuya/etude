using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaBackWalk : StateMachineBehaviour
{
    Hyena hyena;
    float curtime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       hyena = animator.GetComponent<Hyena>();
       hyena.isClose = true;
       hyena.isMoving = true;
       curtime = 2;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena.Flip(true);
        if(hyena.canDash && hyena.canAttack){
            hyena.KeepDistance();
            hyena.isClose = false;
            hyena.isMoving = false;
            hyena.Flip();
            animator.SetTrigger("isDash");
        }
        //else if(hyena.KeepDistance() == 1){
        else if(curtime <= 0f){
            hyena.isClose = false;
            hyena.isMoving = false;
            hyena.Flip();
            animator.SetBool("isBackWalk", false);
        }
        curtime -= Time.deltaTime;
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
