using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator anim;
    Rigidbody2D rigid;
    GameObject player;
    public GameObject atkCollider;
    RaycastHit2D raycast;
    public Vector2 walkSpeed;
    public bool isLeft;
    private bool isChase;
    public bool canAtk;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public int detectDistance;
    public float playerDistance;
    public float range;
    private float cooltime;
    private float curtime;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        canAtk = true;
        isChase = false;
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        playerDistance = int.MaxValue;
        range = 3f;
        cooltime = 3f;
        curtime = 0;
        atkCollider = transform.GetChild(0).gameObject;
        atkCollider.SetActive(false);
        walkSpeed = Vector2.zero;
    }

    private void FixedUpdate()
    {
        //적 탐지
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, transform.right * -1, detectDistance, playerLayer);
        if (rayhit.collider != null)
        {
          isChase = true;
        }
        //적 추격
        if (isChase && playerDistance <= detectDistance && playerDistance > range && canAtk)
        {
            anim.SetBool("isWalk", true);
        }
        //공격
        else if (isChase && playerDistance <= range && canAtk)
        {
            Attack();
        }
        else if(isChase && !canAtk)
        {
            anim.SetBool("isWalk", false);
        }
        //탐지범위 밖으로 이탈
        else if(playerDistance > detectDistance)
        {
            isChase = false;
            anim.SetBool("isWalk", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어와의 거리 계산
        playerDistance = Vector3.Distance(transform.position, player.transform.position);
        //좌우 반전
        if (isChase && transform.position.x - player.transform.position.x < 0 && canAtk)
        {
            isLeft = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if(isChase && canAtk)
        {
            isLeft = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        //쿨타임 관리
        manageCooltime();
    }

    public void Move()
    {
        walkSpeed.x = isLeft ? Vector2.left.x*2 : Vector2.right.x*2;
        Debug.Log(walkSpeed.x);
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, transform.up*-1, 0.1f, obstacleLayer);
        if (raycast.collider == null)
        {
            walkSpeed.y -= 9.8f * Time.fixedDeltaTime;
        }
        else
        {
            walkSpeed.y = 0;
        }
        rigid.velocity = walkSpeed;
    }

    void Attack()
    {
        if (curtime <= 0f)
        {
            canAtk = false;
            anim.SetTrigger("isAtk");
            curtime = cooltime;
        }
    }


    void manageCooltime()
    {
        if (curtime > 0f)
            curtime -= Time.deltaTime;
    }
}
