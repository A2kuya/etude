using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int hp;
    public float positionX;
    public float positionY;
    public int money;
    public int skillPoint;
    public string scene;
    public int[] skillLevel = new int[3];
}
