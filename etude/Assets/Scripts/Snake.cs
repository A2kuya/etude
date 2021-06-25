using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator ani;
    GameObject player;
    RaycastHit2D raycast;
    public int walkSpeed;
    public bool isLeft;
    public LayerMask isLayer;
    public bool isChase;
    public int detectDistance;
    public float playerDistance;
    public float range;
    private float cooltime;
    private float curtime;


    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("player");
        isChase = false;
        isLayer = LayerMask.GetMask("Player");
        range = 3f;
        cooltime = 5;
        curtime = 0;
    }

    private void FixedUpdate()
    {
        
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, transform.right * -1, detectDistance, isLayer);
        if (rayhit.collider != null)
        {
            isChase = true;
        }
        Move();
        Attack();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (transform.position.x - player.transform.position.x < 0 && isChase)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void Move()
    {
        if (!isChase || playerDistance > detectDistance)
        {
            isChase = false;
            ani.SetInteger("isWalk", 0);
        }
        else
        {
            Debug.Log("Ãß°Ý Áß");
            ani.SetInteger("isWalk", walkSpeed);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * walkSpeed);
        }
    }

    void Attack()
    {
        if(playerDistance < range && curtime <= 0f)
        {
            ani.SetTrigger("isAtk");
            curtime = cooltime;
        }
        else
        {
            curtime -= Time.deltaTime;
        }
    }
}
