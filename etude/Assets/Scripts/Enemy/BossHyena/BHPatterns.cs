using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuosBite : AttackPattern
{
    public ContinuosBite() : base(10, 5) {}
    public ContinuosBite(int damage, float cooltime) : base(damage, cooltime) {}
    override public void excute(){

    }
}

public class Summon : AttackPattern
{
    public Summon() : base(0, 10) {}
    public Summon(int damage, float cooltime) : base(damage, cooltime) {}
    override public void excute(){

    }
}


public class Rush : AttackPattern
{
    public Rush() : base(30, 10) {}
    public Rush(int damage, float cooltime) : base(damage, cooltime) {}
    override public void excute(){

    }
}

