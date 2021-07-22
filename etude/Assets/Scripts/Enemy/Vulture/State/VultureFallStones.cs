using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureFallStones : StateMachineBehaviour
{
    Vulture vulture;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>(); 
        animator.speed = 0;
        vulture.isMoving = true;
        vulture.isAttack = true;
        iTween.MoveTo(vulture.gameObject, iTween.Hash("y", vulture.left.y, "easeType", iTween.EaseType.easeOutExpo, "oncomplete", "FallStoneStart"));
        
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture.FallStone();
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture.isMoving = false;
    }
}
