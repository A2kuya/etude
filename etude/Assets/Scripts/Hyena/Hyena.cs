using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyena : Enemy
{
    public int atkCooltime;
    public float atkCurtime;
    public bool canAttack;
    public bool canDash;
    public int dashCooltime;
    public float dashCurtime;
    public float maxDashDistance;
    public float minDashDistance;
    public float dashDistance;
    public float dashSpeed;
    public float dashTime;
    public float maxDashTime = 0.5f;
    public bool isDash;
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

    void FixedUpdate() {
        if(isMoving){
            Movement();
        }
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
        Move(isClose);
    }

    public override void Attack(){

    }

    public override bool Miss(){
        //끝까지 추격
        return false;
    }
    public int KeepDistance(){
        if(playerDistance <= minDashDistance){
            isClose = true;
            return 0;
        }
        else if(playerDistance >= maxDashDistance){
            isClose = false;
            return 2;
        }
        else{
            dashDistance = Mathf.Abs(playerVector.x) - 2f;
            dashTime = Mathf.Min(dashDistance / dashSpeed, maxDashTime);
            isClose = false;
            return 1;
        }
    }
    public bool isClose;
    public float startTime;
    public void Dash(){
        float progress = (Time.time - startTime) / dashTime;
        Move((isClose ? -1 : 1) * dir, dashSpeed);
        if (progress >= 1f)
		{
            Stop();
		    isDash = false;
		}
    }
    override public void ManageCoolTime()    //쿨타임 관리
    {
        if(atkCurtime >= 0f)
            atkCurtime -= Time.deltaTime;
        else
            canAttack = true;

        if(dashCurtime >= 0f)
            dashCurtime -= Time.deltaTime;
        else
            canDash = true;
    }
}
