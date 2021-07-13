using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AttackPattern
{
    public int damage;
    protected float cooltime;
    protected float curtime;
    private bool canAttack;
    public AttackPattern(int damage, float cooltime){
        this.damage = damage;
        this.cooltime = cooltime;
        this.curtime = 0;
        this.canAttack = true;
    }

    abstract public void Excute();
    protected IEnumerator Cooltime(){
        canAttack = false;
        curtime = cooltime;
        while(curtime > 0f){
            curtime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canAttack = true;
    }
    public bool Can() { return canAttack; }
    
}
