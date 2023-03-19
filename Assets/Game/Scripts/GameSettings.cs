using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Data/GameSettings")]
[Serializable]
public class GameSettings : ScriptableObject
{
    public float playerSpeed = 10;
    public float enemySpeed = 10;
    public float spawnEnemyMinTime = 1;
    public float spawnEnemyMaxTime = 5;
}
