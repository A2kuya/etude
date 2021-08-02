using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeDeath : StateMachineBehaviour
{    
    Snake snake;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       snake = animator.GetComponent<Snake>();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            snake.Death();
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake.Death();
    }

}
