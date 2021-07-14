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
    public GameObject prfHpBar;
    public GameObject canvas;
    protected GameObject hpBar;
    public int hp;
    public int curHp;
    public int stiffness;
    public LayerMask playerLayer;   //플레이어 레이어
    public LayerMask obstacleLayer; //지형 레이어
    public Vector2 playerVector;    //플레이어와의 거리
    public float playerDistance;
    public bool isLeft;
    public bool isGround;
    public bool isJump;
    public bool isMoving;
    protected Vector2 spriteSize;

    public void Trigger(string s){
        anim.SetTrigger(s);
    }
    public void Stop(){
        rigid.velocity = new Vector2(0, rigid.velocity.y);
        isMoving = false;
    }
    virtual public void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
    }
    virtual public void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0);
    }
    public void Flip(bool reverse = false)  //좌우 반전
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
        this.stiffness -= stiffness;
        CheckDead();
        if(gameObject.activeSelf)
            anim.SetTrigger("isHurt");
    }
    public void TakeDamage(int damage, int stiffness, Vector3 attackPosition)  //피격
    {
        curHp -= damage;
        this.stiffness -= stiffness;
        Vector2 v;
        if(transform.position.x - attackPosition.x > 0)
            v = Vector2.right;
        else if(transform.position.x - attackPosition.x == 0)
            if(isLeft) v = Vector2.right;
            else v = Vector2.left;
        else    v = Vector2.left;
        Stop();
        rigid.AddForce(v + Vector2.up);
        anim.SetTrigger("isHurt");
    }
    public void GetSpriteSize(){
        Vector2 worldSize = Vector3.zero;
        spriteSize = spriteRenderer.sprite.rect.size;
        Vector2 localSpriteSize = spriteSize / spriteRenderer.sprite.pixelsPerUnit;
        worldSize = localSpriteSize;
        worldSize.x *= transform.lossyScale.x;
        worldSize.y *= transform.lossyScale.y;
        spriteSize = worldSize;
    }
    public void UpdateHpBar(){
        hpBar.SetActive(curHp != hp);
        hpBar.GetComponentInChildren<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, spriteSize.y + 0.5f, 0));
        hpBar.GetComponentInChildren<Slider>().value = (float) curHp / (float) hp;
    }
    virtual public void CheckDead(bool death = false){
        if(curHp <= 0f || death){
            anim.SetTrigger("isDead");
        }
    }
    virtual public void Death(){
        Destroy(hpBar);
        Destroy(gameObject);
    }
}
