using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture : Boss
{
    public GameObject prfStone;
    public phase state;
    public enum phase{ first, second, third }

    public float fallingSotnesCooltime;
    public int fallingStonesDamage;
    public float cutAirCooltime;
    public int cutAirDamage;
    public float shootRayCooltime;
    public int shootRayDamage;
    
    public float attackCooltime;
    public bool isAttack;
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        hpBar = Instantiate(prfHpBar, canvas.transform);
        attackPatterns = new Dictionary<string, AttackPattern>();
        attackPatterns.Add("fallStones", new FallStones(fallingStonesDamage, fallingSotnesCooltime, this));
        attackPatterns.Add("cutAir", new CutAir(cutAirDamage, cutAirCooltime, this));
        attackPatterns.Add("shootRay", new ShootRay(shootRayDamage, shootRayCooltime, this));
        state = phase.first;
        GetSpriteSize();
        StartCoroutine(Attack());
    }

    void Update()
    {
        UpdateHpBar();
        CheckDead();
        CaculateDistance();
        CheckObstacle();
        
    }

    IEnumerator Attack(){
        if (!isAttack)
        {
            yield return new WaitForSeconds(1f);
            if (CheckPhase())
                anim.SetTrigger("changePhase");
            else if (attackPatterns["fallStones"].Can())
                attackPatterns["fallStones"].Excute();
            else if (attackPatterns["shootRay"].Can())
                attackPatterns["shootRay"].Excute();
            else if (attackPatterns["cutAir"].Can())
                attackPatterns["cutAir"].Excute();
            yield return new WaitForSeconds(attackCooltime - 1f);
        }
        else
            yield return new WaitForSeconds(1f);
    }
    IEnumerator Move(){
        WaitForSeconds wait = new WaitForSeconds(3f);
        while (true)
        {
            if(!isMoving){
                Vector3 amount = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
                if (CanMove(amount))
                {
                    isMoving = true;
                    iTween.MoveBy(gameObject, iTween.Hash("amount", amount, "easeType", iTween.EaseType.easeOutExpo, "time", 2f, "oncomplete", "IsNotMove"));
                }
                else{
                    continue;
                }
                yield return wait;
            }else{
                yield return wait;
            }
        } 
    }
    public bool CanMove(Vector3 amount){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, amount, amount.magnitude, obstacleLayer);
        Debug.DrawRay(transform.position, amount, Color.blue);
        if(hit)
            return false;
        else
            return true;
    }
    public void IsNotMove(){
        isMoving = false;
    }
    public Vector3 left;
    public Vector3 right;
    public int roundTripCount;
    private int count;
    public void FallStoneStart(){
        Debug.Log("start");
        count = 0;
        anim.speed = 1;
        isMoving = false;
        Flip();
    }
    public void FallStone(){
        if(!isMoving){
            if(count >= roundTripCount)
                FallStoneEnd();
            else{
                isMoving = true;
                count++;
                iTween.MoveTo(gameObject, iTween.Hash("position", (isLeft ? left : right), "easeType", iTween.EaseType.linear, "speed", 50f, "oncomplete", "Turn"));
            }
        }else{
            if(Random.Range(0f, 1f) <= 0.03 && count > 0)
                Instantiate(prfStone, transform.position, transform.rotation);
        }
    }
    public void Turn(){
        isLeft = !isLeft;
        isMoving = !isMoving;
        Flip();
    }
    public void FallStoneEnd(){
        count = 0;
        anim.speed = 1;
        isMoving = false;
        anim.SetTrigger("rest");
    }



    public bool CheckPhase(){
        if((float) curHp/hp <= 0.2 && state != phase.third){
            state = phase.third;
            return true;
        }
        else if((float) curHp/hp <= 0.5 && state == phase.first){
            state = phase.second;
            return true;
        }
        return false;
    }
    public override void Death()
    {
        StopCoroutine(Attack());
        gameObject.tag = "Untagged";
        Destroy(this);
        base.Death();
    }
    override public void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
    }
    override public void Flip(bool reverse = false){
        bool dir = (reverse ? !isLeft : isLeft);
        if (dir)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
