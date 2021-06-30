using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator ani;
    GameObject player;
    public GameObject atkCollider;
    public Transform pos;
    public Vector2 boxSize;
    RaycastHit2D raycast;
    public int walkSpeed;
    public bool isLeft;
    private bool isChase;
    public bool canAtk;
    public LayerMask isLayer;
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
        player = GameObject.Find("Player");
        canAtk = true;
        isChase = false;
        isLayer = LayerMask.GetMask("Player");
        playerDistance = int.MaxValue;
        range = 3f;
        cooltime = 3f;
        curtime = 0;
    }

    private void FixedUpdate()
    {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, transform.right * -1, detectDistance, isLayer);

        if (rayhit.collider != null)
        {
            isChase = true;
        }

        if (isChase && playerDistance <= detectDistance && playerDistance > range && canAtk)
        {
            ani.SetBool("isWalk", true);
            Move();
        }
        else if (isChase && playerDistance <= range && canAtk)
        {
            Attack();
        }
        else if(isChase && !canAtk)
        {
            if(ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                canAtk = true;
                Debug.Log("11111");
            }
        }
        else if(playerDistance > detectDistance)
        {
            isChase = false;
            ani.SetBool("isWalk", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, player.transform.position);
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
        manageCooltime();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * walkSpeed);
    }

    void Attack()
    {
        if (curtime <= 0f)
        {
            canAtk = false;
            ani.SetTrigger("isAtk");
            curtime = cooltime;
        }
    }


    void manageCooltime()
    {
        if (curtime > 0f)
            curtime -= Time.deltaTime;
    }
}
