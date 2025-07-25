[System.Serializable]
public class EnemySpawnData
{
	public string prefabName;
	public int count;
}

[System.Serializable]
public class WaveData
{
	public EnemySpawnData[] enemies;
}
