using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;

	public int maxHp = 100;
	public int hp = 100;
	public int damage = 10;
	public int stiffness = 0;

	public Animator animator;

	bool leftCanRun = false;
	float leftCheckRun = 0.2f;
	bool rightCanRun = false;
	float rightCheckRun = 0.2f;

	bool isJumping = false;
	public bool isAttacking = false;

	public Collider2D attackPos;
	bool canMove = true;
	bool isDashing = false;

	Vector2 playerDir;
	float dashDistance = 10f;
	float dashTime = 0.2f;
	float startTime = 0f;
	Vector2 moveAmount;

	public bool isLadder = false;
	Collider2D ladderCol;
	bool topLadder = false;
	bool bottomLadder = false;
	Rigidbody2D rb;
	float originGravity;
	float climbSpeed = 8f;
	public bool isClimbing = false;
	float jumpCoolTime = 0.4f;
	float jumpTime = 0.4f;
	bool startJumpTime = false;

	public bool downJump = false;

	public bool isSpecialAttacking = false;
	public BrokeGround brokeGround;

	void Start()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
		rb = GetComponent<Rigidbody2D>();

		originGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		gravity = originGravity;
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		playerDir = new Vector3(1, 0, 0);

		attackPos.gameObject.SetActive(false);
	}

	void Update()
	{
		MoveAnimator();
		JumpAnimator();
		JumpAttackAnimator();
		
		CalculateVelocity();

		LadderCoolTime();
		LadderClimb();
		UpdateDash();

		if (canMove)
		{
			controller.Move(velocity * Time.deltaTime, directionalInput, downJump);
			if (downJump) downJump = false;
		}
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

		SetRun(KeyCode.LeftArrow, ref leftCanRun, ref leftCheckRun);
		SetRun(KeyCode.RightArrow, ref rightCanRun, ref rightCheckRun);
	}
	
	public void SetDirectionalInput(Vector2 input)
	{
		directionalInput = input;
	}

	public void OnJumpInputDown()
	{
		if (wallSliding)
		{
			if (wallDirX == directionalInput.x)
			{
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else if (directionalInput.x == 0)
			{
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			}
			else
			{
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}
		if (controller.collisions.below)
		{
			if (controller.collisions.slidingDownMaxSlope)
			{
				if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
				{ // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			}
			else
			{
				velocity.y = maxJumpVelocity;
			}
		}
	}

	public void OnJumpInputUp()
	{
		if (velocity.y > minJumpVelocity)
		{
			velocity.y = minJumpVelocity;
		}
	}

	public void Dash()
	{
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("player_dash_1"))
		{
			animator.SetTrigger("doDash");
			canMove = false;
			isDashing = true;
			startTime = Time.time;
		}
	}

	void UpdateDash()
    {
		if (isDashing)
		{
			float progress = (Time.time - startTime) / dashTime;
			progress = Mathf.Clamp(progress, 0, 1);
			moveAmount = new Vector3(dashDistance, 0, 0) * progress * playerDir.x;
			controller.Move(moveAmount * 10 * Time.deltaTime, directionalInput);

			if (progress >= 1)
			{
				isDashing = false;
				canMove = true;
			}
		}
	}

	void JumpAttackAnimator()
    {
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_jump_attack"))
		{
			velocity.y -= 0.5f;
		}
	}

	void CalculateVelocity()
	{
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}

	void MoveAnimator()
    {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();

		if (Input.GetAxisRaw("Horizontal") == 0)
		{
			animator.SetBool("isMoving", false);
		}
		else if (Input.GetAxisRaw("Horizontal") < 0)
		{
			animator.SetBool("isMoving", true);
			if (!isAttacking)
			{
				sr.flipX = true;
				attackPos.transform.rotation = Quaternion.Euler(0, 180, 0);
				playerDir = new Vector3(-1, 0, 0);
			}
		}
		else if (Input.GetAxisRaw("Horizontal") > 0)
		{
			animator.SetBool("isMoving", true);
			if (!isAttacking)
			{
				sr.flipX = false;
				attackPos.transform.rotation = Quaternion.Euler(0, 0, 0);
				playerDir = new Vector3(1, 0, 0);
			}
		}
	}

	void SetRun(KeyCode key, ref bool canRun, ref float checkRun)
	{
		if (Input.GetKeyUp(key))
		{
			canRun = true; // �޸��� ������ true�� �ٲ��ش�.
		}
		if (canRun)
		{
			checkRun -= Time.deltaTime;
			if (checkRun <= 0)
			{
				canRun = false;  // �޸��� ������ false�� �ٲ��ش�.
				checkRun = 0.2f; // �׸��� checkRun�� �ð��� 0.2�� �����ش�.
			}
		}
		if (Input.GetKey(key) && canRun == false || Input.GetAxisRaw("Horizontal") == 0)
		{
			moveSpeed = 8;
			animator.SetBool("isRunning", false);
		}
		else if (Input.GetKey(key) && canRun == true)
		{
			moveSpeed = 16;
			leftCanRun = true;
			rightCanRun = true;
			leftCheckRun = 0.1f;
			rightCheckRun = 0.1f;
			animator.SetBool("isMoving", false);
			animator.SetBool("isRunning", true);
		}
		if (Input.GetKeyUp(key))
		{
			moveSpeed = 8;
			animator.SetBool("isRunning", false);
		}
	}

	void LadderClimb()
    {
		if (isLadder && !isAttacking)
		{
			if (directionalInput.y != 0 && jumpTime == jumpCoolTime)
			{
				isClimbing = true;
			}

			if (isClimbing)
			{
				if (directionalInput.y < 0 && bottomLadder || directionalInput.y > 0 && topLadder)
				{
					isClimbing = false;
					animator.SetBool("isClimbing", false);
					return;
				}

				animator.SetBool("isClimbing", true);
				animator.SetBool("isJumping", false);
				isAttacking = false;

				transform.position = new Vector3(ladderCol.transform.position.x, transform.position.y);
				velocity.x = 0;
				velocity.y = 0;
				gravity = 0;

				if (!animator.GetCurrentAnimatorStateInfo(0).IsName("player_climb"))
				{
					animator.SetTrigger("doClimb");
				}

				if (directionalInput.y == 0)
				{
					animator.SetFloat("climbSpeed", 0);
				}
				else if (directionalInput.y > 0 && !topLadder)
				{
					velocity.y = directionalInput.y * climbSpeed;
					animator.SetFloat("climbSpeed", 1);
				}
				else if (directionalInput.y < 0 && !bottomLadder)
				{
					if(topLadder)
                    {
						controller.isClimbing = true;
					}
					velocity.y = directionalInput.y * climbSpeed;
					animator.SetFloat("climbSpeed", 1);
				}

				if (Input.GetKeyDown(KeyCode.X))
				{
					startJumpTime = true;
					velocity.x += 1;
					velocity.y = maxJumpVelocity;

					isClimbing = false;
					animator.SetBool("isClimbing", false);

					isJumping = true;
					animator.SetTrigger("doJumping");
					animator.SetBool("isJumping", true);
				}

				if (Input.GetKeyDown(KeyCode.C))
				{
					startJumpTime = true;
					isClimbing = false;
					animator.SetBool("isClimbing", false);
				}
			}
            else if (gravity != originGravity)
			{
				gravity = originGravity;
			}
		}
		else if (gravity != originGravity)
		{
			gravity = originGravity;
		}
	}

	void LadderCoolTime()
    {
		if (jumpTime > 0 && startJumpTime)
		{
			jumpTime -= Time.deltaTime;
		}
		else
		{
			startJumpTime = false;
			jumpTime = jumpCoolTime;
		}
	}

	void JumpAnimator()
    {
		if (velocity.y < 0 && !isAttacking && !isDashing && !isClimbing)
		{
			isJumping = true;
			animator.SetTrigger("doFalling");
			animator.SetBool("isJumping", true);
		}
		else if (velocity.y > 0 && isJumping == false && !isClimbing)
        {
			isJumping = true;
            animator.SetTrigger("doJumping");
			animator.SetBool("isJumping", true);
		}
		else if(velocity.y == 0 && isJumping == true)
        {
			isJumping = false;
			animator.SetBool("isJumping", false);
		}
	}

	public void TakeDamage(int damage, int stiffness, bool knockback = false)
    {
		hp -= damage;
		this.stiffness -= stiffness;
		//if (!knockback)
		//	rigidbody.AddForce(Vector2.zero);
		animator.SetTrigger("doHurt");
	}

	private void ActiveAttackPos()
    {
		attackPos.gameObject.SetActive(true);
	}

	private void DeActiveAttackPos()
    {
		attackPos.gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			enemy.TakeDamage(damage, 0);
		}

		if (collision.CompareTag("Ladder"))
		{
			isLadder = true;
			ladderCol = collision;
		}
		if (collision.transform.tag == "TopLadder")
		{
			topLadder = true;
		}
		if (collision.transform.tag == "BottomLadder")
		{
			bottomLadder = true;
			controller.bottomLadder = bottomLadder;
		}

		if (isSpecialAttacking && collision.transform.tag == "BrokenFloor")
		{
			brokeGround.Break();
		}
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
		if (Input.GetKeyDown(KeyCode.F))
		{
			if (collision.gameObject.CompareTag("Lever"))
			{
				UseLever lever = collision.gameObject.GetComponent<UseLever>();
				if (!lever.getFlag())
				{
					lever.SwitchFlag();
				}
			}
		}
	}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
			isClimbing = false;
			controller.isClimbing = false;
            animator.SetBool("isClimbing", false);
        }
		if (collision.transform.tag == "TopLadder")
		{
			topLadder = false;
		}
		if (collision.transform.tag == "BottomLadder")
		{
			bottomLadder = false;
			controller.bottomLadder = bottomLadder;
		}
	}
}