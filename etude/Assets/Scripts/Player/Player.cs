using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
	public GameManager manager;

	// State()
	private Collider2D col;
	public Collider2D attackPos;
	private Controller2D controller;
    private Rigidbody2D rb;
	private Animator animator;

	// Key
	private float hAxis;
	private float vAxis;
	private bool jumpKeyDown;
	private bool jumpKeyUp;
	private bool attackKeyDown;
	private bool attackKeyUp;
	private bool attackKey;
	private bool dashKey;
	private bool interactKey;
	private bool HealKey;
	Vector2 directionalInput;

	// State
	private enum State { idle, walk, run, jump, fall, dash, attack, jumpAttack, specialAttack, charge, chargeAttack, climb }
	private State state = State.idle;
	public int maxHp = 100;
	public int hp = 100;
	public int damage = 10;
	public int specialDamage = 30;
	public int chargeDamage = 50;
	private int stiffness = 0;

	// Move
	bool canMove = true;
	public Vector3 velocity;
	public float moveSpeed = 8;
	Vector2 playerDir;

	// Run
	bool leftCanRun = false;
	float leftCheckRun = 0.2f;
	bool rightCanRun = false;
	float rightCheckRun = 0.2f;

	// Jump
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;
	public bool downJump = false;

	// Dash
	public float dashCoolTime;
	float dashDelay;
	bool isDashReady = true;
	Vector2 dashDir;
	float dashDistance = 10f;
	float dashTime = 0.2f;
	float startTime = 0f;
	Vector2 moveAmount;

	// Combo Attack
	private bool[] attacks = new bool[2];
	private bool isAttackReady;
	private float attackDelay;
	public float attackCoolTime;

	// Charge Attack
	private float chargeAttackTime = 0;
	private float chargeAttackMinTime = 1;

	// Special Attack
	public float speicalAttackCoolTime;
	float speicalAttackDelay;
	private bool isSpecialAttackReady = true;

	// Ladder Climb
	bool isLadder = false;
	Collider2D ladderCol;
	bool topLadder = false;
	bool bottomLadder = false;
	float originGravity;
	float climbSpeed = 8f;

	float climbDelay = 0.4f;
	const float climbCoolTime = 0.4f;
	bool isClimbReady = false;

	// Interact
	GameObject interactObj;

	// Heal
	private int potions = 5;
	public int healAmount;

	// UnBeat
	private bool isUnBeat = false;
	public float unBeatTime;

	void Start()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
		col = GetComponent<Collider2D>();
		rb = GetComponent<Rigidbody2D>();

		originGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		gravity = originGravity;
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		playerDir = new Vector3(1, 0, 0);

		attackPos.gameObject.SetActive(false);

		dashDelay = dashCoolTime;
		speicalAttackDelay = speicalAttackCoolTime;
		attackDelay = attackCoolTime;
	}

	void Update()
	{
		InputManager();

		Move();
		Jump();
		Dash();
		Attack();
		LadderClimb();
		Interact();
		Heal();

		StateManager();

		animator.SetInteger("state", (int)state);

		CalculateVelocity();
	}

	void InputManager()
    {
		hAxis = Input.GetAxisRaw("Horizontal");
		vAxis = Input.GetAxisRaw("Vertical");
		attackKeyDown = Input.GetKeyDown(KeyCode.Z);
		attackKeyUp = Input.GetKeyUp(KeyCode.Z);
		attackKey = Input.GetKey(KeyCode.Z);
		jumpKeyDown = Input.GetKeyDown(KeyCode.X);
		jumpKeyUp = Input.GetKeyUp(KeyCode.X);
		dashKey = Input.GetKeyDown(KeyCode.C);
		interactKey = Input.GetKeyDown(KeyCode.F);
		HealKey = Input.GetKeyDown(KeyCode.A);

		directionalInput = new Vector2(hAxis, vAxis);
	}

	void StateManager()
    {
		// Priority = dash -> climb -> fall -> attack -> jump -> run -> walk -> idle
		if (state == State.dash)
        {
			InitAttack();
			animator.Play("player_dash");
		}

		else if (state == State.climb)
        {
			return;
        }

		else if (velocity.y < 0f)
		{
			if (state == State.jumpAttack)
            {
				velocity.y -= .5f;
			}
			else
            {
				state = State.fall;
			}
		}

		else if (state == State.jump)
        {
			if (velocity.y < 0f)
			{
				state = State.fall;
			}
		}

		else if (state == State.fall)
        {
			if(controller.collisions.below)
            {
				state = State.idle;
				InitAttack();
			}
		}

		else if (state == State.jumpAttack)
        {
			if (controller.collisions.below)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_jump_attack_transition") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
				{
					state = State.idle;
				}
				else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("player_jump_attack_transition"))
				{
					animator.SetTrigger("exitJumpAttack");
				}
			}
			
		}

		else if (state == State.specialAttack)
        {
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_special_attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
				state = State.idle;
            }
		}

		else if (state == State.charge)
        {
			moveSpeed = 5f;
			if (attackKeyUp)
			{
				if (chargeAttackTime > chargeAttackMinTime)
				{
					state = State.chargeAttack;
				}
				else
                {
					state = State.idle;
				}
				chargeAttackTime = 0;
				moveSpeed = 8f;
			}
		}

		else if (state == State.chargeAttack)
        {
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_charge_attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
				state = State.idle;
			}
		}

		else if (state == State.attack)
        {
			moveSpeed = 1f;
			if (attacks[0] && !attacks[1])
            {
				animator.Play("player_attack_1");
			}
			
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_attack_1") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
			{
				attacks[0] = false;

				if (attacks[1])
                {
					animator.Play("player_attack_2");
				}

				else if (chargeAttackTime > .3f)
                {
					state = State.charge;
                }

				else
                {
					state = State.idle;
					moveSpeed = 8f;
				}
			}

			else if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_attack_2") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
			{
				attacks[1] = false;

				if (chargeAttackTime > .3f)
				{
					state = State.charge;
				}

				else
				{
					state = State.idle;
					moveSpeed = 8f;
				}
			}
		}

		else if (Mathf.Abs(hAxis) > 0f)
		{
			if (Mathf.Abs(velocity.x) > 8f)
				state = State.run;
			else
				state = State.walk;
		}

		else
        {
			state = State.idle;
        }
    }

	private void Move()
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

		// Moving Left
		if (hAxis < 0)
		{
			transform.localScale = new Vector2(-10, 10);
			playerDir = new Vector3(-1, 0, 0);
		}

		// Moving Right
		else if (hAxis > 0)
		{
			transform.localScale = new Vector2(10, 10);
			playerDir = new Vector3(1, 0, 0);
		}

		SetRun(KeyCode.LeftArrow, ref leftCanRun, ref leftCheckRun);
		SetRun(KeyCode.RightArrow, ref rightCanRun, ref rightCheckRun);
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
		if (Input.GetKey(key) && canRun == false || hAxis == 0)
		{
			moveSpeed = 8;
		}
		else if (Input.GetKey(key) && canRun == true)
		{
			moveSpeed = 16;
			leftCanRun = true;
			rightCanRun = true;
			leftCheckRun = 0.1f;
			rightCheckRun = 0.1f;
		}
		if (Input.GetKeyUp(key))
		{
			moveSpeed = 8;
		}
	}

	void CalculateVelocity()
	{
		if (state != State.climb)
		{
			float targetVelocityX = directionalInput.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
			velocity.y += gravity * Time.deltaTime;
		}
	}

	private void Jump()
    {
		if (state != State.attack && state != State.jumpAttack  && state != State.dash)
		{
			if (jumpKeyDown)
			{
				if (state == State.climb)
                {
					isClimbReady = false;
					climbDelay = 0;
					velocity.x += 1;
					velocity.y = maxJumpVelocity;
					state = State.jump;
					return;
				}

				if (vAxis == -1)
				{
					print(123);
					downJump = true;
				}
				OnJumpInputDown();
				state = State.jump;
			}
			if (jumpKeyUp)
			{
				OnJumpInputUp();
			}
		}
	}

	// JumpKey Down
	private void OnJumpInputDown()
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

			else if (downJump)
            {
				return;
            }

			else
			{
				velocity.y = maxJumpVelocity;
			}
		}
	}

	// JumpKey Up
	private void OnJumpInputUp()
	{
		if (velocity.y > minJumpVelocity)
		{
			velocity.y = minJumpVelocity;
		}
	}

	private void Dash()
	{
		if (dashKey)
		{
			if (isDashReady)
			{
				{
					state = State.dash;
					canMove = false;
					dashDir = playerDir;
					startTime = Time.time;
					dashDelay = 0;
					attackPos.gameObject.SetActive(false);
				}
			}
		}

		CoolTimer(ref dashDelay, ref isDashReady, dashCoolTime);

		if (state == State.dash)
		{
			velocity = Vector3.zero;

			float progress = (Time.time - startTime) / dashTime;
			progress = Mathf.Clamp(progress, 0, 1);
			moveAmount = new Vector3(dashDistance, 0, 0) * progress * dashDir.x;
			controller.Move(moveAmount * 10 * Time.deltaTime, directionalInput);
			if (progress >= 1)
			{
				canMove = true;
				state = State.idle;
				transform.position = new Vector2(transform.position.x - (0.1f * dashDir.x), transform.position.y);
			}
		}
	}

	void CoolTimer(ref float delay, ref bool isReady, float coolTime)
	{
		if (!isReady)
		{
			delay += Time.deltaTime;
		}
		isReady = coolTime < delay;
	}

	private void InitAttack()
    {
		attacks[0] = false;
		attacks[1] = false;
	}

	private void Attack()
    {
		CoolTimer(ref attackDelay, ref isAttackReady, attackCoolTime);
		CoolTimer(ref speicalAttackDelay, ref isSpecialAttackReady, speicalAttackCoolTime);

		if (attackKeyDown)
        {
			if (controller.collisions.below)
            {
				if (vAxis == -1)
				{
					if (isSpecialAttackReady && state == State.idle)
					{
						state = State.specialAttack;
						speicalAttackDelay = 0f;
						isSpecialAttackReady = false;
					}
				}

				else if (isAttackReady)
				{
					state = State.attack;
					if (!attacks[0] && !attacks[1])
					{
						attacks[0] = true;
					}
					else if (attacks[0])
					{
						attacks[1] = true;
						attackDelay = 0f;
						isAttackReady = false;
					}
				}
			}
			 
			else
            {
				state = State.jumpAttack;
				velocity.y = 0;
			}
		}

		if (attackKey && (state == State.attack || state == State.charge))
        {
			if (controller.collisions.below)
			{
				chargeAttackTime += Time.deltaTime;
			}
        }

		if (attackKeyUp)
		{
			if (chargeAttackTime < chargeAttackMinTime)
			{
				chargeAttackTime = 0;
			}
		}
    }

	public String GetState()
    {
		return state.ToString();
    }

	private void ActiveAttackPos()
	{
		attackPos.gameObject.SetActive(true);
	}

	private void DeActiveAttackPos()
	{
		attackPos.gameObject.SetActive(false);
	}

	void LadderClimb()
	{
		CoolTimer(ref climbDelay, ref isClimbReady, climbCoolTime);

		if (isLadder && state != State.attack && state != State.jumpAttack)
		{
			if (vAxis != 0 && isClimbReady)
			{
				state = State.climb;
				canMove = true;
			}

			if (state == State.climb)
			{
				if (directionalInput.y < 0 && bottomLadder || directionalInput.y > 0 && topLadder)
				{
					state = State.idle;
					return;
				}

				transform.position = new Vector3(ladderCol.transform.position.x, transform.position.y);
				velocity.x = 0;
				velocity.y = 0;
				gravity = 0;

				if (!animator.GetCurrentAnimatorStateInfo(0).IsName("player_climb") && !animator.GetCurrentAnimatorStateInfo(0).IsName("player_hurt"))
				{
					animator.Play("player_climb");
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
					if (topLadder)
					{
						controller.isClimbing = true;
					}
					velocity.y = directionalInput.y * climbSpeed;
					animator.SetFloat("climbSpeed", 1);
				}

				if (dashKey)
				{
					isClimbReady = false;
					climbDelay = 0;
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

	private void Interact()
	{
		if (interactObj != null)
		{
			if (interactKey)
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

	public void TakeDamage(int damage, int stiffness)
    {
		if (!isUnBeat)
		{
			hp -= damage;
			this.stiffness -= stiffness;
			animator.Play("player_hurt");
			StartCoroutine("UnBeatable");
		}
	}

	public void TakeDamage(int damage, int stiffness, Vector2 enemyPos, Vector2 knockback)
	{
		if (!isUnBeat)
		{
			hp -= damage;
			this.stiffness -= stiffness;
			int knockbackDir = 0;
			if (transform.position.x - enemyPos.x <= 0)
			{
				knockbackDir = -1;
			}
			else
			{
				knockbackDir = 1;
			}
			velocity.x = knockbackDir * knockback.x;
			velocity.y = knockback.y;
			animator.Play("player_hurt");
			StartCoroutine("UnBeatable");
		}
	}

	private IEnumerator UnBeatable()
    {
		isUnBeat = true;
		yield return new WaitForSeconds(unBeatTime);
		isUnBeat = false;
    }

	private void Heal()
    {
		if (HealKey)
		{
			if (potions > 0)
			{
				potions -= 1;
				hp += healAmount;
			}
		}
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