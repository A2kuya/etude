using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHyena : Boss
{
    GameObject prfHyena;
    List<Hyena> summons;
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
        attackPatterns.Add("continousBite", new ContinuosBite(30, 5));
        attackPatterns.Add("summon", new Summon(0, 10));
        attackPatterns.Add("rush", new Rush(30, 5));
        GetSpriteSize();
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
        ManageCoolTime();
    }
    public void Movement(){

    }

    public void Attack(){

    }

}
