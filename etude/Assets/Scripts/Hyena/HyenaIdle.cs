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
            if(hyena.KeepDistance() == 0){
                //뒤로 대쉬
                Debug.Log("close");
                hyena.isDash = true;
                animator.SetTrigger("isDash");
            }
            else if(hyena.KeepDistance() == 1 && hyena.canAttack){
                //플레이어 앞으로 대쉬
                Debug.Log("attack");
                hyena.isDash = true;
                animator.SetTrigger("isDash");
            }
            else if(hyena.KeepDistance() == 1 && !hyena.canAttack){
                hyena.Stop();
            }
            else if(hyena.KeepDistance() == 2){
                animator.SetBool("isWalk", true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        
    }
}
