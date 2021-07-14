using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackPos : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (player.isSpecialAttacking)
                collision.gameObject.GetComponent<Enemy>().TakeDamage(player.specialDamage);
            else if (player.isChargeAttacking)
                collision.gameObject.GetComponent<Enemy>().TakeDamage(player.chargeDamage);
            else
                collision.gameObject.GetComponent<Enemy>().TakeDamage(player.damage);
        }

        if (player.isSpecialAttacking && collision.transform.tag == "BrokenFloor")
        {
            player.brokeGround.Break();
            gameObject.SetActive(false);
        }
    }
}
