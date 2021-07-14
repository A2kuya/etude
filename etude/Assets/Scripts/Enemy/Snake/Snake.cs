using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : GroundEnemy
{
    public int cooltime;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        hpBar = Instantiate(prfHpBar, canvas.transform);
        atkCollider.SetActive(false);
        dir = Vector2.left;
        attackPattern = new List<AttackPattern>();
        attackPattern.Add(new AttackPattern(0, cooltime));
    }

    // Update is called once per frame
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

    override public void Detect()
    {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position + Vector3.up, transform.right * -1, detectDistance, playerLayer);
        if(rayhit.collider != null)
            isChase = true;
    }
    public override void Movement()
    {
        Move(false);
    }
    public override void Attack()
    {
        if (InRange() && attackPattern[0].curtime <= 0f)
        {
            AttackPattern temp = new AttackPattern(attackPattern[0].cooltime, attackPattern[0].cooltime); 
            attackPattern[0] = temp;
            anim.SetTrigger("isAtk");
        }
        else if(Miss() || InRange()){
            anim.SetBool("isWalk", false);
        }
        else{
            anim.SetBool("isWalk", true);
        }
    }
    public override bool Miss()
    {
        isChase = playerDistance < chaseDistance;
        if(isChase && Mathf.Abs(playerVector.x) < 1f && Mathf.Abs(playerVector.y) > range){
            return true;            
        }
        return !isChase;
    }
    public bool InRange()
    {
        return playerDistance < range;
    }
    public override void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.1f, obstacleLayer);
        //경사 체크
        Vector2 frontPosition = new Vector2(transform.position.x, transform.position.y + 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1, obstacleLayer);
        RaycastHit2D fronthit = Physics2D.Raycast(frontPosition, (isLeft ? Vector2.left : Vector2.right), 0.1f + spriteSize.x /2, obstacleLayer);
        Debug.DrawRay(frontPosition, (isLeft ? Vector2.left : Vector2.right), Color.red);
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if(fronthit){
            CheckSlope(fronthit);
        }else if(hit){
            CheckSlope(hit);
        }
    }
    
}
