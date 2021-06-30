using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy
{
    RaycastHit2D raycast;

    // Start is called before the first frame update
    void Start()
    {
        hp = 100;
        damage = 10;
        playerDistance = int.MaxValue;
        range = 3f;
        cooltime = 3f;
        curtime = 0;
        atkCollider.SetActive(false);
        chaseDistance = 12f;
    }

    // Update is called once per frame
    void Update()
    {        
        CaculateplayerDistance(); //플레이어와의 거리 및 방향 계산
        ManageCoolTime();   //쿨타임 관리
    }

    override public bool Detect()
    {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position + Vector3.up, transform.right * -1, detectDistance, playerLayer);
        return rayhit.collider != null;
    }

    public override void Move()
    {
        velocity.x = speed * Time.fixedDeltaTime * -1;
        controller.Move(velocity, dir);
    }

    public override void Attack()
    {
        if (curtime <= 0f && playerDistance < range)
        {
            anim.SetTrigger("isAtk");
            curtime = cooltime;
        }
    }

    public override bool Miss()
    {
        return playerDistance > chaseDistance;
    }

    public bool InRange()
    {
        if(playerDistance < range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
