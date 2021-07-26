using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    RaycastHit2D hit;
    LayerMask obstacleLayer;
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
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(damage, transform.position, knockback);
            Destroy(gameObject);
        }
    }
}
