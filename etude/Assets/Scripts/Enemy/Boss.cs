using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Boss : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Animator anim;
    protected Rigidbody2D rigid;
    protected GameObject player;
    public GameObject prfHpBar;
    public GameObject canvas;
    protected GameObject hpBar;

    public Dictionary<string, AttackPattern> attackPatterns;

    public int hp;  //체력
    public int curHp;   //현재 체력
    public int stiffness;   //경직도
    public LayerMask playerLayer;   //플레이어 레이어
    public LayerMask obstacleLayer; //지형 레이어
    public Vector2 playerVector;    //플레이어와의 거리
    public float playerDistance;
    public bool isLeft;
    public bool isGround;
    public bool isJump;
    public bool isMoving;
    protected Vector2 spriteSize;



    public void Stop(){
        rigid.velocity = Vector2.zero;
        isMoving = false;
    }
    public void CaculateDistance() {  //거리계산 및 방향계산
        playerVector.x = player.transform.position.x - transform.position.x;
        playerVector.y = player.transform.position.y - transform.position.y;
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0);
    }
    public void yFlip(bool re)  //좌우 반전
    {
        bool reverse = (re? !isLeft : isLeft);
        if (reverse)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    virtual public void CheckObstacle(){
        //바닥 체크
        isGround = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
    }
    
    public void TakeDamage(int damage, int stiffness = 0)  //피격
    {
        curHp -= damage;
        this.stiffness -= stiffness;
        CheckDead();
        anim.SetTrigger("isHurt");
    }
    public void CheckDead(){
        if (curHp <= 0f)
        {
            Transform summons = transform.GetChild(1);
            int childCount = summons.childCount;
            for (int i = 0; i < childCount; i++)
            {
                summons.GetChild(i).GetComponent<Enemy>().CheckDead(true);
            }
            anim.SetTrigger("isDead");
        }
    }
    public void Death(){
        Destroy(hpBar);
        this.enabled = false;
    }
    public void UpdateHpBar(){
        hpBar.SetActive(curHp != hp);
        hpBar.GetComponentInChildren<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, spriteSize.y + 0.5f, 0));
        hpBar.GetComponentInChildren<Slider>().value = (float) curHp / (float) hp;
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
    
    
}
