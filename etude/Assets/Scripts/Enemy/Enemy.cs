using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Animator anim;
    protected Rigidbody2D rigid;
    protected GameObject player;
    public GameObject prfCoin;
    public GameObject hpBar;

    
    [SerializeField] protected int maxHp;
    protected int curHp;
    private int stiffness = 100;
    public int damage;
    [SerializeField] protected int price;
    public LayerMask playerLayer;   //플레이어 레이어
    public LayerMask obstacleLayer; //지형 레이어
    protected Vector2 playerVector;    //플레이어와의 거리
    protected float playerDistance;
    protected bool isLeft;
    public bool isGround;
    protected bool isJump;
    public bool isDead = false;
    public bool isMoving;
    protected Vector2 spriteSize;
    
    public void ConstructSet(bool isLeft){
        this.isLeft = isLeft;
    }
    public void ConstructSet(bool isLeft, int hp, int damage, int price){
        this.isLeft = isLeft;
        this.maxHp = hp;
        this.damage = damage;
        this.price = price;
    }
    public void Trigger(string s){
        anim.SetTrigger(s);
    }
    public void Stop(){
        rigid.velocity = new Vector2(0, rigid.velocity.y);
        isMoving = false;
    }
    virtual protected void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
    }
    virtual protected void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0);
    }
    virtual public void Flip(bool reverse = false)  //좌우 반전
    {  
        bool dir = (reverse? !isLeft : isLeft);
        if (dir)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    public void TakeDamage(int damage, int stiffness = 0)  //피격
    {
        curHp -= damage;
        if(curHp < 0)
            curHp = 0;
        this.stiffness -= stiffness;
        CheckDead();
        if(gameObject.activeSelf)
            anim.SetTrigger("isHurt");
    }
    public void TakeDamage(int damage, int stiffness, Vector2 enemyPos, Vector2 knockback)
    {
		maxHp -= damage;
        if(curHp < 0)
            curHp = 0;
		this.stiffness -= stiffness;
		int knockbackDir = 0; 
		if(transform.position.x - enemyPos.x <= 0)
        {
			knockbackDir = -1;
        }
        else
        {
			knockbackDir = 1;
        }
        Stop();
	    rigid.AddForce(new Vector2(knockbackDir * knockback.x, knockback.y));
		anim.SetTrigger("isHurt");
	}
    protected void GetSpriteSize(){
        Vector2 worldSize = Vector3.zero;
        spriteSize = spriteRenderer.sprite.rect.size;
        Vector2 localSpriteSize = spriteSize / spriteRenderer.sprite.pixelsPerUnit;
        worldSize = localSpriteSize;
        worldSize.x *= transform.lossyScale.x;
        worldSize.y *= transform.lossyScale.y;
        spriteSize = worldSize;
    }
    protected void UpdateHpBar(){
        hpBar.SetActive(curHp != maxHp);
        hpBar.GetComponentInChildren<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, spriteSize.y + 0.5f, 0));
        hpBar.GetComponentInChildren<Slider>().value = (float) curHp / (float) maxHp;
    }
    protected bool isStiff = false;
    public float stiffTime;
    protected float curStiffTime;
    virtual public void CheckStiffness(bool stiff = false){
        if(!isStiff && (stiffness <= 0f || stiff)){
            Trigger("isStiff");
        }
    }
    virtual public void StiffStart(){
        isStiff = true;
        curStiffTime = stiffTime;
    }
    public bool CheckStiffTime(){
        curStiffTime -= Time.deltaTime;
        return curStiffTime <= 0f;        
    }
    virtual public void StiffEnd(){
        stiffness = 100;
        isStiff = false;
    }
    virtual public void CheckDead(bool death = false){
        if(!isDead && (curHp <= 0f || death)){
            isDead = true;
            anim.SetTrigger("isDead");
        }
    }
    virtual public void Death(){
        SpawnCoins();
        Destroy(hpBar);
        Destroy(gameObject);
    }

    protected void SpawnCoins()
    {
        for (int i = 0; i < price; i++)
        {
            CoinPool.GetObject(transform.position, new Vector2(Random.Range(-500, 500), 800));
        }
    }
}
