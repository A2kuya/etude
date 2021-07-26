using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyenaAttackToPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    Hyena parent;
    void Start()
    {
        parent = GetComponentInParent<Hyena>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(parent.damage);
        }
    }
}
