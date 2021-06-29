using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator anim;
    Rigidbody2D rigid;
    GameObject player;
    Controller2D controller;
    public GameObject atkCollider;

    public int hp;  //ü�¹�
    public bool isLeft; //������ �ٶ󺸴���
    public LayerMask playerLayer;   //�÷��̾� ���̾�
    public LayerMask playerAttackLayer; //�÷��̾� ���� ���̾�
    public LayerMask obstacleLayer; //���� ���̾�
    public int detectDistance;      //Ž�� �Ÿ�
    public float playerDistance;    //�÷��̾���� �Ÿ�
    public float range;             //���� ����
    private float cooltime;         //���� ��Ÿ��
    private float curtime;          //�پ��� ���� ��Ÿ��

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        playerAttackLayer = LayerMask.GetMask("PlayerAttackLayer");
        obstacleLayer = LayerMask.GetMask("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        CaculateplayerDistance();
    }

    public void Attack() { }
    public void Detect()
    {
        
    }

    public void CaculateplayerDistance() {
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        isLeft = (transform.position.x - player.transform.position.x > 0) ? true : false;
    }
    public void ManageSprite()  //�ٶ󺸴� ���� ���� �ϴ� �Լ�(�⺻�� ����)
    {
        if (isLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    public void ManageCoolTime()
    {
        if(curtime >= 0f)
        {
            curtime -= Time.deltaTime;
        }
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == playerAttackLayer)
        {
            //������ ������Ʈ���� �������� �޾ƿͼ� ó��
        }
    }
}
