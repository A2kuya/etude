using System.Collections;
using System.Collections.Generic;
using UnityEngine;
abstract public class Enemy : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    protected Animator anim;
    Rigidbody2D rigid;
    GameObject player;
    protected Controller2D controller;
    public GameObject atkCollider;

    public int hp;  //ü�¹�
    public int damage; //������
    public bool isLeft; //������ �ٶ󺸴���
    public LayerMask playerLayer;   //�÷��̾� ���̾�
    public LayerMask playerAttackLayer; //�÷��̾� ���� ���̾�
    public LayerMask obstacleLayer; //���� ���̾�
    public int speed = 1;               //�ӵ� ���
    public Vector2 velocity = Vector2.zero;        //�̵� �ӵ�
    public Vector2 dir = Vector2.zero;  //����
    public int attackSpeed;         //���� �ӵ�
    public float detectDistance;    //Ž�� �Ÿ�
    public float chaseDistance;     //�߰� �Ÿ�
    public float playerDistance;    //�÷��̾���� �Ÿ�
    public float range;             //���� ����
    public float cooltime;         //���� ��Ÿ��
    public float curtime;          //�پ��� ���� ��Ÿ��
    protected float gravity;                  //�߷�

    void Awake()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        playerLayer = LayerMask.GetMask("Player");
        playerAttackLayer = LayerMask.GetMask("PlayerAttackLayer");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        controller = GetComponent<Controller2D>();
    }

    abstract public void Move();    //������
    abstract public void Attack();  //����
    abstract public bool Detect();  //Ž��
    abstract public bool Miss();    //��ġ�� ��

    public void CaculateplayerDistance() {  //�÷��̾� �Ÿ���� �� ���� ����
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        if(isLeft = (transform.position.x - player.transform.position.x > 0))
        {
            dir.x = Vector2.left.x;
        }
        else
        {
            dir.x = Vector2.right.x;
        }
    }
    public void yFlip()  //�¿� �ø�(������ ��)
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

    protected void CaculateVelocity()
    {

    }
}
