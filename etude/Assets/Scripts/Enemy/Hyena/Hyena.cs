using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyena : GroundEnemy
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
    public float maxDashTime;
    public bool isDash;
    public bool isClose;
    public float startTime;
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        atkCollider.SetActive(false);
        attackPattern = new List<AttackPattern>();
        attackPattern.Add(new AttackPattern(0, atkCooltime));   //공격 쿨타임
        attackPattern.Add(new AttackPattern(0, dashCooltime));
        curHp = maxHp;  //대쉬 쿨타임
        Flip();
        CaculateDistance();
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
        if(InRange()){
            anim.SetTrigger("isAttack");
        }
    }
    public bool InRange(){
        if(Mathf.Abs(playerVector.y) < 3f)
            return true;
        else
            return false;
    }
    public override bool Miss(){
        //끝까지 추격
        return false;
    }
    override protected void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
        //경사 체크
        Vector2 frontPosition = new Vector2(transform.position.x + spriteSize.x * (isLeft ? -1 : 1) / 4, transform.position.y + 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, obstacleLayer);
        RaycastHit2D fronthit = Physics2D.Raycast(frontPosition, (isLeft ? Vector2.left : Vector2.right), 0.1f + spriteSize.x /2, obstacleLayer);
        Debug.DrawRay(frontPosition, (isLeft ? Vector2.left : Vector2.right), Color.red);
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if(fronthit){
            CheckSlope(fronthit);
        }else if(hit){
            CheckSlope(hit);
        }
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
            dashDistance = Mathf.Abs(Mathf.Abs(playerVector.x) - 2f);
            dashTime = Mathf.Min(dashDistance / dashSpeed, maxDashTime);
            isClose = false;
            return 1;
        }
    }
    public void Dash(){
        float progress = (Time.time - startTime) / dashTime;
        Move(isClose, dashSpeed);
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
    public void SetAttack(){
        isChase = true;
    }
}
