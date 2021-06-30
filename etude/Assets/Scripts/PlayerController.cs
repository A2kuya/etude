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

    bool leftCanRun = false;
    float leftCheckRun = 0.2f;
    bool rightCanRun = false;
    float rightCheckRun = 0.2f;

    Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
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

        FloorCheck();
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

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            animator.SetBool("isMoving", true);
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            input.x *= -1;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            animator.SetBool("isMoving", true);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        transform.Translate(input.x * moveSpeed * Time.deltaTime, 0, 0);
    }
    void SetRun(KeyCode key,ref bool canRun,ref float checkRun)
    {
        if (Input.GetKeyUp(key))
        {
            canRun = true; // 달리는 판정을 true로 바꿔준다.
        }
        if (canRun)
        {
            checkRun -= Time.deltaTime;
            if (checkRun <= 0)
            {
                canRun = false;  // 달리는 판정을 false로 바꿔준다.
                checkRun = 0.2f; // 그리고 checkRun의 시간은 0.5로 돌려준다.
            }
        }
        if (Input.GetKey(key) && canRun == false || Input.GetAxisRaw("Horizontal") == 0)
        {
            moveSpeed = 8;
            animator.SetBool("isRunning", false);
        }
        else if (Input.GetKey(key) && canRun == true)
        {
            if (!isJumping || (isJumping == true && moveSpeed == 16))
            {
                moveSpeed = 16;
                leftCanRun = true;
                rightCanRun = true;
                leftCheckRun = 0.1f;
                rightCheckRun = 0.1f;
                animator.SetBool("isMoving", false);
                animator.SetBool("isRunning", true);
            }
        }
        if (Input.GetKeyUp(key))
        {
            moveSpeed = 8;
            animator.SetBool("isRunning", false);
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

    void FloorCheck()
    {
        float extraHeightText = .01f;
        RaycastHit2D raycastHit = Physics2D.Raycast(col.bounds.center, Vector2.down, col.bounds.extents.y);
        Debug.DrawRay(col.bounds.center, Vector2.down * (col.bounds.extents.y + extraHeightText));
    }
}
