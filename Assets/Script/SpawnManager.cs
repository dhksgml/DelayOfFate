using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
	public GameObject[] enemyPrefabs;
	public GameObject itemPrefab;
	public ItemData[] item_date;
	public int totalValPoint;

	private List<Transform> enemySpawnPoints = new List<Transform>();
	private List<Transform> itemSpawnPoints = new List<Transform>();

	public List<List<int>> Wave_Data(int day)
	{
		// 0: 어둑쥐(21), 1: 처녀귀신(65), 2: 음양(72), 3: 분열귀(35), 4: 약탈귀(50),
		// 5: 소면귀(73), 6: 두억시니(250), 7: 죽음장승(107), 8: 석등령(75), 9: 탈혼귀(40)
		// 10: 땅상어(86), 11: 멧돼지(80), 12: 저주인형(30), 13: 그슨대(50)

		// 1일차: 0, 1, 3, 4, 11, 12, 13 (어둑쥐, 처녀귀신, 분열귀, 약탈귀, 멧돼지, 저주인형, 그슨대)
		// 2일차: 0, 1, 2, 3, 4, 9, 10, 11, 12, 13 (어둑쥐, 처녀귀신, 음양, 분열귀, 약탈귀, 탈혼귀, 땅상어, 멧돼지, 저주인형, 그슨대)
		// 3일차: 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12 (어둑쥐, 처녀귀신, 음양, 분열귀, 약탈귀, 소면귀, 죽음장승, 석등령, 탈혼귀, 땅상어, 멧돼지, 저주인형)
		// 4일차: 1, 2, 3, 5, 7, 8, 9, 10, 11 (처녀귀신, 음양, 분열귀, 소면귀, 죽음장승, 석등령, 탈혼귀, 땅상어, 멧돼지)
		// 5일차: 1, 5, 6, 7, 8, 9, 10 (처녀귀신, 소면귀, 두억시니, 죽음장승, 석등령, 탈혼귀, 땅상어)
		// 6일차: 5, 6, 7, 8, 9, 10 (소면귀, 두억시니, 죽음장승, 석등령, 탈혼귀, 땅상어)
		// 7일차: 0-13 전체 (모든 몹 출현 가능)

		Dictionary<int, List<List<List<int>>>> wavePoolByDay = new Dictionary<int, List<List<List<int>>>>()
		{
			// 1일차 목표: ~750 (모든 종류 최소 1마리, 최대 9마리)
			{ 0, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 8 }, new List<int> { 1, 3 }, new List<int> { 3, 5 }, new List<int> { 4, 2 }, new List<int> { 11, 1 }, new List<int> { 12, 3 }, new List<int> { 13, 2 } }, // 168+195+175+100+80+90+100 = 908
				new List<List<int>> { new List<int> { 0, 6 }, new List<int> { 1, 4 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 11, 2 }, new List<int> { 12, 2 }, new List<int> { 13, 3 } }, // 126+260+105+150+160+60+150 = 1011
				new List<List<int>> { new List<int> { 0, 10 }, new List<int> { 1, 2 }, new List<int> { 3, 4 }, new List<int> { 4, 2 }, new List<int> { 11, 1 }, new List<int> { 12, 4 }, new List<int> { 13, 1 } }, // 210+130+140+100+80+120+50 = 830
				new List<List<int>> { new List<int> { 0, 7 }, new List<int> { 1, 3 }, new List<int> { 3, 6 }, new List<int> { 4, 1 }, new List<int> { 11, 2 }, new List<int> { 12, 2 }, new List<int> { 13, 2 } }, // 147+195+210+50+160+60+100 = 922
				new List<List<int>> { new List<int> { 0, 9 }, new List<int> { 1, 3 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 11, 1 }, new List<int> { 12, 3 }, new List<int> { 13, 2 } }, // 189+195+105+150+80+90+100 = 909
				new List<List<int>> { new List<int> { 0, 5 }, new List<int> { 1, 4 }, new List<int> { 3, 5 }, new List<int> { 4, 2 }, new List<int> { 11, 2 }, new List<int> { 12, 1 }, new List<int> { 13, 3 } } // 105+260+175+100+160+30+150 = 980
			}},
	
			// 2일차 목표: ~1500 (모든 종류 최소 1마리, 최대 9마리)
			{ 1, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 8 }, new List<int> { 1, 5 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 4, 3 }, new List<int> { 9, 4 }, new List<int> { 10, 2 }, new List<int> { 11, 1 }, new List<int> { 12, 2 }, new List<int> { 13, 1 } }, // 168+325+72+210+150+160+172+80+60+50 = 1447
				new List<List<int>> { new List<int> { 0, 6 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 5 }, new List<int> { 4, 4 }, new List<int> { 9, 5 }, new List<int> { 10, 3 }, new List<int> { 11, 2 }, new List<int> { 12, 1 }, new List<int> { 13, 2 } }, // 126+260+72+175+200+200+258+160+30+100 = 1581
				new List<List<int>> { new List<int> { 0, 7 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 4, 2 }, new List<int> { 9, 6 }, new List<int> { 10, 1 }, new List<int> { 11, 1 }, new List<int> { 12, 3 }, new List<int> { 13, 2 } }, // 147+390+72+140+100+240+86+80+90+100 = 1445
				new List<List<int>> { new List<int> { 0, 9 }, new List<int> { 1, 5 }, new List<int> { 2, 1 }, new List<int> { 3, 7 }, new List<int> { 4, 3 }, new List<int> { 9, 3 }, new List<int> { 10, 2 }, new List<int> { 11, 1 }, new List<int> { 12, 1 }, new List<int> { 13, 1 } }, // 189+325+72+245+150+120+172+80+30+50 = 1433
				new List<List<int>> { new List<int> { 0, 5 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 4, 4 }, new List<int> { 9, 4 }, new List<int> { 10, 3 }, new List<int> { 11, 2 }, new List<int> { 12, 2 }, new List<int> { 13, 2 } }, // 105+260+72+210+200+160+258+160+60+100 = 1585
				new List<List<int>> { new List<int> { 0, 8 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 5 }, new List<int> { 4, 2 }, new List<int> { 9, 5 }, new List<int> { 10, 2 }, new List<int> { 11, 1 }, new List<int> { 12, 2 }, new List<int> { 13, 1 } } // 168+390+72+175+100+200+172+80+60+50 = 1467
			}},
	
			// 3일차 목표: ~2250 (모든 종류 최소 1마리, 최대 9마리)
			{ 2, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 6 }, new List<int> { 1, 6 }, new List<int> { 2, 2 }, new List<int> { 3, 5 }, new List<int> { 4, 3 }, new List<int> { 5, 3 }, new List<int> { 7, 3 }, new List<int> { 8, 2 }, new List<int> { 9, 4 }, new List<int> { 10, 2 }, new List<int> { 11, 1 }, new List<int> { 12, 2 } }, // 126+390+144+175+150+219+321+150+160+172+80+60 = 2147
				new List<List<int>> { new List<int> { 0, 5 }, new List<int> { 1, 5 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 4, 4 }, new List<int> { 5, 2 }, new List<int> { 7, 4 }, new List<int> { 8, 3 }, new List<int> { 9, 3 }, new List<int> { 10, 3 }, new List<int> { 11, 2 }, new List<int> { 12, 1 } }, // 105+325+72+210+200+146+428+225+120+258+160+30 = 2279
				new List<List<int>> { new List<int> { 0, 7 }, new List<int> { 1, 7 }, new List<int> { 2, 2 }, new List<int> { 3, 4 }, new List<int> { 4, 2 }, new List<int> { 5, 3 }, new List<int> { 7, 2 }, new List<int> { 8, 3 }, new List<int> { 9, 5 }, new List<int> { 10, 1 }, new List<int> { 11, 1 }, new List<int> { 12, 2 } }, // 147+455+144+140+100+219+214+225+200+86+80+60 = 2070
				new List<List<int>> { new List<int> { 0, 4 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 7 }, new List<int> { 4, 3 }, new List<int> { 5, 4 }, new List<int> { 7, 3 }, new List<int> { 8, 2 }, new List<int> { 9, 4 }, new List<int> { 10, 2 }, new List<int> { 11, 2 }, new List<int> { 12, 1 } }, // 84+390+72+245+150+292+321+150+160+172+160+30 = 2226
				new List<List<int>> { new List<int> { 0, 6 }, new List<int> { 1, 5 }, new List<int> { 2, 2 }, new List<int> { 3, 5 }, new List<int> { 4, 4 }, new List<int> { 5, 2 }, new List<int> { 7, 4 }, new List<int> { 8, 2 }, new List<int> { 9, 3 }, new List<int> { 10, 3 }, new List<int> { 11, 1 }, new List<int> { 12, 2 } }, // 126+325+144+175+200+146+428+150+120+258+80+60 = 2212
				new List<List<int>> { new List<int> { 0, 5 }, new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 4, 2 }, new List<int> { 5, 3 }, new List<int> { 7, 3 }, new List<int> { 8, 3 }, new List<int> { 9, 4 }, new List<int> { 10, 2 }, new List<int> { 11, 2 }, new List<int> { 12, 1 } } // 105+455+72+210+100+219+321+225+160+172+160+30 = 2229
			}},
	
			// 4일차 목표: ~3000 (모든 종류 최소 1마리, 최대 9마리)
			{ 3, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 1, 8 }, new List<int> { 2, 2 }, new List<int> { 3, 5 }, new List<int> { 5, 4 }, new List<int> { 7, 4 }, new List<int> { 8, 5 }, new List<int> { 9, 6 }, new List<int> { 10, 3 }, new List<int> { 11, 2 } }, // 520+144+175+292+428+375+240+258+160 = 2592
				new List<List<int>> { new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 5, 5 }, new List<int> { 7, 3 }, new List<int> { 8, 6 }, new List<int> { 9, 5 }, new List<int> { 10, 4 }, new List<int> { 11, 1 } }, // 455+72+210+365+321+450+200+344+80 = 2497
				new List<List<int>> { new List<int> { 1, 9 }, new List<int> { 2, 2 }, new List<int> { 3, 4 }, new List<int> { 5, 3 }, new List<int> { 7, 5 }, new List<int> { 8, 4 }, new List<int> { 9, 7 }, new List<int> { 10, 2 }, new List<int> { 11, 2 } }, // 585+144+140+219+535+300+280+172+160 = 2535
				new List<List<int>> { new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 7 }, new List<int> { 5, 4 }, new List<int> { 7, 4 }, new List<int> { 8, 5 }, new List<int> { 9, 6 }, new List<int> { 10, 3 }, new List<int> { 11, 2 } }, // 390+72+245+292+428+375+240+258+160 = 2460
				new List<List<int>> { new List<int> { 1, 8 }, new List<int> { 2, 2 }, new List<int> { 3, 5 }, new List<int> { 5, 5 }, new List<int> { 7, 3 }, new List<int> { 8, 6 }, new List<int> { 9, 5 }, new List<int> { 10, 4 }, new List<int> { 11, 1 } }, // 520+144+175+365+321+450+200+344+80 = 2599
				new List<List<int>> { new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 5, 3 }, new List<int> { 7, 5 }, new List<int> { 8, 4 }, new List<int> { 9, 7 }, new List<int> { 10, 3 }, new List<int> { 11, 2 } } // 455+72+210+219+535+300+280+258+160 = 2489
			}},
	
			// 5일차 목표: ~3750 (모든 종류 최소 1마리, 최대 9마리)
			{ 4, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 1, 9 }, new List<int> { 5, 6 }, new List<int> { 6, 3 }, new List<int> { 7, 5 }, new List<int> { 8, 6 }, new List<int> { 9, 8 }, new List<int> { 10, 4 } }, // 585+438+750+535+450+320+344 = 3422
				new List<List<int>> { new List<int> { 1, 8 }, new List<int> { 5, 5 }, new List<int> { 6, 4 }, new List<int> { 7, 4 }, new List<int> { 8, 7 }, new List<int> { 9, 7 }, new List<int> { 10, 5 } }, // 520+365+1000+428+525+280+430 = 3548
				new List<List<int>> { new List<int> { 1, 9 }, new List<int> { 5, 7 }, new List<int> { 6, 3 }, new List<int> { 7, 6 }, new List<int> { 8, 5 }, new List<int> { 9, 9 }, new List<int> { 10, 3 } }, // 585+511+750+642+375+360+258 = 3481
				new List<List<int>> { new List<int> { 1, 7 }, new List<int> { 5, 6 }, new List<int> { 6, 4 }, new List<int> { 7, 5 }, new List<int> { 8, 6 }, new List<int> { 9, 8 }, new List<int> { 10, 4 } }, // 455+438+1000+535+450+320+344 = 3542
				new List<List<int>> { new List<int> { 1, 9 }, new List<int> { 5, 5 }, new List<int> { 6, 3 }, new List<int> { 7, 6 }, new List<int> { 8, 7 }, new List<int> { 9, 7 }, new List<int> { 10, 5 } }, // 585+365+750+642+525+280+430 = 3577
				new List<List<int>> { new List<int> { 1, 8 }, new List<int> { 5, 7 }, new List<int> { 6, 4 }, new List<int> { 7, 4 }, new List<int> { 8, 6 }, new List<int> { 9, 9 }, new List<int> { 10, 3 } } // 520+511+1000+428+450+360+258 = 3527
			}},
	
			// 6일차 목표: ~4500 (모든 종류 최소 1마리, 최대 9마리)
			{ 5, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 5, 7 }, new List<int> { 6, 5 }, new List<int> { 7, 7 }, new List<int> { 8, 8 }, new List<int> { 9, 9 }, new List<int> { 10, 6 } }, // 511+1250+749+600+360+516 = 3986
				new List<List<int>> { new List<int> { 5, 8 }, new List<int> { 6, 4 }, new List<int> { 7, 8 }, new List<int> { 8, 7 }, new List<int> { 9, 9 }, new List<int> { 10, 7 } }, // 584+1000+856+525+360+602 = 3927
				new List<List<int>> { new List<int> { 5, 6 }, new List<int> { 6, 6 }, new List<int> { 7, 6 }, new List<int> { 8, 9 }, new List<int> { 9, 8 }, new List<int> { 10, 5 } }, // 438+1500+642+675+320+430 = 4005
				new List<List<int>> { new List<int> { 5, 9 }, new List<int> { 6, 4 }, new List<int> { 7, 7 }, new List<int> { 8, 8 }, new List<int> { 9, 9 }, new List<int> { 10, 6 } }, // 657+1000+749+600+360+516 = 3882
				new List<List<int>> { new List<int> { 5, 7 }, new List<int> { 6, 5 }, new List<int> { 7, 8 }, new List<int> { 8, 7 }, new List<int> { 9, 8 }, new List<int> { 10, 7 } }, // 511+1250+856+525+320+602 = 4064
				new List<List<int>> { new List<int> { 5, 8 }, new List<int> { 6, 5 }, new List<int> { 7, 6 }, new List<int> { 8, 9 }, new List<int> { 9, 9 }, new List<int> { 10, 5 } } // 584+1250+642+675+360+430 = 3941
			}},
	
		// 7일차 목표: ~5250 (모든 14종류 몹 최소 1마리, 최대 9마리)
			{ 6, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 4 }, new List<int> { 1, 7 }, new List<int> { 2, 2 }, new List<int> { 3, 5 }, new List<int> { 4, 3 }, new List<int> { 5, 6 }, new List<int> { 6, 6 }, new List<int> { 7, 7 }, new List<int> { 8, 8 }, new List<int> { 9, 8 }, new List<int> { 10, 7 }, new List<int> { 11, 3 }, new List<int> { 12, 2 }, new List<int> { 13, 3 } }, // 84+455+144+175+150+438+1500+749+600+320+602+240+60+150 = 5667
				new List<List<int>> { new List<int> { 0, 5 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 6 }, new List<int> { 4, 4 }, new List<int> { 5, 5 }, new List<int> { 6, 7 }, new List<int> { 7, 6 }, new List<int> { 8, 7 }, new List<int> { 9, 9 }, new List<int> { 10, 6 }, new List<int> { 11, 2 }, new List<int> { 12, 3 }, new List<int> { 13, 2 } }, // 105+390+72+210+200+365+1750+642+525+360+516+160+90+100 = 5485
				new List<List<int>> { new List<int> { 0, 6 }, new List<int> { 1, 8 }, new List<int> { 2, 2 }, new List<int> { 3, 4 }, new List<int> { 4, 2 }, new List<int> { 5, 7 }, new List<int> { 6, 5 }, new List<int> { 7, 8 }, new List<int> { 8, 6 }, new List<int> { 9, 7 }, new List<int> { 10, 8 }, new List<int> { 11, 3 }, new List<int> { 12, 1 }, new List<int> { 13, 4 } }, // 126+520+144+140+100+511+1250+856+450+280+688+240+30+200 = 5535
				new List<List<int>> { new List<int> { 0, 3 }, new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 5 }, new List<int> { 4, 3 }, new List<int> { 5, 6 }, new List<int> { 6, 7 }, new List<int> { 7, 7 }, new List<int> { 8, 9 }, new List<int> { 9, 8 }, new List<int> { 10, 6 }, new List<int> { 11, 2 }, new List<int> { 12, 2 }, new List<int> { 13, 3 } }, // 63+455+72+175+150+438+1750+749+675+320+516+160+60+150 = 5733
				new List<List<int>> { new List<int> { 0, 5 }, new List<int> { 1, 6 }, new List<int> { 2, 2 }, new List<int> { 3, 6 }, new List<int> { 4, 3 }, new List<int> { 5, 5 }, new List<int> { 6, 6 }, new List<int> { 7, 6 }, new List<int> { 8, 8 }, new List<int> { 9, 9 }, new List<int> { 10, 7 }, new List<int> { 11, 3 }, new List<int> { 12, 2 }, new List<int> { 13, 2 } }, // 105+390+144+210+150+365+1500+642+600+360+602+240+60+100 = 5468
				new List<List<int>> { new List<int> { 0, 4 }, new List<int> { 1, 8 }, new List<int> { 2, 1 }, new List<int> { 3, 5 }, new List<int> { 4, 4 }, new List<int> { 5, 6 }, new List<int> { 6, 6 }, new List<int> { 7, 7 }, new List<int> { 8, 7 }, new List<int> { 9, 8 }, new List<int> { 10, 8 }, new List<int> { 11, 2 }, new List<int> { 12, 3 }, new List<int> { 13, 3 } } // 84+520+72+175+200+438+1500+749+525+320+688+160+90+150 = 5671
			}},
		};

		if (!wavePoolByDay.ContainsKey(day))
		{
			Debug.LogWarning($"[Wave_Data] day {day}는 정의되어 있지 않습니다.");
			return new List<List<int>>();
		}

		List<List<List<int>>> pool = wavePoolByDay[day];
		int index = Random.Range(0, pool.Count);
		return pool[index];
	}

	public void SpawnWave_ByPattern(int day)
	{
		enemySpawnPoints.Clear();
		itemSpawnPoints.Clear();

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyPoint"))
		{
			print("스폰직전찾음");
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

		//Debug.Log($"[아이템 소환] 전체 포인트: {totalValPoint}, 몬스터 사용 포인트: {usedCoinTotal}, 남은 포인트: {coinRemain}");
		//Debug.Log($"[아이템 소환] 사용 가능한 아이템 개수: {validItems.Count}");
		if (validItems.Count == 0) return;

		int minItemCoin = validItems.Min(i => i.Coin);
		//Debug.Log($"[아이템 소환] 가장 저렴한 아이템 포인트: {minItemCoin}");

		while (coinRemain >= minItemCoin && itemSpawnPoints.Count > 0)
		{
			List<ItemData> spawnables = validItems.FindAll(i => i.Coin <= coinRemain);
			//Debug.Log($"[아이템 소환] 현재 남은 포인트: {coinRemain}, 생성 가능한 아이템 수: {spawnables.Count}, 남은 스폰 지점 수: {itemSpawnPoints.Count}");

			if (spawnables.Count == 0)
			{
				//Debug.Log("[아이템 소환] 남은 포인트로 생성 가능한 아이템이 없습니다.");
				break;
			}

			ItemData randomItem = spawnables[Random.Range(0, spawnables.Count)];
			int index = Random.Range(0, itemSpawnPoints.Count);
			Transform spawnPoint = itemSpawnPoints[index];
			itemSpawnPoints.RemoveAt(index);

			GameObject itemObj = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
			ItemObject itemObjComp = itemObj.GetComponentInChildren<ItemObject>();

			//Debug.Log($"[아이템 소환] 아이템 생성: {randomItem.name}, 위치: {spawnPoint.position}, 소모 포인트: {randomItem.Coin}");

			if (itemObjComp != null)
			{
				itemObjComp.itemDataTemplate = randomItem;
				itemObjComp.itemData = new Item(randomItem);
			}
			else
			{
				//Debug.LogWarning("[아이템 소환] 프리팹에 ItemObject 컴포넌트가 없습니다.");
			}

			coinRemain -= randomItem.Coin;
		}

	}
}
