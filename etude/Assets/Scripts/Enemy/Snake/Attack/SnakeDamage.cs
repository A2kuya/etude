using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeDamage : MonoBehaviour
{
    Snake parent;
    void Start()
    {
        parent = GetComponentInParent<Snake>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(parent.damage, 0);
        }
    }
}
