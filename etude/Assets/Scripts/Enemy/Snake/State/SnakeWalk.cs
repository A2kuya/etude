using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeWalk : StateMachineBehaviour
{
    Snake snake;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake = animator.GetComponent<Snake>();
        snake.isMoving = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!snake.Miss()){
            snake.yFlip();
            snake.Movement();
            snake.Attack();
        }
        else{
            animator.SetBool("isWalk", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake.isMoving = false;
    }
}
