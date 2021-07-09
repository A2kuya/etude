using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHurt : StateMachineBehaviour
{
    Snake snake;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake = animator.GetComponent<Snake>();
        //snake.gameObject.tag = "EnemyUnbeatable";
        snake.CheckDead();
        animator.speed = 0.5f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //snake.gameObject.tag = "Enemy";
        animator.speed = 1;
    }

}
