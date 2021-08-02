using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Boss : Enemy
{
    public phase state;
    public enum phase{ first, second, third }
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
        SpawnCoins();
        Destroy(hpBar);
        StopAllCoroutines();
        gameObject.tag = "Untagged";
        anim.speed = 0;
        this.enabled = false;
    }
    public bool CheckPhase(){
        if((float) curHp/maxHp <= 0.2 && state != phase.third){
            state = phase.third;
            return true;
        }
        else if((float) curHp/maxHp <= 0.5 && state == phase.first){
            state = phase.second;
            return true;
        }
        return false;
    }
    public int GetPhase(){
        switch (state)
        {
            case phase.first:
                return 1;
            case phase.second:
                return 2;
            case phase.third:
                return 3;
        }
        return 0;
    }
}
