using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHyena : Boss
{
    public GameObject prfHyena;
    List<GameObject> hyenas;
    List<Hyena> summons;
    public phase state;
    public enum phase{ first, second, third }
    // Start is called before the first frame update
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
            if (state >= phase.third && attackPatterns["backAttack"].Can())
                attackPatterns["backAttack"].Excute();
            else if (state >= phase.second && attackPatterns["rush"].Can())
                attackPatterns["rush"].Excute();    
            else if (attackPatterns["summon"].Can())
                attackPatterns["summon"].Excute();
            else if (attackPatterns["bite"].Can())
                attackPatterns["bite"].Excute();
            yield return new WaitForSeconds(2f);
        }
    }

    public void Summon(){
        Vector3 left = new Vector3(-10, 0, 0);
        Vector3 right = new Vector3(10, 0, 0);
        Instantiate(prfHyena, transform.position + left, Quaternion.Euler(0,180,0)).transform.parent = transform.GetChild(1).transform;
        Instantiate(prfHyena, transform.position + right, Quaternion.Euler(0,0,0)).transform.parent = transform.GetChild(1).transform;        
    }
    public void Bite(){

    }
    public void Rush(){

        
    }

    public void BackAttack(){
        Vector3 v = new Vector3(0, (isLeft ? -1 : 1), 0);
        Quaternion q = Quaternion.Euler(0, (isLeft ? 180 : 0), 0);
        Instantiate(prfHyena, player.transform.position + v, q).transform.parent = transform.GetChild(1).transform;
    }

    public void Trigger(string s){
        anim.SetTrigger(s);
    }

    public void ManagePhase(){
        if((float) curHp/hp <= 0.2) state = phase.third;
        else if((float) curHp/hp <= 0.5) state = phase.second;
    }


}
