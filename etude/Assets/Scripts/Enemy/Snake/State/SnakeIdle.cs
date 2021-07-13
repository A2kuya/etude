using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeIdle : StateMachineBehaviour
{
    Snake snake;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake = animator.GetComponent<Snake>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!snake.isChase){
            snake.Detect();
        }
        else if (!snake.Miss())
        {
            if (snake.InRange())
            {
                snake.Attack();
            }
            else
                animator.SetBool("isWalk", true);
        }
        if (snake.InRange())
            snake.yFlip();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
