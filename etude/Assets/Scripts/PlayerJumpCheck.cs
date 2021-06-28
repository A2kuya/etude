using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpCheck : MonoBehaviour
{
    public GameObject player;
    PlayerController script;
    void Start()
    {
        script = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            script.isJumping = false;
            script.animator.SetBool("isJumping", false);
        }
    }
}
