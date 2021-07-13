using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
abstract public class Enemy : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Animator anim;
    public Rigidbody2D rigid;
    protected GameObject player;
    public GameObject prfHpBar;
    public GameObject canvas;
    protected GameObject hpBar;
    //protected Controller2D controller;
    public GameObject atkCollider;

    public int hp;  //체력
    public int curHp;   //현재 체력
    public int stiffness = 0;   //경직도
    public int damage; //데미지
    public bool isLeft; //바라보는 방향
    public LayerMask playerLayer;   //플레이어 레이어
    public LayerMask playerAttackLayer; //플레이어 공격 레이어
    public LayerMask obstacleLayer; //지형 레이어
    public int speed = 1;               //속도
    public Vector2 dir = Vector2.zero;  //방향 벡터
    public int attackSpeed;         //공격 속도
    public float detectDistance;    //감지 거리
    public float chaseDistance;     //추격 거리
    public Vector2 playerVector;    //플레이어와의 거리
    public float playerDistance;
    public float range;             //공격 범위
    public bool isGround;
    public bool isJump;
    public bool isSlope;
    public bool isMoving;
    public bool isChase;
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
    public void Stop(){
        rigid.velocity = Vector2.zero;
        isMoving = false;
    }

    virtual public void CheckObstacle(){
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
    public void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0);
        if(isLeft) dir = Vector2.left;
        else dir = Vector2.right;
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
    public void yFlip(bool re)  //좌우 반전
    {  
        bool reverse = (re? !isLeft : isLeft);
        if (reverse)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
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
    public void TakeDamage(int damage, int stiffness = 0)  //피격
    {
        curHp -= damage;
        this.stiffness -= stiffness;
        CheckDead();
        if(gameObject.activeSelf)
            anim.SetTrigger("isHurt");
    }
    public void TakeDamage(int damage, int stiffness, Vector3 attackPosition)  //피격
    {
        curHp -= damage;
        this.stiffness -= stiffness;
        Vector2 v;
        if(transform.position.x - attackPosition.x > 0)
            v = Vector2.right;
        else if(transform.position.x - attackPosition.x == 0)
            if(isLeft) v = Vector2.right;
            else v = Vector2.left;
        else    v = Vector2.left;
        Stop();
        rigid.AddForce(v + Vector2.up);
        anim.SetTrigger("isHurt");
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
    public void UpdateHpBar(){
        hpBar.SetActive(curHp != hp);
        hpBar.GetComponentInChildren<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, spriteSize.y + 0.5f, 0));
        hpBar.GetComponentInChildren<Slider>().value = (float) curHp / (float) hp;
    }
    public void CheckDead(bool death = false){
        if(curHp <= 0f || death){
            anim.SetTrigger("isDead");
        }
    }
    public void Death(){
        Destroy(hpBar);
        Destroy(gameObject);
    }
}
