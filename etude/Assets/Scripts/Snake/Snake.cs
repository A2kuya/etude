using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy
{
    RaycastHit2D raycast;
	public float maxJumpHeight = 4;
	public float timeToJumpApex = 3f;
    public float gravity;       //중력

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        playerAttackLayer = LayerMask.GetMask("PlayerAttackLayer");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        //controller = GetComponent<Controller2D>();
        atkCollider.SetActive(false);
        dir = Vector2.left;
        hp = 100;
        damage = 10;
        range = 3f;
        attackPattern = new List<AttackPattern>();
        attackPattern.Add(new AttackPattern(3, 0));
        chaseDistance = 12f;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        
    }

    // Update is called once per frame
    void Update()
    {
        GetSpriteSize();
        DontSlide();
        CheckObstacle();
        CaculateDistance(); //거리 계산 및 방향판정
        ManageCoolTime();   //쿨타임 관리
    }

    override public bool Detect()
    {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position + Vector3.up, transform.right * -1, detectDistance, playerLayer);
        return rayhit.collider != null;
    }
    public override void Movement()
    {
        Move(isLeft);


        // velocity.y += gravity * Time.deltaTime;
        // velocity.x = v.x * speed * Time.fixedDeltaTime * -1;
        // dir.y = -1;
        // controller.Move(velocity, dir);
        // if (controller.collisions.above || controller.collisions.below)
		// {
		// 	if (controller.collisions.slidingDownMaxSlope)
		// 	{
		// 		velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
		// 	}
		// 	else
		// 	{
		// 		velocity.y = 0;
		// 	}
		// }        
    }
    public override void Attack()
    {
        if (attackPattern[0].curtime <= 0f && InRange())
        {
            anim.SetTrigger("isAtk");
            attackPattern[0] = new AttackPattern(attackPattern[0].cooltime, attackPattern[0].cooltime);
        }
        else if(Miss() || InRange()){
            anim.SetBool("isWalk", false);
        }
        else{
            anim.SetBool("isWalk", true);
        }
    }
    public override bool Miss()
    {
        return playerDistance > chaseDistance;
    }
    public bool InRange()
    {
        if(playerDistance < range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.1f, obstacleLayer);
        //경사 체크
        Vector2 frontPosition = new Vector2(transform.position.x, transform.position.y + 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1, obstacleLayer);
        RaycastHit2D fronthit = Physics2D.Raycast(frontPosition, (isLeft ? Vector2.left : Vector2.right), 0.1f + spriteSize.x /2, obstacleLayer);
        Debug.DrawRay(frontPosition, (isLeft ? Vector2.left : Vector2.right), Color.red);
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if(fronthit){
            CheckSlope(fronthit);
        }else if(hit){
            CheckSlope(hit);
        }
    }
    
}
