using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class SpawnManager : MonoBehaviour
{
	public GameObject[] enemyPrefabs;
	public GameObject itemPrefab;
	public ItemData[] item_date;
	public WaveData[] waveList; // 웨이브별 구성
	public int totalValPoint;

	private List<Transform> enemySpawnPoints = new List<Transform>();
	private List<Transform> itemSpawnPoints = new List<Transform>();

	private Dictionary<string, GameObject> enemyPrefabDict = new Dictionary<string, GameObject>();

	void Awake()
	{
		// 프리팹 이름 → 프리팹 객체 매핑
		foreach (GameObject prefab in enemyPrefabs)
		{
			if (prefab != null && !enemyPrefabDict.ContainsKey(prefab.name))
			{
				enemyPrefabDict.Add(prefab.name, prefab);
			}
		}
	}

	public void SpawnWave(int waveIndex)
	{
		if (waveIndex < 0 || waveIndex >= waveList.Length)
		{
			Debug.LogWarning("Wave index out of range.");
			return;
		}

		enemySpawnPoints.Clear();
		itemSpawnPoints.Clear();

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyPoint"))
		{
			if (obj.name.Contains("EnemyPoint"))
			{
				enemySpawnPoints.Add(obj.transform);
				obj.GetComponent<Create_Disable>()?.Disable();
			}
		}

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ItemPoint"))
		{
			if (obj.name.Contains("ItemPoint"))
			{
				itemSpawnPoints.Add(obj.transform);
				obj.GetComponent<Create_Disable>()?.Disable();
			}
		}

		// 스폰 시작
		WaveData wave = waveList[waveIndex];
		int usedCoinTotal = 0;

		// 중간보스 처리
		if (wave.hasMidBoss && wave.enemies.Count > 0)
		{
			EnemySpawnData midBossData = wave.enemies[0];

			if (midBossData.count > 0 && enemyPrefabDict.TryGetValue(midBossData.prefabName, out GameObject bossPrefab))
			{
				Enemy bossComp = bossPrefab.GetComponentInChildren<Enemy>();
				if (bossComp != null && bossComp.enemyData != null && enemySpawnPoints.Count > 0)
				{
					int index = Random.Range(0, enemySpawnPoints.Count);
					Transform spawnPoint = enemySpawnPoints[index];
					enemySpawnPoints.RemoveAt(index);

					// 중간보스 생성
					var boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
					usedCoinTotal += bossComp.enemyData.Coin;

					// 색깔 넣기
					SpriteRenderer enemyColor = boss.GetComponentInChildren<SpriteRenderer>();

					if (enemyColor != null)
					{
						// 빨강으로
						enemyColor.color = new Color(1f, 0f, 0f);
					}

					// 해당 몬스터 수량 1 감소
					midBossData.count--;
				}
			}
		}


		// 2. 일반 적 스폰
		foreach (var spawn in wave.enemies)
		{
			if (spawn.count <= 0) continue;
			if (!enemyPrefabDict.TryGetValue(spawn.prefabName, out GameObject prefab)) continue;

			Enemy enemyComp = prefab.GetComponentInChildren<Enemy>();
			if (enemyComp == null || enemyComp.enemyData == null) continue;

			for (int i = 0; i < spawn.count; i++)
			{
				if (enemySpawnPoints.Count == 0) break;

				int index = Random.Range(0, enemySpawnPoints.Count);
				Transform spawnPoint = enemySpawnPoints[index];
				enemySpawnPoints.RemoveAt(index);

				Instantiate(prefab, spawnPoint.position, Quaternion.identity);
				usedCoinTotal += enemyComp.enemyData.Coin;
			}
		}


		// 3. 아이템 스폰 (기존 로직 그대로)
		int coinRemain = totalValPoint - usedCoinTotal;

		List<ItemData> validItems = item_date.Where(i => i != null).ToList();
		int minItemCoin = validItems.Min(i => i.ValPoint);

		while (coinRemain >= minItemCoin && itemSpawnPoints.Count > 0)
		{
			List<ItemData> spawnables = validItems.FindAll(i => i.ValPoint <= coinRemain);
			if (spawnables.Count == 0) break;

			ItemData randomItem = spawnables[Random.Range(0, spawnables.Count)];
			int index = Random.Range(0, itemSpawnPoints.Count);
			Transform spawnPoint = itemSpawnPoints[index];
			itemSpawnPoints.RemoveAt(index);

			GameObject itemObj = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
			ItemObject itemObjComp = itemObj.GetComponentInChildren<ItemObject>();
			if (itemObjComp != null)
			{
				itemObjComp.itemDataTemplate = randomItem;
				itemObjComp.itemData = new Item(randomItem);
			}

			coinRemain -= randomItem.ValPoint;
		}
	}

}

