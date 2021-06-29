using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeWalk : StateMachineBehaviour
{
    Snake snake;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake = animator.GetComponent<Snake>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        snake.walkSpeed.x = snake.isLeft ? Vector2.left.x : Vector2.right.x;
        RaycastHit2D raycast = Physics2D.Raycast(snake.transform.position, snake.transform.up * -1, 0.1f, snake.obstacleLayer);
        Debug.DrawRay(snake.transform.position, snake.transform.up * -1, Color.green);
        if (raycast.collider == null)
        {
            snake.walkSpeed.y -= 9.8f * Time.fixedDeltaTime;
        }
        else
        {
            snake.walkSpeed.y = 0;
        }
        snake.GetComponent<Rigidbody2D>().velocity = snake.walkSpeed;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
