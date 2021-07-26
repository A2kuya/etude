using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    public Text amountText;
    public Player player;

    public void SetPotion()
    {
        amountText.text = "x" + player.potions;
    }
}
