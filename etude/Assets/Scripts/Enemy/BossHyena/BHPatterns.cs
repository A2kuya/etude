using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bite : AttackPattern
{
    BossHyena bossHyena;
    public Bite() : base(10, 5) {}
    public Bite(int damage, float cooltime, BossHyena bh) : base(damage, cooltime) {
        bossHyena = bh;
    }
    override public void Excute(){
        bossHyena.Trigger("bite");
        bossHyena.StartCoroutine(Cooltime());
    }
}

public class Summon : AttackPattern
{
    BossHyena bossHyena;
    public Summon() : base(0, 10) {}
    public Summon(int damage, float cooltime, BossHyena bh) : base(damage, cooltime) {
        bossHyena = bh;
    }
    override public void Excute(){
        bossHyena.Trigger("summon");
        bossHyena.StartCoroutine(Cooltime());
    }
}


public class Rush : AttackPattern
{
    BossHyena bossHyena;
    public Rush() : base(30, 10) {}
    public Rush(int damage, float cooltime, BossHyena bh) : base(damage, cooltime) {
        bossHyena = bh;
    }
    override public void Excute(){
        bossHyena.Trigger("rush");
        bossHyena.StartCoroutine(Cooltime());
    }
}

public class BackAttack : AttackPattern
{
    BossHyena bossHyena;
    public BackAttack() : base(30, 10) {}
    public BackAttack(int damage, float cooltime, BossHyena bh) : base(damage, cooltime) {
        bossHyena = bh;
    }
    override public void Excute(){
        bossHyena.Trigger("backAttack");
        bossHyena.StartCoroutine(Cooltime());
    }
}

