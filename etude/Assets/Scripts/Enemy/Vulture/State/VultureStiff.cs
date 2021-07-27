using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureStiff : StateMachineBehaviour
{
    Vulture vulture;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>();
        vulture.StopAllCoroutines();
        vulture.StiffStart();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture.CheckFall();
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture.StiffEnd();
    }
}
