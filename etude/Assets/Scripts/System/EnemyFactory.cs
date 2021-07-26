using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyFactory
{
    public abstract GameObject CreateEnemy(string s);
    public abstract GameObject CreateEnemy(string s, Vector3 position, bool isLeft);
    public abstract GameObject CreateEnemy(string s, Vector3 position, bool isLeft, int hp, int damage);
    
}
