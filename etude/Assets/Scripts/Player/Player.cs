using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
	public GameManager manager;

	bool canMove = true;
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 8;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	Vector2 directionalInput;

	public int maxHp = 100;
	public int hp = 100;
	public int damage = 10;
	public int specialDamage = 30;
	public int chargeDamage = 50;
	public int stiffness = 0;

	[HideInInspector]
	public Animator animator;

	bool leftCanRun = false;
	float leftCheckRun = 0.2f;
	bool rightCanRun = false;
	float rightCheckRun = 0.2f;

	bool isJumping = false;
	public bool isAttacking = false;
	[HideInInspector]
	public Collider2D attackPos;
	
	bool isDashing = false;

	Vector2 playerDir;
	Vector2 dashDir;
	float dashDistance = 10f;
	float dashTime = 0.2f;
	float startTime = 0f;
	Vector2 moveAmount;

	public bool isLadder = false;
	Collider2D ladderCol;
	bool topLadder = false;
	bool bottomLadder = false;
	float originGravity;
	float climbSpeed = 8f;
	public bool isClimbing = false;
	float jumpCoolTime = 0.4f;
	float jumpTime = 0.4f;
	bool startJumpTime = false;

	public bool downJump = false;

	public bool isSpecialAttacking = false;
	public BrokeGround brokeGround;

	GameObject interactObj;

	public bool isChargeAttacking = false;

	float dashDelay = 1f;
	const float dashCoolTime = 1f;
	bool isDashReady = true;

	float speicalAttackDelay = 2.5f;
	const float speicalAttackCoolTime = 4.5f;
	public bool isSpecialAttackReady = true;

	float hAxis;
	float vAxis;

	bool jumpKey;
	bool attackKey;
	bool dashKey;


	void Start()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();

		originGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		gravity = originGravity;
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		playerDir = new Vector3(1, 0, 0);

		attackPos.gameObject.SetActive(false);
	}

	void Update()
	{
		//GetInput();
		MoveAnimator();
		JumpAnimator();
		JumpAttackAnimator();

		CoolTimer(ref speicalAttackDelay, speicalAttackCoolTime, ref isSpecialAttackReady);

		CalculateVelocity();

		LadderCoolTime();
		LadderClimb();
		UpdateDash();

		Movement();
	}

	void GetInput()
    {
		hAxis = Input.GetAxisRaw("Horizontal");
		vAxis = Input.GetAxisRaw("Vertical");
		attackKey = Input.GetKeyDown(KeyCode.Z);
		jumpKey = Input.GetKeyDown(KeyCode.X);
		dashKey = Input.GetKeyDown(KeyCode.C);
	}

	private void FixedUpdate()
    {
		Debug.DrawRay(transform.position, playerDir * 5f, new Color(0, 1, 0));
		RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDir, 5f, LayerMask.GetMask("Interact")); 

		if (hit.collider != null)
        {
			interactObj = hit.collider.gameObject;
        }
		else
        {
			interactObj = null;
        }
    }

	void CoolTimer(ref float delay, float coolTime, ref bool isReady)
    {
		delay += Time.deltaTime;
		isReady = coolTime < delay;
    }

	void ChargeAttackOn()
    {
		isChargeAttacking = true;
    }

	void ChargeAttackOff()
    {
		isChargeAttacking = false;
	}

	public void SetDirectionalInput(Vector2 input)
	{
		directionalInput = input;
	}

	public void OnJumpInputDown()
	{
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

	public void Interact()
    {
		if (interactObj != null)
		{
			if (interactObj.name == "Lever")
			{
				UseLever useLever = interactObj.GetComponent<UseLever>();
				if (!useLever.getFlag())
				{
					useLever.SwitchFlag();
				}
			}

			if (interactObj.tag == "NPC")
			{
				manager.Action(interactObj);
				
			}
		}
	}

	public void Dash()
	{
		if (isDashReady)
		{
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("player_dash_1"))
			{
				animator.SetTrigger("doDash");
				attackPos.gameObject.SetActive(false);
				canMove = false;
				isDashing = true;
				dashDir = playerDir;
				startTime = Time.time;
				dashDelay = 0;
			}
		}
	}

	void Movement()
    {
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

	void UpdateDash()
    {
		CoolTimer(ref dashDelay, dashCoolTime, ref isDashReady);

		if (isDashing)
		{
			float progress = (Time.time - startTime) / dashTime;
			progress = Mathf.Clamp(progress, 0, 1);
			moveAmount = new Vector3(dashDistance, 0, 0) * progress * dashDir.x;
			controller.Move(moveAmount * 10 * Time.deltaTime, directionalInput);

			if (progress >= 1)
			{
				isDashing = false;
				canMove = true;
				transform.position = new Vector2(transform.position.x - (0.1f * dashDir.x), transform.position.y);
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
			canRun = true; 
		}
		if (canRun)
		{
			checkRun -= Time.deltaTime;
			if (checkRun <= 0)
			{
				canRun = false;  
				checkRun = 0.2f; 
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
	
	public void TakeDamage(int damage, int stiffness)
    {
		hp -= damage;
		this.stiffness -= stiffness;
		animator.SetTrigger("doHurt");
	}

	public void TakeDamage(int damage, int stiffness, Vector2 enemyPos, Vector2 knockback)
    {
		hp -= damage;
		this.stiffness -= stiffness;
		int knockbackDir = 0; 
		if(transform.position.x - enemyPos.x <= 0)
        {
			knockbackDir = -1;
        }
        else
        {
			knockbackDir = 1;
        }
		velocity.x = knockbackDir * knockback.x;
		velocity.y = knockback.y;
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