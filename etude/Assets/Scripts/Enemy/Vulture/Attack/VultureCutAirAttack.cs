using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureCutAirAttack : MonoBehaviour
{
    Vulture parent;
    void OnEnable()
    {
        parent = gameObject.transform.parent.GetComponentInParent<Vulture>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<Player>().TakeDamage(parent.attackPatterns["cutAir"].damage, 0, parent.transform.position, new Vector2(20, 20));
        }
    }
}
