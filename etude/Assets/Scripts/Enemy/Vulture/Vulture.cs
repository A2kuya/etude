using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture : Boss
{
    public GameObject prfStone;
    public GameObject prfRay;
    public GameObject prfWarningLine;
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
        attack = Attack();
        creatStones = CreatStones();
        StartCoroutine(attack);
    }
    IEnumerator attack;
    IEnumerator creatStones;

    void Update()
    {
        UpdateHpBar();
        CheckDead();
        CaculateDistance();
        CheckObstacle();
        curtime -= Time.deltaTime;
    }
    public float curtime;

    IEnumerator Attack(){
        while(true){
            if (!isAttack && !isMoving)
            {
                yield return new WaitForSeconds(1f);
                if (CheckPhase()){
                    anim.SetTrigger("changePhase");
                }
                else if (attackPatterns["fallStones"].Can())
                {
                    isAttack = true;
                    attackPatterns["fallStones"].Excute();
                }
                else if (attackPatterns["shootRay"].Can())
                {
                    isAttack = true;
                    attackPatterns["shootRay"].Excute();
                }
                else if (attackPatterns["cutAir"].Can()){
                    isAttack = true;
                    attackPatterns["cutAir"].Excute();
                }
                else
                {
                    Move();
                    yield return new WaitForSeconds(attackCooltime - 1f);
                }
            }
            else
                yield return new WaitForSeconds(1f);
        }
    }
    public void Move(){
        while (true)
        {
            if(!isMoving){
                Vector3 amount = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
                if (CanMove(amount))
                {
                    isMoving = true;
                    UpdateDirection();
                    Flip();
                    if(!isLeft)
                        amount.x *= -1;
                    iTween.MoveBy(gameObject, iTween.Hash(
                        "amount", amount,
                        "easeType", iTween.EaseType.easeOutExpo,
                        "time", 3f,
                        "oncomplete", "IsNotMove"
                    ));
                }
                else{
                    continue;
                }
            }
            else{
                return;
            }
        }
    }
    public bool CanMove(Vector3 amount){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, amount, amount.magnitude, obstacleLayer);
        Debug.DrawRay(transform.position, amount, Color.blue);
        if(hit && !hit.collider.CompareTag("Through")){
            return false;
        }
        else{
            return true;
        }
    }
    public void IsNotMove(){
        isMoving = false;
    }
    public Vector3 left;
    public Vector3 right;
    public int roundTripCount;
    public int roundTripSpeed;
    public float stoneCycleMin;
    public float stoneCycleMax;
    private int count;
    public void FallStoneStart(){
        count = 0;
        anim.speed = 1;
        isMoving = false;
        StartCoroutine(creatStones);
        Flip();
    }
    public void FallStone(){
        if(!isMoving){
            if(count >= roundTripCount)
                FallStoneEnd();
            else{
                isMoving = true;
                count++;
                iTween.MoveTo(gameObject, iTween.Hash(
                    "position", (isLeft ? left : right),
                    "easeType", iTween.EaseType.linear,
                    "speed", roundTripSpeed,
                    "oncomplete", "Turn"
                ));
            }
        }
    }
    IEnumerator CreatStones(){
        while(true){
            Instantiate(prfStone, transform.position, transform.rotation);
            yield return new WaitForSeconds(Random.Range(stoneCycleMin, stoneCycleMax));
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
        StopCoroutine(creatStones);
        isAttack = false;
        anim.SetTrigger("rest");
    }

    public int rayTime;
    public int rayRotateAmount;
    public void ShootRayStart(){
        int raynum = 0;
        UpdateDirection();
        Flip();
        transform.GetChild(1).localRotation = Quaternion.Euler(0, 0, 0);
        isAttack = true;
        switch(state){
            case phase.first:
                raynum = 5;
                break;
            case phase.second:
            case phase.third:
                raynum = 6;
                break;
        }
        float angle = 360 / raynum;
        for (int i = 0; i < raynum; i++){
            var tmp = Instantiate(prfRay, transform.GetChild(1));
            tmp.transform.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad * i) * (isLeft ? 1 : -1), Mathf.Sin(angle * Mathf.Deg2Rad * i), 0) / 4;
            tmp.transform.rotation = Quaternion.Euler(0, 0, angle * i);            
        }
        iTween.RotateAdd(transform.GetChild(1).gameObject, iTween.Hash(
            "amount", new Vector3(0,0, angle * rayRotateAmount),
            "time", rayTime ,
            "easeType", iTween.EaseType.easeInOutBack,
            "oncomplete", "ShootRayEnd",
            "oncompletetarget", gameObject
            ));
    }
    public void ShootRayEnd(){
        int num = transform.GetChild(1).childCount;
        for(int i=0;i<num;i++){
            Destroy(atkCollider[1].transform.GetChild(i).gameObject);
        }
        isAttack = false;
        anim.SetTrigger("attackEnd");
    }


    bool keep;
    Vector3 nextAttackDir;
    Vector3 nextAttackLocation;
    int cutCount;
    public float cutAirDelayFirstPhase;
    public float cutAirDelaySecondPhase;
    float cutAirDelay;
    public void ReadyCutAir(){
        keep = false;
        nextAttackDir = Vector3.right;
        isLeft = true;
        Flip();
        switch(state){
        case phase.first:
            cutAirDelay = cutAirDelayFirstPhase;
            cutCount = 3;
            break;
        case phase.second:
            cutAirDelay = cutAirDelaySecondPhase;
            cutCount = 3;
            break;
        case phase.third:
            cutAirDelay = cutAirDelaySecondPhase;
            cutCount = 4;
            break;
        }
        iTween.MoveBy(gameObject, iTween.Hash(
            "y", 100,
            "speed", 50,
            "easeType", iTween.EaseType.easeInExpo,
            "oncomplete", "NextCut"
        ));
    }
    void NextCut(){
        switch(state){
        case phase.first:
        case phase.second:
            switch(cutCount){
                case 1:
                case 3:
                    nextAttackDir = Vector3.right;
                    break;
                case 2:
                    nextAttackDir = Vector3.left;
                    break;
                case 0:
                    anim.speed = 1;
                    anim.SetTrigger("attackEnd");
                    return;
            }
            break;
        case phase.third:
            switch(cutCount){
                case 4:
                    nextAttackDir = Vector3.right;
                    break;
                case 3:
                    nextAttackDir = Vector3.left + Vector3.up;
                    break;
                case 2:
                    nextAttackDir = Vector3.down;
                    break;
                case 1:
                    nextAttackDir = Vector3.right + Vector3.up;
                    break;
                case 0:
                    anim.speed = 1;
                    anim.SetTrigger("attackEnd");
                    return;
            }
            break;
        }
        nextAttackLocation = player.transform.position + Vector3.up*2;
        anim.SetTrigger("cutAir");
    }
    public void CutAirStart(){
        isAttack = true;
        StartCoroutine(DrawWarningLine());
    }
    IEnumerator DrawWarningLine(){
        WaitForSeconds wait = new WaitForSeconds(cutAirDelay / 3);
        float q = Vector2.SignedAngle(Vector2.right, nextAttackDir);
        transform.position = nextAttackLocation + nextAttackDir * -40;
        GameObject warningLine = Instantiate(prfWarningLine, nextAttackLocation, Quaternion.Euler(0, 0, q));
        transform.rotation = Quaternion.Euler(0, 0, q - 180);
        yield return wait;
        warningLine.SetActive(false);
        yield return wait;
        warningLine.SetActive(true);
        yield return wait;
        Destroy(warningLine);
        Cut();
    }
    private void Cut(){
        cutCount--;
        atkCollider[0].SetActive(true);
        iTween.MoveBy(gameObject, iTween.Hash(
            "x", -100,
            "time", 0.5f,
            "easeType", iTween.EaseType.linear,
            "oncomplete", "NextCut"
        ));
    }
    public void CutAirEnd(){
        atkCollider[0].SetActive(false);
        transform.rotation = Quaternion.Euler(0,0,0);
        transform.position = new Vector2(Random.Range(left.x, right.x), left.y+10);
    }


    protected Vector2 restPoint;
    public void RestStart(){
        IEnumerator rest = Rest();
        iTween.MoveTo(gameObject, iTween.Hash(
            "y", left.y,
            "time", 1f,
            "easeType", iTween.EaseType.linear,
            "oncomplete", "StartCoroutine",
            "oncompleteparams", rest
        ));    
    }
    IEnumerator Rest(){
        WaitForSeconds wait = new WaitForSeconds(1f);
        rigid.bodyType = RigidbodyType2D.Dynamic;
        while(!isGround){
            iTween.MoveBy(gameObject, iTween.Hash(
                "y", 2f,
                "time", 1f,
                "easeType", iTween.EaseType.easeInBack
            ));
            yield return wait;
        }
        Trigger("restEnd");
        isAttack = false;
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
    public int GetPhase(){
        switch (state)
        {
            case phase.first:
                return 1;
            case phase.second:
                return 2;
            case phase.third:
                return 3;
        }
        return 0;
    }
    public override void Death()
    {
        StopCoroutine(Attack());
        rigid.bodyType = RigidbodyType2D.Dynamic;
        gameObject.tag = "Untagged";
        Destroy(this);
        base.Death();
    }
    override public void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
    }
    public void UpdateDirection(){
        isLeft = playerVector.x < 0;
    }
    override public void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(new Vector2(0, transform.position.y - spriteSize.y / 2), 0.1f, obstacleLayer);
    }
    override public void Flip(bool reverse = false){
        bool dir = (reverse ? !isLeft : isLeft);
        if (dir)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
