using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureCutAirEnter : StateMachineBehaviour
{
    Vulture vulture;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vulture = animator.GetComponent<Vulture>();
        vulture.ReadyCutAir();
    }
}
