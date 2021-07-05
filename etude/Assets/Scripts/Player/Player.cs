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

	int hp;
	int damage;

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
	Vector3 startPos;
	Vector2 moveAmount;

	void Start()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		playerDir = new Vector3(1, 0, 0);
	}

	void Update()
	{
		MoveAnimator();
		JumpAnimator();
		JumpAttackAnimator();
		CalculateVelocity();

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
		if (canMove)
			controller.Move(velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below)
		{
			if (controller.collisions.slidingDownMaxSlope)
			{
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			}
			else
			{
				velocity.y = 0;
				print(velocity.y);
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

	public void Attack()
	{
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("player_attack"))
		{
			moveSpeed = 1f;
			isAttacking = true;
			animator.SetTrigger("doAttack");
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
			startPos = transform.position;
		}
	}

	void JumpAttackAnimator()
    {
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_attack"))
		{
			if (Math.Truncate(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * 10) / 10 == 0.5f && (isJumping || velocity.y < 0))
			{
				velocity.y -= 2f;
				animator.enabled = false;
			}
			if (isJumping == false)
			{
				animator.enabled = true;
			}
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
			canRun = true; // 달리는 판정을 true로 바꿔준다.
		}
		if (canRun)
		{
			checkRun -= Time.deltaTime;
			if (checkRun <= 0)
			{
				canRun = false;  // 달리는 판정을 false로 바꿔준다.
				checkRun = 0.2f; // 그리고 checkRun의 시간은 0.2로 돌려준다.
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

	void JumpAnimator()
    {
		if (velocity.y < 0 && !isAttacking && !isDashing)
		{
			animator.SetTrigger("doFalling");
		}
        else if (velocity.y > 0 && isJumping == false)
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
}