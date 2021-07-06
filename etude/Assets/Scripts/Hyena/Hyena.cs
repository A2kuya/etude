using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyena : Enemy
{
    public int atkCooltime;
    public float atkCurtime;
    public int dashCooltime;
    public float dashCurtime;
    public float maxDashDistance;
    public float minDashDistance;

    public Vector2 dashTarget;
    public float dashSpeed;
    public float dashTime;
    public bool isDash;
    public bool canAttack;
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        playerAttackLayer = LayerMask.GetMask("PlayerAttackLayer");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        hpBar = Instantiate(prfHpBar, canvas.transform);
        atkCollider.SetActive(false);
        dir = Vector2.left;
        isChase = false;
        attackPattern = new List<AttackPattern>();
        attackPattern.Add(new AttackPattern(0, atkCooltime));   //공격 쿨타임
        attackPattern.Add(new AttackPattern(0, dashCooltime));  //대쉬 쿨타임
    }
    void fixedUpdate(){
        if(isDash){
            Dash();
        }
    }
    void Update()
    {
        CheckDead();
        UpdateHpBar();
        GetSpriteSize();
        CheckObstacle();
        DontSlide();
        CaculateDistance(); //거리 계산 및 방향판정
        ManageCoolTime();   //쿨타임 관리
    }

    public override void Detect(){
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position + Vector3.up, transform.right * -1, detectDistance, playerLayer);
        Debug.DrawRay(transform.position + Vector3.up, transform.right * -1, Color.red);
        if(rayhit.collider != null)
            isChase = true;
    }

    public override void Movement(){
        Move(isLeft);
    }

    public override void Attack(){

        if (atkCurtime <= 0f &&  (isGround || isSlope))
        {
            AttackPattern temp = new AttackPattern(attackPattern[0].cooltime, attackPattern[0].cooltime); 
            attackPattern[0] = temp;
            atkCurtime = atkCooltime;
            canAttack = false;
            anim.SetTrigger("isAtk");
        }
    }

    public override bool Miss(){
        //끝까지 추격
        return false;
    }
    public int KeepDistance(){
        if(playerDistance <= minDashDistance){
            dashTarget = new Vector2(transform.position.x + (maxDashDistance - minDashDistance), transform.position.y);
            return 0;
        }
        else if(playerDistance >= maxDashDistance){
            return 2;
        }
        else{
            dashTarget = player.transform.position;
            return 1;
        }
    }

    public void Dash(){
        
    }
    override public void ManageCoolTime()    //쿨타임 관리
    {
        if(atkCurtime > 0f)
            atkCurtime -= Time.deltaTime;
        else
            canAttack = true;
        if(dashCurtime > 0f) dashCurtime -= Time.deltaTime;
    }
}
