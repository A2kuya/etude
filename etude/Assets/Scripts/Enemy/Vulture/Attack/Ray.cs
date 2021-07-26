using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    RaycastHit2D hit;
    public int damage;
    public Vector2 knockback;


    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(damage, transform.position, knockback);
        }
    }
    
}
