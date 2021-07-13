using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaIdle : StateMachineBehaviour
{
    Hyena hyena;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena = animator.GetComponent<Hyena>();
        hyena.Stop();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!hyena.isChase){
            hyena.Detect();
        }
        else{
            hyena.yFlip();
            if(hyena.KeepDistance() == 1 && hyena.canAttack && hyena.canDash){
                //플레이어 앞으로 대쉬
                animator.SetTrigger("isDash");
            }
            else if(hyena.KeepDistance() == 0 && hyena.canDash){
                //뒤로 걷기
                animator.SetBool("isBackWalk", true);
            }   
            else if(hyena.KeepDistance() == 1 && !hyena.canAttack){
                hyena.Stop();
            }
            else if(hyena.KeepDistance() == 2){
                animator.SetBool("isWalk", true);
            }else{

            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        
    }
}
