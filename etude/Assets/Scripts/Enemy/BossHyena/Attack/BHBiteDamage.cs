using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHBiteDamage : MonoBehaviour
{
    BossHyena parent;
    void OnEnable()
    {
        parent = gameObject.transform.parent.GetComponentInParent<BossHyena>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(parent.attackPatterns["bite"].damage);
        }
    }
}
