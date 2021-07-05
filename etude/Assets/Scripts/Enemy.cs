using System.Collections;
using System.Collections.Generic;
using UnityEngine;
abstract public class Enemy : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Animator anim;
    protected Rigidbody2D rigid;
    protected GameObject player;
    //protected Controller2D controller;
    public GameObject atkCollider;

    public int hp;  //체력
    public int stiffness = 0;   //경직도
    public int damage; //데미지
    public bool isLeft; //바라보는 방향
    public LayerMask playerLayer;   //플레이어 레이어
    public LayerMask playerAttackLayer; //플레이어 공격 레이어
    public LayerMask obstacleLayer; //지형 레이어
    public int speed = 1;               //속도
    public Vector2 velocity = Vector2.zero;        //속도 벡터
    public Vector2 dir = Vector2.zero;  //방향 벡터
    public int attackSpeed;         //공격 속도
    public float detectDistance;    //감지 거리
    public float chaseDistance;     //추격 거리
    public float playerDistance;    //플레이어와의 거리
    public float range;             //공격 범위
    public bool isGround;
    public bool isJump;
    public bool isSlope;
    public bool isMoving;
    public float maxangle;
    protected Vector2 perp;
    public float angle;
    protected Vector2 spriteSize;

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
    abstract public bool Detect();  //감지
    abstract public bool Miss();    //놓침

    
    public void Move(bool left){
        int dir = (left ? -1 : 1);
        Debug.Log(perp.x + " " + perp.y);
        if(isSlope && isGround && angle < maxangle)
            rigid.velocity = perp * dir * speed * -1f;
        else if(!isSlope && isGround)
            rigid.velocity = new Vector2(dir * speed, 0);
        else if(!isGround)
            rigid.velocity = new Vector2(dir * speed, rigid.velocity.y);
    }

    virtual public void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.1f, obstacleLayer);
        //경사 체크
        Vector2 frontPosition = new Vector2(transform.position.x, transform.position.y - spriteSize.x / 2 + 0.1f);
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

    public void CaculateDistance() {  //거리계산 및 방향계산
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0);
    }
    public void yFlip()  //좌우 반전
    {
        if (isLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    public void ManageCoolTime()    //쿨타임 관리
    {
        for(int i = 0; i < attackPattern.Count; i++){
            if(attackPattern[i].curtime >= 0f)
            {
                attackPattern[i] = new AttackPattern(attackPattern[i].curtime - Time.deltaTime, attackPattern[i].cooltime);
            }
        }
    }
    public void TakeDamage(int damage, int stiffness, bool knockback = false)  //피격
    {
        hp -= damage;
        this.stiffness -= stiffness;
        if(!knockback);
            rigid.AddForce(Vector2.zero);
    }
    public void TakeDamage(int damage, int stiffness, int knockback, Vector3 attackPosition)  //피격
    {
        hp -= damage;
        this.stiffness -= stiffness;
        Vector2 v = Vector2.up;
        if(transform.position.x - attackPosition.x > 0)
            v.x = -1;
        else if(transform.position.x - attackPosition.x == 0)
            if(isLeft) v.x = -1;
            else v.x = 1;
        else    v.x = 1;
        rigid.AddForce(v);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == playerAttackLayer)
        {
            collision.gameObject.GetComponentInParent<Player>();    //타격 함수 호출
        }
    }

    public void GetSpriteSize(){
        Vector2 worldSize = Vector3.zero;
        spriteSize = spriteRenderer.sprite.rect.size;
        Vector2 localSpriteSize = spriteSize / spriteRenderer.sprite.pixelsPerUnit;
        worldSize = localSpriteSize;
        worldSize.x *= transform.lossyScale.x;
        worldSize.y *= transform.lossyScale.y;
        spriteSize = worldSize;
    }
}
