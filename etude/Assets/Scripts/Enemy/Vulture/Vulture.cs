using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture : Boss
{
    public GameObject prfStone;
    public Queue<Stone> stonePool = new Queue<Stone>();
    public GameObject prfRay;
    public List<Ray> rayPool = new List<Ray>();
    public GameObject prfWarningLine;
    public List<GameObject> warningLinePool = new List<GameObject>();
 
    public float fallSotnesCooltime;
    public int fallStonesDamageMultiple;
    public float cutAirCooltime;
    public int cutAirDamageMultiple;
    public float shootRayCooltime;
    public int shootRayDamageMultiple;
    
    public float attackCooltime;
    public bool isAttack;
    private Vector2 transportVector;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        attackPatterns = new Dictionary<string, AttackPattern>();
        attackPatterns.Add("fallStones", new FallStones(fallStonesDamageMultiple * damage, fallSotnesCooltime, this));
        attackPatterns.Add("cutAir", new CutAir(cutAirDamageMultiple * damage, cutAirCooltime, this));
        attackPatterns.Add("shootRay", new ShootRay(shootRayDamageMultiple * damage, shootRayCooltime, this));
        isGround = false;
        state = phase.first;
        curHp = maxHp;
        Flip();
        GetSpriteSize();
        attack = Attack();
        creatStones = CreatStones();

        for (int i = 0; i < 6; i++){
            warningLinePool.Add(Instantiate(prfWarningLine, atkCollider[1].transform));
            warningLinePool[i].SetActive(false);
            warningLinePool[i].transform.localScale /= 20;
            rayPool.Add(Instantiate(prfRay, atkCollider[1].transform).GetComponent<Ray>());
            rayPool[i].gameObject.SetActive(false);
        }
    }

    private void OnEnable() {
        StartCoroutine(attack);
    }
    IEnumerator attack;
    IEnumerator creatStones;

    void Update()
    {
        UpdateHpBar();
        CheckDead();
        CheckStiffness();
        CaculateDistance();
        CheckObstacle();
    }

    IEnumerator Attack(){
        while(true){
            if (!isAttack && !isMoving)
            {
                yield return new WaitForSeconds(1f);
                if (CheckPhase()){
                    anim.SetTrigger("changePhase");
                    UpdateDirection();
                    Flip();
                }
                else if (attackPatterns["fallStones"].Can())
                {
                    Debug.Log("fallstone");
                    UpdateDirection();
                    Flip();
                    isAttack = true;
                    attackPatterns["fallStones"].Excute();
                }
                else if (attackPatterns["shootRay"].Can())
                {
                    Debug.Log("shootray");
                    UpdateDirection();
                    Flip();
                    isAttack = true;
                    attackPatterns["shootRay"].Excute();
                }
                else if (attackPatterns["cutAir"].Can()){
                    Debug.Log("cutair");
                    UpdateDirection();
                    Flip();
                    isAttack = true;
                    attackPatterns["cutAir"].Excute();
                }
                else
                {
                    Debug.Log("Move");
                    UpdateDirection();
                    Flip();
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
    private Vector3 startPosition;
    public int roundTripCount;
    public int roundTripSpeed;
    public float stoneCycleMin;
    public float stoneCycleMax;
    private int count;
    public void FallStoneStart(){
        count = 0;
        anim.speed = 1;
        isMoving = false;
        startPosition = transform.position;
        StartCoroutine(creatStones);
        Flip();
    }
    public void FallStone(){
        if(!isMoving){
            if(count > roundTripCount)
                FallStoneEnd();
            else if(count == roundTripCount){
                isMoving = true;
                count++;
                Flip();
                iTween.MoveTo(gameObject, iTween.Hash(
                    "position", startPosition,
                    "easeType", iTween.EaseType.linear,
                    "speed", roundTripSpeed,
                    "oncomplete", "Turn"
                ));
            }
            else{
                isMoving = true;
                count++;
                Flip();
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
            var stone = StonePool.GetObject();
            stone.transform.position = transform.position;
            yield return new WaitForSeconds(Random.Range(stoneCycleMin, stoneCycleMax));
        }
    }
    public void Turn(){
        isLeft = !isLeft;
        isMoving = !isMoving;
    }
    public void FallStoneEnd(){
        count = 0;
        anim.speed = 1;
        isMoving = false;
        transport = false;
        StopCoroutine(creatStones);
        anim.SetTrigger("attackEnd");

    }



    public int rayTime;
    public int rayRotateAmount;
    public float rayDelay;
    private int raynum;
    private float angle;
    public void ShootRayStart(){
        UpdateDirection();
        Flip();
        
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
        angle = 360 / raynum;
        StartCoroutine(RayWarning());
    }
    IEnumerator RayWarning(){
        WaitForSeconds wait = new WaitForSeconds(rayDelay / 3);
        GameObject[] warningLines = new GameObject[6];
        float q = Vector2.SignedAngle(Vector2.right, playerVector + new Vector2(0, 0.5f));
        atkCollider[1].transform.localRotation = Quaternion.Euler(0, 0, q);
        for (int i = 0; i < raynum; i++){
            warningLinePool[i].transform.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad * i), Mathf.Sin(angle * Mathf.Deg2Rad * i), 0) / 4 * 10.5f;
            warningLinePool[i].transform.localRotation = Quaternion.Euler(0, 0, angle * i);
            warningLinePool[i].GetComponent<SpriteRenderer>().spriteSortPoint = SpriteSortPoint.Pivot;
            warningLinePool[i].SetActive(true);
        }
        yield return wait;
        for(int i=0;i<raynum;i++){
            warningLinePool[i].SetActive(false);
        }
        yield return wait;
        for(int i=0;i<raynum;i++){
            warningLinePool[i].SetActive(true);
        }
        yield return wait;
        for(int i=0;i<raynum;i++){
            warningLinePool[i].SetActive(false);
        }
        for (int i = 0; i < raynum; i++){
            rayPool[i].transform.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad * i), Mathf.Sin(angle * Mathf.Deg2Rad * i), 0) / 4;
            rayPool[i].transform.localRotation = Quaternion.Euler(0, 0, angle * i);
            rayPool[i].gameObject.SetActive(true);
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
        for(int i=0;i<raynum;i++){
            rayPool[i].gameObject.SetActive(false);
        }
        isAttack = false;
        anim.SetTrigger("attackEnd");
    }


    Vector3 nextAttackDir;
    Vector3 nextAttackLocation;
    int cutCount;
    public float cutAirDelayFirstPhase;
    public float cutAirDelaySecondPhase;
    float cutAirDelay;
    public void ReadyCutAir(){
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
        StartCoroutine(CutWarning());
    }
    IEnumerator CutWarning(){
        WaitForSeconds wait = new WaitForSeconds(cutAirDelay / 3);
        float q = Vector2.SignedAngle(Vector2.right, nextAttackDir);
        transform.position = nextAttackLocation + nextAttackDir * -50;
        transform.rotation = Quaternion.Euler(0, 0, q - 180);
        atkCollider[1].transform.rotation = Quaternion.Euler(0,0,0);

        warningLinePool[1].SetActive(true);
        warningLinePool[1].transform.position = nextAttackLocation;
        warningLinePool[1].transform.rotation = Quaternion.Euler(0, 0, q - 180);
        
        yield return wait;
        warningLinePool[1].SetActive(false);
        yield return wait;
        warningLinePool[1].SetActive(true);
        yield return wait;
        warningLinePool[1].SetActive(false);
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
        transportVector = new Vector2(Random.Range(left.x, right.x), left.y+10);
        transport = true;
    }



    public float resttime;
    public float fallDistance;
    private int downCount;
    private bool transport;
    public void RestStart(){
        if(transport){
            transport = false;
            transform.position = transportVector;
            iTween.MoveTo(gameObject, iTween.Hash(
                "y", left.y,
                "time", 1f,
                "easeType", iTween.EaseType.linear,
                "oncomplete", "RestFallStart"
            ));
         }else{
             RestFallStart();
         }
    }
    void RestFallStart(){
        StartCoroutine(RestFall());
    }
    IEnumerator RestFall(){
        WaitForSeconds wait = new WaitForSeconds(1f);
        Vector2 origin = transform.position - new Vector3(0, spriteSize.y, 0);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 25, obstacleLayer);
        downCount = (int)(hit.distance / fallDistance);
        for(int i = downCount;i > 0; i--){
            iTween.MoveBy(gameObject, iTween.Hash(
                "y", -fallDistance,
                "time", 0.9f,
                "easeType", iTween.EaseType.easeOutCubic
            ));
            yield return wait;
        }
        iTween.MoveBy(gameObject, iTween.Hash(
            "y", downCount * fallDistance - hit.distance,
            "time", 1f,
            "easeType", iTween.EaseType.easeOutCubic
        ));
        yield return wait;
        Trigger("rest");
    }
    public void RestInLand(){
        StartCoroutine(Rest());
    }
    IEnumerator Rest(){
        rigid.bodyType = RigidbodyType2D.Dynamic;
        float curtime = resttime;
        while(curtime > 0f){
            yield return new WaitForEndOfFrame();
            curtime -= Time.deltaTime;
        }
        rigid.bodyType = RigidbodyType2D.Kinematic;
        isAttack = false;
        Trigger("restEnd");
    }
    public void TakeOffStart(){
        StartCoroutine(TakeOff());
    }
    IEnumerator TakeOff(){
        WaitForSeconds wait = new WaitForSeconds(1f);
        for(int i = 3;i > 0; i--){
            iTween.MoveBy(gameObject, iTween.Hash(
                "y", 1f,
                "time", 0.9f,
                "easeType", iTween.EaseType.easeOutCubic
            ));
            yield return wait;
        }
        yield return wait;
        yield return wait;
        Trigger("fly");
    }

    override public void StiffStart(){
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.AddForce(new Vector2(0, 10));
        base.StiffStart();
    }
    public void CheckFall(){
        if(rigid.velocity.y == 0 || isGround){
            Trigger("rest");
        }
    }
    override public void StiffEnd(){
        base.StiffEnd();
        StartCoroutine(attack);
    }
    public override void Death()
    {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<BoxCollider2D>().size = new Vector2(0.37f, 0.1f);
        for(int i=0;i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
        base.Death();
    }
    override protected void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
    }
    public void UpdateDirection(){
        isLeft = playerVector.x < 0;
    }
    override protected void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position - new Vector3(0, spriteSize.y / 2, 0), 0.2f, obstacleLayer);
    }
    public void KnockbackToPlayer(){
        Vector2 v = new Vector2(100, 20);
        player.GetComponent<Player>().TakeDamage(0, transform.position, v);
    }
    override public void Flip(bool reverse = false){
        bool dir = (reverse ? !isLeft : isLeft);
        if (dir)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
