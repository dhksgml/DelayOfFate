using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnData
{
	public string prefabName;
	public int count;
}

[System.Serializable]
public class WaveData
{
	public List<EnemySpawnData> enemies;
	public List<EnemySpawnData> middleBoss;
	public bool hasMidBoss;
}
