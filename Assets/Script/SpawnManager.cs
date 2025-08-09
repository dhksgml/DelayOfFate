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
		// 0: 어둑쥐(21), 1: 처녀귀신(65), 2: 음양(72), 3: 분열귀(100), 4: 약탈귀(50),
		// 5: 소면귀(73), 6: 두억시니(250), 7: 죽음장승(107), 8: 석등령(75), 9: 탈혼귀(40)

		// 1일차: 0, 1, 3, 4 (어둑쥐, 처녀귀신, 분열귀, 약탈귀)
		// 2일차: 0, 1, 2, 3, 4, 9 (어둑쥐, 처녀귀신, 음양, 분열귀, 약탈귀, 탈혼귀)
		// 3일차: 0, 1, 2, 3, 4, 5, 7, 8, 9 (어둑쥐, 처녀귀신, 음양, 분열귀, 약탈귀, 소면귀, 죽음장승, 석등령, 탈혼귀)
		// 4일차: 1, 2, 3, 5, 7, 8, 9 ( 처녀귀신, 음양, 분열귀, 소면귀, 죽음장승, 석등령, 탈혼귀)
		// 5일차: 1, 5, 6, 7, 8, 9 (처녀귀신, 소면귀, 두억시니, 죽음장승, 석등령, 탈혼귀)

		Dictionary<int, List<List<List<int>>>> wavePoolByDay = new Dictionary<int, List<List<List<int>>>>()
		{
			{ 0, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 18 }, new List<int> { 1, 3 }, new List<int> { 3, 2 }, new List<int> { 4, 3 } }, // 18*21 + 3*65 + 2*100 + 3*50 = 378 + 195 + 200 + 150 = 923
				new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 1, 2 }, new List<int> { 3, 1 }, new List<int> { 4, 4 } }, // 420 + 130 + 100 + 200 = 850
				new List<List<int>> { new List<int> { 0, 19 }, new List<int> { 1, 2 }, new List<int> { 3, 2 }, new List<int> { 4, 3 } }, // 399 + 260 + 200 + 150 = 1009
				new List<List<int>> { new List<int> { 0, 15 }, new List<int> { 1, 4 }, new List<int> { 3, 1 }, new List<int> { 4, 5 } }, // 315 + 130 + 100 + 250 = 795
				new List<List<int>> { new List<int> { 0, 13 }, new List<int> { 1, 4 }, new List<int> { 3, 3 }, new List<int> { 4, 3 } }, // 273 + 260 + 300 + 150 = 983
				new List<List<int>> { new List<int> { 0, 22 }, new List<int> { 1, 3 }, new List<int> { 3, 2 }, new List<int> { 4, 1 } } // 462 + 195 + 200 + 50 = 907
			}},
			{ 1, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 15 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 2 }, new List<int> { 4, 4 }, new List<int> { 9, 4 } }, // 315 + 390 + 72 + 200 + 200 + 160 = 1337
				new List<List<int>> { new List<int> { 0, 17 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 9, 4 } }, // 357 + 260 + 72 + 300 + 150 + 160 = 1299
				new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 1, 3 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 2 }, new List<int> { 9, 4 } }, // 420 + 195 + 72 + 300 + 100 + 160 = 1247
				new List<List<int>> { new List<int> { 0, 25 }, new List<int> { 1, 2 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 4, 2 }, new List<int> { 9, 4 } }, // 525 + 130 + 72 + 400 + 100 + 160 = 1387
				new List<List<int>> { new List<int> { 0, 15 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 2 }, new List<int> { 4, 4 }, new List<int> { 9, 4 } }, // 315 + 390 + 72 + 200 + 200 + 160 = 1337
				new List<List<int>> { new List<int> { 0, 18 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 9, 4 } }, // 378 + 260 + 72 + 300 + 150 + 160 = 1320
			}},
			{ 2, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 10 }, new List<int> { 1, 5 }, new List<int> { 3, 4 }, new List<int> { 4, 4 }, new List<int> { 5, 2 }, new List<int> { 7, 2 }, new List<int> { 8, 1 }, new List<int> { 9, 5 } }, // 210 + 325 + 400 + 200 + 146 + 214 + 150 + 200 = 1845
				new List<List<int>> { new List<int> { 0, 15 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 5, 1 }, new List<int> { 7, 2 }, new List<int> { 9, 6 } }, // 315 + 260 + 72 + 300 + 150 + 73 + 214 + 240 = 1624
				new List<List<int>> { new List<int> { 0, 5 }, new List<int> { 1, 6 }, new List<int> { 3, 4 }, new List<int> { 4, 5 }, new List<int> { 5, 3 }, new List<int> { 7, 2 }, new List<int> { 8, 1 }, new List<int> { 9, 4 } }, // 105 + 390 + 400 + 250 + 219 + 214 + 150 + 160 = 1888
				new List<List<int>> { new List<int> { 0, 7 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 4, 3 }, new List<int> { 5, 2 }, new List<int> { 7, 2 }, new List<int> { 8, 2 }, new List<int> { 9, 4 } }, // 147 + 390 + 72 + 400 + 150 + 146 + 214 + 150 + 160 = 1829
				new List<List<int>> { new List<int> { 0, 12 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 5, 2 }, new List<int> { 8, 2 }, new List<int> { 9, 6 } }, // 252 + 260 + 72 + 300 + 150 + 146 + 150 + 240 = 1570
				new List<List<int>> { new List<int> { 0, 10 }, new List<int> { 1, 5 }, new List<int> { 3, 4 }, new List<int> { 4, 4 }, new List<int> { 5, 2 }, new List<int> { 7, 2 }, new List<int> { 8, 1 }, new List<int> { 9, 5 } }  // 210 + 325 + 400 + 200 + 146 + 214 + 150 + 200 = 1845
			}},
			{ 3, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 5, 4 }, new List<int> { 7, 3 }, new List<int> { 8, 6 }, new List<int> { 9, 6 } }, // 390 + 72 + 600 + 292 + 321 + 450 + 280 = 2405
				new List<List<int>> { new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 5 }, new List<int> { 5, 3 }, new List<int> { 7, 4 }, new List<int> { 8, 5 }, new List<int> { 9, 7 } }, // 455 + 72 + 500 + 219 + 428 + 375 + 280 = 2329
				new List<List<int>> { new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 5, 2 }, new List<int> { 7, 5 }, new List<int> { 8, 6 }, new List<int> { 9, 6 } }, // 390 + 72 + 600 + 146 + 535 + 450 + 280 = 2473
				new List<List<int>> { new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 5 }, new List<int> { 5, 4 }, new List<int> { 7, 3 }, new List<int> { 8, 6 }, new List<int> { 9, 7 } }, // 455 + 72 + 500 + 292 + 321 + 450 + 280 = 2370
				new List<List<int>> { new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 5, 3 }, new List<int> { 7, 4 }, new List<int> { 8, 5 }, new List<int> { 9, 7 } }, // 390 + 72 + 600 + 219 + 428 + 375 + 280 = 2364
				new List<List<int>> { new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 5 }, new List<int> { 5, 2 }, new List<int> { 7, 5 }, new List<int> { 8, 6 }, new List<int> { 9, 6 } }, // 455 + 72 + 500 + 146 + 535 + 450 + 280 = 2438
			}},
			{ 4, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 1, 10 }, new List<int> { 5, 5 }, new List<int> { 6, 3 }, new List<int> { 7, 6 }, new List<int> { 8, 6 }, new List<int> { 9, 12 } }, // 650 + 365 + 750 + 535 + 375 + 400 = 3075
			}},
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
