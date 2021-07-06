using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeDamage : MonoBehaviour
{
    public LayerMask playerLayer;   //플레이어 레이어
    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
    }
   

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == 6){            
            Debug.Log("take damage");
            other.gameObject.GetComponent<Player>().TakeDamage(10, transform);
        }
    }
}
