using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaHurt : StateMachineBehaviour
{
    Hyena hyena;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hyena = animator.GetComponent<Hyena>();
        //hyena.gameObject.tag = "EnemyUnbeatable";
        hyena.CheckDead();
        animator.speed = 0.5f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //hyena.gameObject.tag = "Enemy";
        animator.speed = 1;
    }

}
