using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHyena : Boss
{
    public GameObject prfHyena;
    Transform summons;
    public int biteMultiplyer;
    public float biteCooltime;
    public float summonCooltime;
    public int rushMultiplyer;
    public float rushCooltime;
    public float backAttackCooltime;
    public float attackCooltime;
    private bool isDash;
    MonsterFactory mf;

    // Start is called before the first frame update
    private void Awake(){
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        summons = GameObject.Find("Summons").transform;
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        attackPatterns = new Dictionary<string, AttackPattern>();
        attackPatterns.Add("bite", new Bite(biteMultiplyer * damage, biteCooltime, this));
        attackPatterns.Add("summon", new Summon(0, summonCooltime, this));
        attackPatterns.Add("rush", new Rush(rushMultiplyer * damage, rushCooltime, this));
        attackPatterns.Add("backAttack", new BackAttack(0, backAttackCooltime, this));
        mf = new MonsterFactory();
        state = phase.first;
        isAttack = false;
        GetSpriteSize();
    }
    private void OnEnable() {
        StartCoroutine(Attack());
        curHp = maxHp;
        Flip();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHpBar();
        CheckDead();
        CheckStiffness();
        CaculateDistance();
        CheckObstacle();
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
                yield return new WaitForSeconds(1f);
                if(CheckPhase())
                    anim.SetTrigger("changePhase");
                else if (state >= phase.third && attackPatterns["backAttack"].Can()){
                    isAttack = true;
                    attackPatterns["backAttack"].Excute();
                }
                else if (state >= phase.second && attackPatterns["rush"].Can()){
                    isAttack = true;
                    attackPatterns["rush"].Excute();
                }
                else if (attackPatterns["summon"].Can()){
                    isAttack = true;
                    attackPatterns["summon"].Excute();
                }
                else if (attackPatterns["bite"].Can() && !Far()){
                    isAttack = true;
                    attackPatterns["bite"].Excute();
                }
                else if (Far()){
                    anim.SetBool("isWalk", true);
                }
                yield return new WaitForSeconds(attackCooltime - 1f);
            }
            else{
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public  Vector3 leftSummon;
    public  Vector3 rightSummon;
    public void Summon(){
        Vector3 left, right;
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, leftSummon, Vector2.Distance(Vector2.zero, leftSummon), obstacleLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, rightSummon, Vector2.Distance(Vector2.zero, rightSummon), obstacleLayer);
        if(hit1)
            left = new Vector3(-hit1.distance + 1f, 0, 0);
        else
            left = leftSummon;
        if(hit2)
            right = new Vector3(hit2.distance - 1f, 0, 0);
        else
            right = rightSummon;
        prfHyena.SetActive(true);        
        mf.CreateEnemy("hyena", summons, transform.position + left, false, 100, damage, 0).GetComponent<Hyena>().SetAttack();
        mf.CreateEnemy("hyena", summons, transform.position + right, true, 100, damage, 0).GetComponent<Hyena>().SetAttack();
        prfHyena.SetActive(false);
        isAttack = false;
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
    public float rushReadyTime;
    public void rushStart(){
        rushspeed = maxRushspeed;
        rushtime = maxRushtime;
        beforeLeft = isLeft;
        isTurn = false;
        InAttack(true);
    }
    public void rushEnd(){
        SetRushCollider(false);
        isTurn = true;
        rushtime = 0;
        InAttack(false);
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
        atkCollider[1].SetActive(set);
    }
    public void BackAttack(){
        Vector3 v = new Vector3((isLeft ? -5 : 5), 0, 0);
        mf.CreateEnemy("hyena", summons, player.transform.position + v, !isLeft, 100, damage, 0).GetComponent<Hyena>().SetAttack();        isAttack = false;
    }
    public bool InRange(){
        return Mathf.Abs(playerVector.x) <= 6f;
    }
    public void KnockbackToPlayer(){
        Vector2 v = new Vector2(100, 20);
        player.GetComponent<Player>().TakeDamage(0, transform.position, v);
    }
    public override void StiffEnd()
    {
        base.StiffEnd();
        StartCoroutine(Attack());
    }
    public override void CheckDead(bool death = false)
    {
        if(curHp <= 0f || death){
            int childCount = summons.childCount;
            for (int i = 0; i < childCount; i++)
            {
                summons.GetChild(i).GetComponent<Enemy>().CheckDead(true);
            }
            anim.SetTrigger("isDead");
        }
    }
}
