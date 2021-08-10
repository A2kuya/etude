using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
	// Sound
	public AudioClip clip;
	public AudioClip[] audioClips;

	// State()
	private Collider2D col;
	public Collider2D attackPos;
	private Controller2D controller;
	private Rigidbody2D rb;
	private Animator animator;
	public HealthBar healthBar;
	public StaminaBar staminaBar;

	// Key
	public bool canGetKey = true;
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
	private bool SkilltreeKey;

	Vector2 directionalInput;

	// State
	private enum State { idle, walk, run, jump, fall, dash, attack, jumpAttack, specialAttack, charge, chargeAttack, climb, dead }
	private State state = State.idle;
	public int maxHp = 100;
	public int hp = 100;
	public int damage = 10;
	public int specialDamage = 30;
	public int chargeDamage = 50;
	public float stamina = 100;
	private bool isExhaust = false;

	//inventory
	public int money = 0;
	public int skillPoint = 0;

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
	bool isCoroutineRunnung = false;

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
	public float dashStaminaCost = 20f;

	// Combo Attack
	private bool[] attacks = new bool[2];
	private bool isAttackReady;
	private float attackDelay;
	public float attackCoolTime;

	// Charge Attack
	private float chargeAttackTime = 0;
	private float chargeAttackMinTime = 1;
	public float chargeAttackStaminaCostPerSec = 30f;

	// Special Attack
	public float speicalAttackCoolTime;
	float speicalAttackDelay;
	private bool isSpecialAttackReady = true;
	public float speicalAttackStaminaCost = 40f;

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

	// Hurt
	public float hurtTime;

	// Interact
	GameObject interactObj;
	public InteractManager manager;

	// Heal
	public PotionUI potionUI;
	public int potions = 5;
	public int healAmount;

	// UnBeat
	private bool isUnBeat = false;
	public float unBeatTime;

	// DoubleJump
	private bool canDoubleJump = true;

	// GameOver
	public GameObject gameOver;

	void Start()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
		col = GetComponent<Collider2D>();
		rb = GetComponent<Rigidbody2D>();

		healthBar.SetMaxHealth(maxHp);

		originGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		gravity = originGravity;
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		playerDir = new Vector3(1, 0, 0);

		attackPos.gameObject.SetActive(false);

		dashDelay = dashCoolTime;
		speicalAttackDelay = speicalAttackCoolTime;
		attackDelay = attackCoolTime;

        StartCoroutine("RecoverStamina");

		potionUI.SetPotion();
	}

	void Update()
	{
		InputManager();
		if (stamina <= 0)
        {
			StartCoroutine("Exhaust");
		}

		Move();
		Jump();
		Dash();
		Attack();
		LadderClimb();
		Interact();
		UsePotion();
		SkillTree();

		StateManager();
		animator.SetInteger("state", (int)state);

		CalculateVelocity();
		staminaBar.SetStamina(stamina);
	}

	void InputManager()
	{
		if (canGetKey)
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
			SkilltreeKey = Input.GetKeyDown(KeyCode.S);
		}

		else
		{
			hAxis = 0;
			vAxis = 0;
			attackKeyDown = false;
			attackKeyUp = false;
			attackKey = false;
			jumpKeyDown = false;
			jumpKeyUp = false;
			dashKey = false;
			interactKey = false;
			HealKey = false;
			SkilltreeKey = false;
		}

		directionalInput = new Vector2(hAxis, vAxis);
	}

	void StateManager()
	{
		// Priority = dash -> climb -> fall -> attack -> jump -> run -> walk -> idle
		if (state == State.dead)
        {
			return;
        }

		else if (state == State.dash)
		{
			InitAttack();
			animator.Play("player_dash_blend_tree");
		}

		else if (state == State.climb)
		{
			return;
		}

		else if (velocity.y < -1f)
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

			if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_jump") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
			{
				state = State.fall;
			}
		}

		else if (state == State.fall)
		{
			if (controller.collisions.below)
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
					if (stamina > chargeAttackStaminaCostPerSec / 100)
					{
						state = State.charge;
						StartCoroutine("SpendStaminaGradually", chargeAttackStaminaCostPerSec);
					}
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
			{
				state = State.run;
				if (!isCoroutineRunnung)
				{
					isCoroutineRunnung = true;
					StartCoroutine("SpendStaminaGradually", 15f);
				}
			}
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
			if (state != State.dash)
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
		if (state != State.attack && state != State.jumpAttack && state != State.dash)
		{
			if (jumpKeyDown)
			{
				if (state == State.climb)
				{
					isClimbReady = false;
					climbDelay = 0;
					velocity.y = maxJumpVelocity;
					state = State.jump;
					return;
				}

				if (vAxis == -1)
				{
					downJump = true;
					state = State.fall;
				}
                else
                {
					OnJumpInputDown();
					state = State.jump;
				}
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
			canDoubleJump = true;
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
		else if (SkillManager.Instance.IsSkillUnlocked(SkillManager.SkillType.DoubleJump, 1) && canDoubleJump)
        {
			canDoubleJump = false;
			state = State.jump;
			animator.Play("player_idle");
			animator.Play("player_jump");
			velocity.y = maxJumpVelocity;
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
		if (SkillManager.Instance.IsSkillUnlocked(SkillManager.SkillType.Dash, 1))
		{
			if (dashKey)
			{
				if (isDashReady)
				{
					if (stamina > 0)
					{
                        if (SkillManager.Instance.IsSkillUnlocked(SkillManager.SkillType.Dash, 5))
                        {
                            animator.SetFloat("dashNum", 1);
                        }
                        state = State.dash;
						canGetKey = false;
						dashDir = playerDir;
						startTime = Time.time;
						dashDelay = 0;
						attackPos.gameObject.SetActive(false);
						StartCoroutine("SpendStamina", dashStaminaCost);
					}
				}
			}

			CoolTimer(ref dashDelay, ref isDashReady, dashCoolTime - 0.5f * SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.Dash]);

			if (state == State.dash)
			{
				velocity = Vector3.zero;

				float progress = (Time.time - startTime) / dashTime;
				progress = Mathf.Clamp(progress, 0, 1);
				moveAmount = new Vector3(dashDistance, 0, 0) * progress * dashDir.x;
				controller.Move(moveAmount * 10 * Time.deltaTime, directionalInput);
                if (SkillManager.Instance.IsSkillUnlocked(SkillManager.SkillType.Dash, 5))
                {
                    StartCoroutine("UnBeatable", dashTime);
                }
                if (progress >= 1)
				{
					if (!isExhaust)
					{
						canGetKey = true;
					}
					state = State.idle;
					transform.position = new Vector2(transform.position.x - (0.1f * dashDir.x), transform.position.y);
				}
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
					if (SkillManager.Instance.IsSkillUnlocked(SkillManager.SkillType.SpecialAttack, 1))
					{
						if (isSpecialAttackReady && state == State.idle)
						{
							if (stamina > 0)
							{
								state = State.specialAttack;
								specialDamage = 10 * SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.SpecialAttack];
								speicalAttackDelay = 0f;
								isSpecialAttackReady = false;
								StartCoroutine("SpendStamina", speicalAttackStaminaCost);
							}
						}
					}
				}

				else
				{
					if (!attacks[0] && !attacks[1] && isAttackReady)
					{
						state = State.attack;
						attacks[0] = true;
					}
					else if (attacks[0])
					{
						state = State.attack;
						attacks[1] = true;
						attackDelay = 0f;
						isAttackReady = false;
					}
				}
			}
			else if (state != State.climb && bottomLadder == false && topLadder == false)
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
				if (directionalInput.y < 0 && bottomLadder || directionalInput.y > 0 && topLadder)
				{
					if (state == State.climb)
					{
						state = State.idle;
					}
					return;
				}
				state = State.climb;
				canMove = true;
			}

			if (state == State.climb)
			{
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

	private void SkillTree()
	{
		if(SkilltreeKey)
		{
			SkillManager.Instance.Active(ref skillPoint);
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

	public void TakeDamage(int damage)
    {
		if (!isUnBeat)
		{
			hp -= damage;
			healthBar.SetHealth(hp);
			StartCoroutine("hurtEffector");
			if (hp <= 0)
			{
				Dead();
			}
		}
	}

	public void TakeDamage(int damage, Vector2 enemyPos, Vector2 knockback)
	{
		if (!isUnBeat)
		{
            hp -= damage;
			healthBar.SetHealth(hp);
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
			state = State.idle;
			InitAttack();
			canMove = false;
			StartCoroutine("hurtEffector");
			if (hp <= 0)
			{
				Dead();
			}
		}
	}

	private IEnumerator hurtEffector()
    {
		animator.Play("player_hurt");
		animator.SetBool("isHurt", true);
		state = State.idle;
		InitAttack();
		canMove = true;
		canGetKey = false;
		StartCoroutine("UnBeatable", unBeatTime);

		yield return new WaitForSeconds(hurtTime);
	}

	private IEnumerator UnBeatable(float time)
    {
		isUnBeat = true;
		yield return new WaitForSeconds(time);
		animator.SetBool("isHurt", false);
		isUnBeat = false;
		if (state != State.dead && !isExhaust)
		{
			canGetKey = true;
		}
	}

	private void UsePotion()
    {
		if (HealKey)
		{
			if (potions > 0)
			{
				potions -= 1;
				potionUI.SetPotion();
				Heal(healAmount);
			}
		}
    }

	public void Heal(int healAmount)
    {
		hp += healAmount;
		if (hp > 100)
			hp = 100;
		healthBar.SetHealth(hp);
	}

	public void SpendMoney(int amount)
    {
		money -= amount;
    }

	public void GetMoney(int amount)
    {
		money += amount;
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

	public void PlayAttack1Sound()
    {
		SoundManager.instance.SFXPlay("Attack1", audioClips[0]);
    }

	public void PlayAttack2Sound()
	{
		SoundManager.instance.SFXPlay("Attack2", audioClips[1]);
	}

	private IEnumerator SpendStamina(float staminaCost)
	{
		float MaxStamina = 0f;
		if (stamina <= 0)
		{
			StopCoroutine("SpendStamina");
		}
		else
		{
			while (MaxStamina < staminaCost)
			{
				stamina -= 1f;
				MaxStamina += 1f;
				yield return new WaitForSeconds(0.01f);
			}
		}
	}

	private IEnumerator SpendStaminaGradually(float staminaCostPerSec)
	{
		while (state == State.charge || state == State.run)
		{
			if (stamina <= 0)
            {
				state = State.idle;
				StopCoroutine("SpendStaminaGradually");
				break;
            }
			stamina -= staminaCostPerSec / 100;
			yield return new WaitForSeconds(0.01f);
		}
		isCoroutineRunnung = false;
	}

	private IEnumerator RecoverStamina()
    {
		while (stamina < 100)
		{
			stamina = Mathf.Clamp(stamina + 1f, 0, 100);
			yield return new WaitForSeconds(0.1f);
		}
		yield return null;
		yield return StartCoroutine("RecoverStamina");
	}


	private IEnumerator Exhaust()
	{
		if (!isExhaust)
		{
			canGetKey = false;
			isExhaust = true;
			yield return new WaitForSeconds(1f);

			canGetKey = true;
			isExhaust = false;
		}
	}

	private void Dead()
    {
		animator.Play("player_dead");
		state = State.dead;
		col.enabled = false;
		canGetKey = false;
		gameOver.SetActive(true);
    }
}