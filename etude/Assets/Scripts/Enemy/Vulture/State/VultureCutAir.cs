using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureCutAir : StateMachineBehaviour
{
    Vulture vulture;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>();      
        vulture.CutAirStart(); 
        animator.speed = 0;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 1;
        vulture.CutAirEnd();
    }
}
