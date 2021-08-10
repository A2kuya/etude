using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjData : MonoBehaviour
{
    public int id;
    public bool isNpc;
     public GameObject talkPanel;

    public virtual void Action(bool isAction) {}

}