using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eocnd : StateMachineBehaviour
{
    public GameObject s;

    public void canAttack()
    {
        s.GetComponent<Snake>().canAtk = true;
        Debug.Log("aa");
    }
}
