using System.Collections;
using System.Collections.Generic;
using UnityEngine;
abstract public class Enemy : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    protected Animator anim;
    Rigidbody2D rigid;
    GameObject player;
    protected Controller2D controller;
    public GameObject atkCollider;

    public int hp;  //체력바
    public int damage; //데미지
    public bool isLeft; //왼쪽을 바라보는지
    public LayerMask playerLayer;   //플레이어 레이어
    public LayerMask playerAttackLayer; //플레이어 공격 레이어
    public LayerMask obstacleLayer; //지형 레이어
    public int speed = 1;               //속도 계수
    public Vector2 velocity = Vector2.zero;        //이동 속도
    public Vector2 dir = Vector2.zero;  //방향
    public int attackSpeed;         //공격 속도
    public float detectDistance;    //탐지 거리
    public float chaseDistance;     //추격 거리
    public float playerDistance;    //플레이어와의 거리
    public float range;             //공격 범위
    public float cooltime;         //공격 쿨타임
    public float curtime;          //줄어드는 공격 쿨타임
    protected float gravity;                  //중력

    void Awake()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        playerAttackLayer = LayerMask.GetMask("PlayerAttackLayer");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        controller = GetComponent<Controller2D>();
    }

    abstract public void Move();    //움직임
    abstract public void Attack();  //공격
    abstract public bool Detect();  //탐지
    abstract public bool Miss();    //놓치는 것

    public void CaculateplayerDistance() {  //플레이어 거리계산 및 방향 판정
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        if(isLeft = (transform.position.x - player.transform.position.x > 0))
        {
            dir.x = Vector2.left.x;
        }
        else
        {
            dir.x = Vector2.right.x;
        }
    }
    public void yFlip()  //좌우 플립(왼쪽이 참)
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
    public void ManageCoolTime()
    {
        if(curtime >= 0f)
        {
            curtime -= Time.deltaTime;
        }
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == playerAttackLayer)
        {
            //감지된 오브젝트에서 데미지를 받아와서 처리
        }
    }

    protected void CaculateVelocity()
    {

    }
}
