using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryData : ObjData
{
    public Player player;

    public override void Action(bool isAction)
    {
        talkPanel.SetActive(isAction);
        player.Heal(100);
        
    }
}
