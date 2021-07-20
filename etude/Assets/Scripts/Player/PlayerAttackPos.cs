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
            if (player.GetState() == "specialAttack")
                collision.gameObject.GetComponent<Enemy>().TakeDamage(player.specialDamage);
            else if (player.GetState() == "chargeAttack")
                collision.gameObject.GetComponent<Enemy>().TakeDamage(player.chargeDamage);
            else
                collision.gameObject.GetComponent<Enemy>().TakeDamage(player.damage);
        }

        if (player.GetState() == "specialAttack" && collision.transform.tag == "BrokenFloor")
        {
            BrokeGround brokeGround = collision.GetComponent<BrokeGround>();
            brokeGround.Break();
            gameObject.SetActive(false);
        }
    }
}
