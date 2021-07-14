using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHyena : Boss
{
    public GameObject prfHyena;
    List<GameObject> hyenas;
    Transform summons;
    public phase state;
    public enum phase{ first, second, third }
    public float dashDistance;
    public int biteDamage;
    public float biteCooltime;
    public float summonCooltime;
    public int rushDamage;
    public float rushCooltime;
    public float backAttackCooltime;
    public float attackCooltime;
    private bool isDash;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        summons = GameObject.Find("Summons").transform;
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        hpBar = Instantiate(prfHpBar, canvas.transform);
        attackPatterns = new Dictionary<string, AttackPattern>();
        attackPatterns.Add("bite", new Bite(biteDamage, biteCooltime, this));
        attackPatterns.Add("summon", new Summon(0, summonCooltime, this));
        attackPatterns.Add("rush", new Rush(rushDamage, rushCooltime, this));
        attackPatterns.Add("backAttack", new BackAttack(0, backAttackCooltime, this));
        state = phase.first;
        GetSpriteSize();
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHpBar();
        CheckDead();
        CaculateDistance();
        CheckObstacle();
        ManagePhase();
    }
    public void Movement(){
        Move((isLeft?Vector2.left:Vector2.right), walkSpeed);
    }
    private bool isAttack;
    public void InAttack(bool b){
        isAttack = b;
    }
    IEnumerator Attack(){
        while (gameObject.activeSelf)
        {
            if(!isAttack){
                yield return new WaitForSeconds(attackCooltime);
                if (state >= phase.third && attackPatterns["backAttack"].Can())
                    attackPatterns["backAttack"].Excute();
                else if (state >= phase.second && attackPatterns["rush"].Can())
                    attackPatterns["rush"].Excute();
                else if (attackPatterns["summon"].Can())
                    attackPatterns["summon"].Excute();
                else if (attackPatterns["bite"].Can() && !Far())
                    attackPatterns["bite"].Excute();
                else if (Far())
                    anim.SetBool("isWalk", true);
            }
            else
                yield return new WaitForSeconds(1f);
        }
    }

    public void Summon(){
        Vector3 left = new Vector3(-10, 0, 0);
        Vector3 right = new Vector3(10, 0, 0);
        prfHyena.SetActive(true);
        Instantiate(prfHyena, transform.position + left, Quaternion.Euler(0,180,0)).transform.parent = summons;
        Instantiate(prfHyena, transform.position + right, Quaternion.Euler(0,0,0)).transform.parent = summons;
        prfHyena.SetActive(false);
    }
    public void Dash(){
        isDash = true;
        Vector3 v = new Vector3(Mathf.Abs(playerVector.x) - 3f, 0, 0);        
        iTween.MoveBy(gameObject, iTween.Hash("amount", v * -1 , "easeType", iTween.EaseType.easeOutCubic, "speed", 30, "time", 1, "oncomplete", "Bite"));
    }
    public void Bite(){
        isDash = false;
    }
    public bool EndDash(){
        return isDash;
    }
    public bool Far(){
        return Mathf.Abs(playerVector.x) >= 25f;
    }
    
    public float maxRushspeed;
    public float maxRushtime;
    private float rushspeed;
    private float rushtime;
    private bool isTurn = false;
    private bool beforeLeft;
    public void rushStart(){
        rushspeed = maxRushspeed;
        rushtime = maxRushtime;
        beforeLeft = isLeft;
        isTurn = false;
        atkCollider[1].SetActive(true);
    }
    public void rushEnd(){
        atkCollider[1].SetActive(false);
        isTurn = true;
        rushtime = 0;
    }
    public bool Rush(){
        if(rushtime <= 0f && rushspeed <=0f){
            rushEnd();
            return false;
        }

        if(!isTurn){
            Move((beforeLeft?Vector2.left:Vector2.right), rushspeed);
            checkTurn();
        }else{
            if(rushspeed <= 0.1f){
                rushspeed = maxRushspeed;
                beforeLeft = isLeft;
                isTurn = false;
            }else{
                rushspeed -= Time.fixedDeltaTime * maxRushspeed;
                Move((beforeLeft ? Vector2.left : Vector2.right), rushspeed);
            }
        }
        rushtime -= Time.fixedDeltaTime;    
        return true;
    }
    public void checkTurn(){
        if(beforeLeft != isLeft){
            isTurn = true;
            Flip();
        }
    }
    public void SetRushCollider(bool set){
        transform.GetChild(0).GetChild(1).gameObject.SetActive(set);
    }

    public void BackAttack(){
        Vector3 v = new Vector3((isLeft ? -5 : 5), 0, 0);
        Quaternion q = Quaternion.Euler(0, (isLeft ? 180 : 0), 0);
        prfHyena.SetActive(true);
        Instantiate(prfHyena, player.transform.position + v, q).transform.parent = summons;
        prfHyena.SetActive(false);
    }
    public bool InRange(){
        return Mathf.Abs(playerVector.x) <= 6f;
    }

    

    public void ManagePhase(){
        if((float) curHp/hp <= 0.2) state = phase.third;
        else if((float) curHp/hp <= 0.5) state = phase.second;
    }
    public override void CheckDead(bool death = false)
    {
        if(curHp <= 0f || death){
            int childCount = summons.childCount;
            for (int i = 0; i < childCount; i++)
            {
                summons.GetChild(i).GetComponent<GroundEnemy>().CheckDead(true);
            }
            anim.SetTrigger("isDead");
        }
    }
    public override void Death()
    {
        StopCoroutine(Attack());
        gameObject.tag = "Untagged";
        Destroy(this);
        base.Death();
    }

}
