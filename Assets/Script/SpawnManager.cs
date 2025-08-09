using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
	public GameObject[] enemyPrefabs;
	public GameObject itemPrefab;
	public ItemData[] item_date;
	[HideInInspector] public int totalValPoint;

	private List<Transform> enemySpawnPoints = new List<Transform>();
	private List<Transform> itemSpawnPoints = new List<Transform>();

	void Start()
	{
		//SpawnWave_ByPattern(GameManager.Instance.Day - 1);
	}

	public List<List<int>> Wave_Data(int day)
	{
		// 0: 분열귀(5), 1: 어둑쥐(10), 2: 양(1), 3: 약탈귀(5), 4: 처녀귀신(7)
		// 5: 죽음장승(5), 6: 석등령(5), 7: 탈혼귀(7), 8: 두억시니(3)
		Dictionary<int, List<List<List<int>>>> wavePoolByDay = new Dictionary<int, List<List<List<int>>>>()
		{
			{ 0, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 10 } },
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 15 } },
				new List<List<int>> { new List<int> { 1, 20 } },
				new List<List<int>> { new List<int> { 1, 14 }, new List<int> { 3, 3 } },
				new List<List<int>> { new List<int> { 1, 13 }, new List<int> { 3, 3 }, new List<int> { 4, 1 } },
				new List<List<int>> { new List<int> { 1, 13 }, new List<int> { 3, 5 }, new List<int> { 4, 1 } }
			}},
			{ 1, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 10 } }, // 115 + 210 = 525
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 6 }, new List<int> { 4, 1 } }, // 115 + 246 + 85 = 446
				new List<List<int>> { new List<int> { 1, 5 }, new List<int> { 2, 1 }, new List<int> { 4, 2 } }, 
				new List<List<int>> { new List<int> { 1, 5 }, new List<int> { 3, 3 }, new List<int> { 4, 1 } }, // 205 + 150 + 85 = 440
				new List<List<int>> { new List<int> { 1, 7 }, new List<int> { 3, 2 }, new List<int> { 2, 1 } }, // 287 + 100 + 82 = 469
				new List<List<int>> { new List<int> { 1, 9 }, new List<int> { 3, 3 }, new List<int> { 4, 1 } }  // 369 + 150 + 85 = 604
			}},
			{ 2, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 10 }, new List<int> { 2, 1 }, new List<int> { 4, 2 } }, // 115 + 210 + 82 + 170 = 777
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 8 }, new List<int> { 2, 1 }, new List<int> { 4, 1 } },
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 3 }, new List<int> { 3, 4 }, new List<int> { 4, 3 } }, // 115 + 123 + 200 + 255 = 693
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 7 }, new List<int> { 3, 3 }, new List<int> { 4, 1 } },
				new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 1, 5 }, new List<int> { 3, 5 }, new List<int> { 4, 2 } }, // 115 + 205 + 250 + 170 = 740
				new List<List<int>> { new List<int> { 1, 10 }, new List<int> { 2, 1 }, new List<int> { 3, 5 } } // 210 + 82 + 250 = 742
			}}
		};


		if (!wavePoolByDay.ContainsKey(day))
		{
			Debug.LogWarning($"[Wave_Data] day {day}는 정의되어 있지 않습니다.");
			return new List<List<int>>();
		}

		List<List<List<int>>> pool = wavePoolByDay[day];
		int index = Random.Range(0, pool.Count);
		Debug.Log($"[SpawnWave_ByPattern] Day {day + 1} - Selected Pattern Index: {index}");
		return pool[index];
	}

	public void SpawnWave_ByPattern(int day)
	{
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

		List<List<int>> pattern = Wave_Data(day);
		if (pattern == null || pattern.Count == 0) return;

		int usedCoinTotal = 0;

		// 일반 몬스터 소환
		foreach (var enemyInfo in pattern)
		{
			int prefabIndex = enemyInfo[0];
			int count = enemyInfo[1];

			if (prefabIndex < 0 || prefabIndex >= enemyPrefabs.Length) continue;
			GameObject prefab = enemyPrefabs[prefabIndex];

			for (int i = 0; i < count; i++)
			{
				if (enemySpawnPoints.Count == 0) break;

				int index = Random.Range(0, enemySpawnPoints.Count);
				Transform spawnPoint = enemySpawnPoints[index];
				enemySpawnPoints.RemoveAt(index);

				GameObject enemyObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
				Enemy enemyScript = enemyObj.GetComponentInChildren<Enemy>();
				if (enemyScript == null || enemyScript.enemyData == null) continue;

				enemyScript.enemyMobType = EnemyMobType.Normal;
				enemyScript.EnemyInt();

				usedCoinTotal += enemyScript.enemyData.Coin;
			}
		}

		// -------------------
		// 중간보스 소환 예시 - 현재 코드에 맞게 변형
		// -------------------

		// 예시용 중간보스 정보 (외부에서 받아와야 함)
		bool hasMidBoss = true; // 중간보스 존재 여부 (실제론 외부 변수 또는 파라미터)
		int midBossCount = 1; // 중간보스 수량
		int midBossPrefabIndex = 0; // enemyPrefabs에서 중간보스가 있다고 가정

		if (hasMidBoss && midBossCount > 0)
		{
			if (midBossPrefabIndex >= 0 && midBossPrefabIndex < enemyPrefabs.Length)
			{
				GameObject bossPrefab = enemyPrefabs[midBossPrefabIndex];

				if (enemySpawnPoints.Count > 0)
				{
					int index = Random.Range(0, enemySpawnPoints.Count);
					Transform spawnPoint = enemySpawnPoints[index];
					enemySpawnPoints.RemoveAt(index);

					GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
					Enemy bossComp = boss.GetComponentInChildren<Enemy>();

					if (bossComp != null && bossComp.enemyData != null)
					{
						bossComp.enemyMobType = EnemyMobType.MiddleBoss;
						bossComp.EnemyInt();

						usedCoinTotal += bossComp.enemyData.Coin;

						if (bossComp.sp != null)
						{
							bossComp.sp.color = new Color(1f, 0f, 0f);
						}

						midBossCount--;
					}
				}
			}
		}

		// 남은 코인으로 아이템 소환
		int coinRemain = totalValPoint /*- usedCoinTotal*/;
		List<ItemData> validItems = item_date.Where(i => i != null).ToList();

		Debug.Log($"[아이템 소환] 전체 포인트: {totalValPoint}, 몬스터 사용 포인트: {usedCoinTotal}, 남은 포인트: {coinRemain}");
		Debug.Log($"[아이템 소환] 사용 가능한 아이템 개수: {validItems.Count}");
		if (validItems.Count == 0) return;

		int minItemCoin = validItems.Min(i => i.ValPoint);
		Debug.Log($"[아이템 소환] 가장 저렴한 아이템 포인트: {minItemCoin}");

		while (coinRemain >= minItemCoin && itemSpawnPoints.Count > 0)
		{
			List<ItemData> spawnables = validItems.FindAll(i => i.ValPoint <= coinRemain);
			Debug.Log($"[아이템 소환] 현재 남은 포인트: {coinRemain}, 생성 가능한 아이템 수: {spawnables.Count}, 남은 스폰 지점 수: {itemSpawnPoints.Count}");

			if (spawnables.Count == 0)
			{
				Debug.Log("[아이템 소환] 남은 포인트로 생성 가능한 아이템이 없습니다.");
				break;
			}

			ItemData randomItem = spawnables[Random.Range(0, spawnables.Count)];
			int index = Random.Range(0, itemSpawnPoints.Count);
			Transform spawnPoint = itemSpawnPoints[index];
			itemSpawnPoints.RemoveAt(index);

			GameObject itemObj = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
			ItemObject itemObjComp = itemObj.GetComponentInChildren<ItemObject>();

			Debug.Log($"[아이템 소환] 아이템 생성: {randomItem.name}, 위치: {spawnPoint.position}, 소모 포인트: {randomItem.ValPoint}");

			if (itemObjComp != null)
			{
				itemObjComp.itemDataTemplate = randomItem;
				itemObjComp.itemData = new Item(randomItem);
			}
			else
			{
				Debug.LogWarning("[아이템 소환] 프리팹에 ItemObject 컴포넌트가 없습니다.");
			}

			coinRemain -= randomItem.ValPoint;
		}

	}
}
