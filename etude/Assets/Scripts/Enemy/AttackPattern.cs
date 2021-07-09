using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AttackPattern
{
    int damage;
    float cooltime;
    float curtime;
    bool canAttack;
    public AttackPattern(int damage, float cooltime){
        this.damage = damage;
        this.cooltime = cooltime;
        this.curtime = 0;
        this.canAttack = true;
    }

    abstract public void excute();
    public void ManageCoolTime(){
        if(curtime >= 0f){
            curtime -= Time.deltaTime;
            canAttack = false;
        }
        else   
            canAttack = true;
    }
}
