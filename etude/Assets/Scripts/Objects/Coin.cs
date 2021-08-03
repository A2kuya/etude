using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private float time;
    private void OnEnable() {
        time = 0.5f;
        gameObject.layer = 11;
        StartCoroutine(DontDrop());
    }

    IEnumerator DontDrop(){
        var wait = new WaitForSeconds(0.1f);
        while(time > 0f){
            time -= 0.1f;
            yield return wait;
        }
        gameObject.layer = 22;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.GetMoney(1);
            CoinPool.ReturnObject(this);
        }
    }
}
