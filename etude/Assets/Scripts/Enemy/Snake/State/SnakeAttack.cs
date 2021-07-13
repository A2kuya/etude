using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAttack : StateMachineBehaviour
{
    Snake snake;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake = animator.GetComponent<Snake>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f)
            snake.atkCollider.SetActive(true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake.atkCollider.SetActive(false);
        if (snake.Miss() || snake.InRange())
        {
            animator.SetBool("isWalk", false);
        }else{
            animator.SetBool("isWalk", true);
        }
    }
}
