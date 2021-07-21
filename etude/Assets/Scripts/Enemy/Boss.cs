using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Boss : Enemy
{

    public Dictionary<string, AttackPattern> attackPatterns;
    public GameObject[] atkCollider;
    public int walkSpeed;


    public void Move(Vector2 dir, float speed)
    {
        if (isGround)
            rigid.velocity = new Vector2(dir.x * speed, 0);
        else if (!isGround)
            rigid.velocity = new Vector2(dir.x * speed, rigid.velocity.y);
    }
    
    override public void Death(){
        Destroy(hpBar);
        StopAllCoroutines();
        gameObject.tag = "Untagged";
        anim.speed = 0;
        this.enabled = false;
    }
}
