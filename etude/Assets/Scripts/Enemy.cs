using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator anim;
    Rigidbody2D rigid;
    GameObject player;
    Controller2D controller;
    public GameObject atkCollider;

    public int hp;  //체력바
    public bool isLeft; //왼쪽을 바라보는지
    public LayerMask playerLayer;   //플레이어 레이어
    public LayerMask playerAttackLayer; //플레이어 공격 레이어
    public LayerMask obstacleLayer; //지형 레이어
    public int detectDistance;      //탐지 거리
    public float playerDistance;    //플레이어와의 거리
    public float range;             //공격 범위
    private float cooltime;         //공격 쿨타임
    private float curtime;          //줄어드는 공격 쿨타임

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        playerAttackLayer = LayerMask.GetMask("PlayerAttackLayer");
        obstacleLayer = LayerMask.GetMask("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        CaculateplayerDistance();
    }

    public void Attack() { }
    public void Detect()
    {
        
    }

    public void CaculateplayerDistance() {
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0) ? true : false;
    }
    public void ManageSprite()  //바라보는 쪽을 보게 하는 함수(기본은 왼쪽)
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
}
