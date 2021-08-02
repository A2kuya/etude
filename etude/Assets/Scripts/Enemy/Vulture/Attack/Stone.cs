using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private RaycastHit2D hit;
    private LayerMask obstacleLayer;
    [SerializeField]
    private AudioClip audioClip;
    public int damage;
    public Vector2 knockback;

    void Start()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");
    }

    void Update()
    {
        CheckDrop();
    }
    public void CheckDrop(){
        hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, obstacleLayer);
        if(hit && !hit.transform.CompareTag("Through")){
            SoundManager.instance.SFXPlay("StoneCrash", audioClip);
            StonePool.ReturnObject(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(damage, transform.position, knockback);
            SoundManager.instance.SFXPlay("StoneCrash", audioClip);
            StonePool.ReturnObject(this);
        }
    }
}
