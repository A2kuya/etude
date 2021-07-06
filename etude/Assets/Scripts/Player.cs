using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	public Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;

	public int hp;
	int damage;

	public Animator animator;
	public Rigidbody2D rigidbody;

	bool leftCanRun = false;
	float leftCheckRun = 0.2f;
	bool rightCanRun = false;
	float rightCheckRun = 0.2f;

	bool isJumping = false;
	bool canMove = true;

	public 

	void Start()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
		rigidbody = GetComponent<Rigidbody2D>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	void Update()
	{
		MoveAnimator();
		JumpAnimator();
		CalculateVelocity();

		if(animator.GetCurrentAnimatorStateInfo(0).IsName("player_attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.1f)
        {
			canMove = true;
        }

		if (canMove)
		{
			controller.Move(velocity * Time.deltaTime, directionalInput);
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

	public void Attack()
	{
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("player_attack") && isJumping == false)
		{
			animator.SetTrigger("doAttack");
			canMove = false;
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
			sr.flipX = true;
		}
		else if (Input.GetAxisRaw("Horizontal") > 0)
		{
			animator.SetBool("isMoving", true);
			sr.flipX = false;
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

	void JumpAnimator()
    {
		if (velocity.y < 0)
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
	public void TakeDamage(int damage, Transform damageTransform)  //피격
    {
        hp -= damage;
		//넉백 코드 필요
    }

}