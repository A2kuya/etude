using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallStones : AttackPattern
{
    Vulture vulture;
    public FallStones() : base(10, 5) {}
    public FallStones(int damage, float cooltime, Vulture v) : base(damage, cooltime) {
        vulture = v;
    }
    override public void Excute(){
        vulture.Trigger("fallingStones");
        vulture.StartCoroutine(Cooltime());
    }
}

public class ShootRay : AttackPattern
{
    Vulture vulture;
    public ShootRay() : base(10, 5) {}
    public ShootRay(int damage, float cooltime, Vulture v) : base(damage, cooltime) {
        vulture = v;
    }
    override public void Excute(){
        vulture.Trigger("shootRay");
        vulture.StartCoroutine(Cooltime());
    }
}

public class CutAir : AttackPattern
{
    Vulture vulture;
    public CutAir() : base(10, 5) {}
    public CutAir(int damage, float cooltime, Vulture v) : base(damage, cooltime) {
        vulture = v;
    }
    override public void Excute(){
        vulture.Trigger("cutAir");
        vulture.StartCoroutine(Cooltime());
    }
}
