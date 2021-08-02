using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureDeath : StateMachineBehaviour
{
    Vulture vulture;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>();
        vulture.StopAllCoroutines();
        animator.speed = 1;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("test");
        animator.speed = 0;
        vulture.Death();
    }
}
