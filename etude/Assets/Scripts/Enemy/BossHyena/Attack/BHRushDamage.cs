using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHRushDamage : MonoBehaviour
{
    BossHyena parent;
    void OnEnable()
    {
        parent = gameObject.transform.parent.GetComponentInParent<BossHyena>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(parent.attackPatterns["rush"].damage, 0, transform.position, new Vector2(100, 20));
            parent.rushEnd();
        }
    }
}
