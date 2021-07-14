using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
abstract public class GroundEnemy : Enemy
{
    //protected Controller2D controller;
    public GameObject atkCollider;


    public int damage; //데미지
    public int speed = 1;               //속도
    public Vector2 dir = Vector2.zero;  //방향 벡터
    public float detectDistance;    //감지 거리
    public float chaseDistance;     //추격 거리

    public float range;             //공격 범위
    public bool isSlope;
    public bool isChase;
    public float maxangle;
    protected Vector2 perp;
    public float angle;
    public struct AttackPattern{
            public AttackPattern(float cur, float cool){
                cooltime = cool;
                curtime = cur;
            }
            public float cooltime;         //공격 쿨타임
            public float curtime;          //쿨타임 시간
    }
    public List<AttackPattern> attackPattern;   //공격 패턴
    abstract public void Movement();    //움직임
    abstract public void Attack();  //공격
    abstract public void Detect();  //감지
    abstract public bool Miss();    //놓침

    
    public void Move(bool re){
        int reverse = (re ? -1 : 1);
        if(isSlope && isGround && angle < maxangle)
            rigid.velocity = perp * dir.x * reverse * speed * -1f;
        else if(!isSlope && isGround)
            rigid.velocity = new Vector2(dir.x * reverse * speed, 0);
        else if(!isGround)
            rigid.velocity = new Vector2(dir.x * reverse * speed, rigid.velocity.y);
    }
    public void Move(bool re, float speed){
        int reverse = (re ? -1 : 1);
        if(isSlope && isGround && angle < maxangle)
            rigid.velocity = perp * dir.x * reverse * speed * -1f;
        else if(!isSlope && isGround)
            rigid.velocity = new Vector2(dir.x * reverse * speed, 0);
        else if(!isGround)
            rigid.velocity = new Vector2(dir.x * reverse * speed, rigid.velocity.y);
    }
    public void Move(Vector2 dir, float speed){     //해당 속도로 좌우이동
        if(isSlope && isGround && angle < maxangle)
            rigid.velocity = perp * dir.x * speed * -1f;
        else if(!isSlope && isGround)
            rigid.velocity = new Vector2(dir.x * speed, 0);
        else if(!isGround)
            rigid.velocity = new Vector2(dir.x * speed, rigid.velocity.y);
    }
    

    override public void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
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
    public void CheckSlope(RaycastHit2D hit){
            perp = Vector2.Perpendicular(hit.normal).normalized;
            angle = Vector2.Angle(hit.normal, Vector2.up);
            isSlope = (angle != 0);
    }
    public void DontSlide(){
        if(isSlope && !isMoving)
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    override public void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0);
        if(isLeft) dir = Vector2.left;
        else dir = Vector2.right;
    }
    
    virtual public void ManageCoolTime()    //쿨타임 관리
    {
        AttackPattern temp;
        for(int i = 0; i < attackPattern.Count; i++){
            temp = new AttackPattern(attackPattern[i].curtime, attackPattern[i].cooltime);
            if(attackPattern[i].curtime >= 0f)
            {
                temp.curtime -= Time.deltaTime;
                attackPattern[i] = temp;
            }
        }
    }
    
}
