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
        attackPatterns.Add("bite", new Bite(30, 5, this));
        attackPatterns.Add("summon", new Summon(0, 20, this));
        attackPatterns.Add("rush", new Rush(30, 10, this));
        attackPatterns.Add("backAttack", new BackAttack(30, 10, this));
        state = phase.first;
        GetSpriteSize();
        StartCoroutine(Attack());
    }
    void FixedUpdate() {
        if(isMoving){
            Movement();
        }
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

    }

    IEnumerator Attack(){
        while (true)
        {
            yield return new WaitForSeconds(1f);
            // if (state >= phase.third && attackPatterns["backAttack"].Can())
            //     attackPatterns["backAttack"].Excute();
            // else if (state >= phase.second && attackPatterns["rush"].Can())
            //     attackPatterns["rush"].Excute();    
            // else if (attackPatterns["summon"].Can())
            //     attackPatterns["summon"].Excute();
            // else if (attackPatterns["bite"].Can())
            //     attackPatterns["bite"].Excute();
            if (attackPatterns["rush"].Can())
                 attackPatterns["rush"].Excute();    
            yield return new WaitForSeconds(2f);
        }
    }

    public void Summon(){
        Vector3 left = new Vector3(-10, 0, 0);
        Vector3 right = new Vector3(10, 0, 0);
        Instantiate(prfHyena, transform.position + left, Quaternion.Euler(0,180,0)).transform.parent = summons;
        Instantiate(prfHyena, transform.position + right, Quaternion.Euler(0,0,0)).transform.parent = summons;
    }
    public void Bite(){
        Vector3 v = new Vector3(Mathf.Abs(playerVector.x) - 3f, 0, 0);
        iTween.MoveBy(gameObject, iTween.Hash("amount", v * -1 , "easeType", iTween.EaseType.easeOutQuint));
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
    }
    public bool Rush(){
        if(rushtime <= 0f && rushspeed <=0f)
            return false;

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
        Vector3 v = new Vector3(0, (isLeft ? -1 : 1), 0);
        Quaternion q = Quaternion.Euler(0, (isLeft ? 180 : 0), 0);
        Instantiate(prfHyena, player.transform.position + v, q).transform.parent = summons;
    }
    public bool InRange(){
        return Mathf.Abs(playerVector.x) <= 6f;
    }

    public void Trigger(string s){
        anim.SetTrigger(s);
    }

    public void ManagePhase(){
        if((float) curHp/hp <= 0.2) state = phase.third;
        else if((float) curHp/hp <= 0.5) state = phase.second;
    }
    public override void CheckDead()
    {
        if(curHp <= 0f){
            int childCount = summons.childCount;
            for (int i = 0; i < childCount; i++)
            {
                summons.GetChild(i).GetComponent<Enemy>().CheckDead(true);
            }
            anim.SetTrigger("isDead");
        }
    }

}
