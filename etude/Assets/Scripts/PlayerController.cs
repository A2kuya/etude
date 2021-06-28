using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float moveableRange = 8f;

    public Rigidbody2D rigid;

    public float jumpVelocity = 13f;

    bool jumpRequest;
    public bool isJumping;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public Animator animator;

    private bool leftCanRun = false;
    private float leftCheckRun = 0.2f;
    private bool rightCanRun = false;
    private float rightCheckRun = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        isJumping = false;
        jumpRequest = false;
    }

    void Update()
    {
        //transform.position = new Vector2(Mathf.Clamp(transform.position.x, -MoveableRange, MoveableRange), transform.position.y);
        
        if (Input.GetKeyDown(KeyCode.X) && rigid.velocity.y == 0)
        {
            jumpRequest = true;
        }

        if (rigid.velocity.y < 0)
        {
            animator.SetTrigger("doFalling");
        }
        SetRun(KeyCode.LeftArrow, ref leftCanRun, ref leftCheckRun);
        SetRun(KeyCode.RightArrow, ref rightCanRun, ref rightCheckRun);
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        GravityScaling();
    }

    void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            animator.SetBool("isMoving", true);
            sr.flipX = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            animator.SetBool("isMoving", true);
            sr.flipX = false;
        }

        

        transform.Translate(input.x * moveSpeed * Time.deltaTime, 0, 0);
    }
    void SetRun(KeyCode key,ref bool canRun,ref float checkRun)
    {
        if (Input.GetKeyUp(key))
        {
            canRun = true; // �޸��� ������ true�� �ٲ��ش�.
            print(leftCanRun);
        }
        if (canRun)
        {
            checkRun -= Time.deltaTime;
            if (checkRun <= 0)
            {
                canRun = false;  // �޸��� ������ false�� �ٲ��ش�.
                checkRun = 0.2f; // �׸��� checkRun�� �ð��� 0.5�� �����ش�.
            }
        }
        if (Input.GetKey(key) && canRun == false)  // ���� WŰ�� ���� ���� + �޸� �� ���� ���¶��
            moveSpeed = 8;
        else if (Input.GetKey(key) && canRun == true) // ���� WŰ�� ���� ���� + �޸� �� �ִ� ���¶��
        {
            moveSpeed = 16;
            canRun = true;
            checkRun = 0.1f;
        }
        if (Input.GetKeyUp(key))
        {
            moveSpeed = 8;
        }
    }
    void Jump()
    {
        if (isJumping == false && jumpRequest == true)
        {
            isJumping = true;
            jumpRequest = false;
            rigid.velocity = Vector2.up * jumpVelocity;

            animator.SetBool("isJumping", true);
            animator.SetTrigger("doJumping");
        }
        else
        {
            jumpRequest = false;
            return;
        }
        
        
    }
    void GravityScaling()
    {
        if (rigid.velocity.y < 0)
        {
            rigid.gravityScale = fallMultiplier;
        }
        else if (rigid.velocity.y > 0 && !Input.GetKeyDown(KeyCode.Space))
        {
            rigid.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rigid.gravityScale = 1f;
        }
    }
}
