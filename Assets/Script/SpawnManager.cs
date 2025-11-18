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
		// 0: 어둑쥐(21), 1: 처녀귀신(65)★, 2: 음양(72)★, 3: 불귀신(35)★, 4: 약탈귀(50)★,
		// 5: 소면귀(73)★, 6: 두억시니(250), 7: 죽음장승(107), 8: 석등령(75), 9: 탈혼귀(40)
		// 10: 땅상어(86), 11: 멧돼지(80), 12: 저주인형(30), 13: 그슨대(50)★

		// ★ = 귀신 속성 (목표 금액에서 제외, 보너스 몹)

		// 1일차: 0, 1, 3, 4, 11, 12, 13 (어둑쥐, 처녀귀신, 불귀신, 약탈귀, 멧돼지, 저주인형, 그슨대)
		// 2일차: 0, 1, 2, 3, 4, 9, 10, 11, 12, 13 (어둑쥐, 처녀귀신, 음양, 불귀신, 약탈귀, 탈혼귀, 땅상어, 멧돼지, 저주인형, 그슨대)
		// 3일차: 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12 (어둑쥐, 처녀귀신, 음양, 불귀신, 약탈귀, 소면귀, 죽음장승, 석등령, 탈혼귀, 땅상어, 멧돼지, 저주인형)
		// 4일차: 1, 2, 3, 5, 7, 8, 9, 10, 11 (처녀귀신, 음양, 불귀신, 소면귀, 죽음장승, 석등령, 탈혼귀, 땅상어, 멧돼지)
		// 5일차: 1, 5, 6, 7, 8, 9, 10 (처녀귀신, 소면귀, 두억시니, 죽음장승, 석등령, 탈혼귀, 땅상어)
		// 6일차: 5, 6, 7, 8, 9, 10 (소면귀, 두억시니, 죽음장승, 석등령, 탈혼귀, 땅상어)
		// 7일차: 0-13 전체 (모든 몹 출현 가능)

		Dictionary<int, List<List<List<int>>>> wavePoolByDay = new Dictionary<int, List<List<List<int>>>>()
		{
			// 1일차 목표: 일반몹 ~750 (귀신은 보너스, 최대 20마리)
			// 일반몹: 0(어둑쥐), 11(멧돼지), 12(저주인형)
			// 귀신: 1(처녀귀신), 3(불귀신), 4(약탈귀), 13(그슨대)
			{ 0, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 11, 3 }, new List<int> { 12, 5 }, new List<int> { 1, 2 }, new List<int> { 3, 3 }, new List<int> { 4, 1 }, new List<int> { 13, 2 } }, // 420+240+150 + (130+105+50+100) = 810 + 385(보너스)
				new List<List<int>> { new List<int> { 0, 18 }, new List<int> { 11, 4 }, new List<int> { 12, 4 }, new List<int> { 1, 3 }, new List<int> { 3, 2 }, new List<int> { 4, 2 }, new List<int> { 13, 1 } }, // 378+320+120 + (195+70+100+50) = 818 + 415(보너스)
				new List<List<int>> { new List<int> { 0, 19 }, new List<int> { 11, 2 }, new List<int> { 12, 6 }, new List<int> { 1, 2 }, new List<int> { 3, 4 }, new List<int> { 4, 1 }, new List<int> { 13, 2 } }, // 399+160+180 + (130+140+50+100) = 739 + 420(보너스)
				new List<List<int>> { new List<int> { 0, 17 }, new List<int> { 11, 5 }, new List<int> { 12, 3 }, new List<int> { 1, 3 }, new List<int> { 3, 3 }, new List<int> { 4, 2 }, new List<int> { 13, 2 } }, // 357+400+90 + (195+105+100+100) = 847 + 500(보너스)
				new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 11, 3 }, new List<int> { 12, 4 }, new List<int> { 1, 2 }, new List<int> { 3, 2 }, new List<int> { 4, 2 }, new List<int> { 13, 1 } }, // 420+240+120 + (130+70+100+50) = 780 + 350(보너스)
				new List<List<int>> { new List<int> { 0, 16 }, new List<int> { 11, 4 }, new List<int> { 12, 5 }, new List<int> { 1, 3 }, new List<int> { 3, 3 }, new List<int> { 4, 1 }, new List<int> { 13, 2 } } // 336+320+150 + (195+105+50+100) = 806 + 450(보너스)
			}},

			// 2일차 목표: 일반몹 ~1500 (귀신은 보너스, 최대 20마리)
			// 일반몹: 0(어둑쥐), 9(탈혼귀), 10(땅상어), 11(멧돼지), 12(저주인형)
			// 귀신: 1(처녀귀신), 2(음양), 3(불귀신), 4(약탈귀), 13(그슨대)
			{ 1, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 18 }, new List<int> { 9, 10 }, new List<int> { 10, 5 }, new List<int> { 11, 4 }, new List<int> { 12, 5 }, new List<int> { 1, 3 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 2 }, new List<int> { 13, 2 } }, // 378+400+430+320+150 + (195+72+105+100+100) = 1678 + 572(보너스)
				new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 9, 8 }, new List<int> { 10, 6 }, new List<int> { 11, 3 }, new List<int> { 12, 4 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 2 }, new List<int> { 4, 3 }, new List<int> { 13, 1 } }, // 420+320+516+240+120 + (260+72+70+150+50) = 1616 + 602(보너스)
				new List<List<int>> { new List<int> { 0, 16 }, new List<int> { 9, 12 }, new List<int> { 10, 4 }, new List<int> { 11, 5 }, new List<int> { 12, 3 }, new List<int> { 1, 3 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 4, 1 }, new List<int> { 13, 2 } }, // 336+480+344+400+90 + (195+72+140+50+100) = 1650 + 557(보너스)
				new List<List<int>> { new List<int> { 0, 19 }, new List<int> { 9, 9 }, new List<int> { 10, 5 }, new List<int> { 11, 4 }, new List<int> { 12, 4 }, new List<int> { 1, 3 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 2 }, new List<int> { 13, 2 } }, // 399+360+430+320+120 + (195+72+105+100+100) = 1629 + 572(보너스)
				new List<List<int>> { new List<int> { 0, 17 }, new List<int> { 9, 11 }, new List<int> { 10, 6 }, new List<int> { 11, 3 }, new List<int> { 12, 5 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 2 }, new List<int> { 4, 3 }, new List<int> { 13, 1 } }, // 357+440+516+240+150 + (260+72+70+150+50) = 1703 + 602(보너스)
				new List<List<int>> { new List<int> { 0, 20 }, new List<int> { 9, 10 }, new List<int> { 10, 4 }, new List<int> { 11, 4 }, new List<int> { 12, 4 }, new List<int> { 1, 3 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 2 }, new List<int> { 13, 2 } } // 420+400+344+320+120 + (195+72+105+100+100) = 1604 + 572(보너스)
			}},

			// 3일차 목표: 일반몹 ~2250 (귀신은 보너스, 최대 20마리)
			// 일반몹: 0(어둑쥐), 7(죽음장승), 8(석등령), 9(탈혼귀), 10(땅상어), 11(멧돼지), 12(저주인형)
			// 귀신: 1(처녀귀신), 2(음양), 3(불귀신), 4(약탈귀), 5(소면귀)
			{ 2, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 15 }, new List<int> { 7, 6 }, new List<int> { 8, 5 }, new List<int> { 9, 8 }, new List<int> { 10, 5 }, new List<int> { 11, 3 }, new List<int> { 12, 4 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 2 }, new List<int> { 5, 2 } }, // 315+642+375+320+430+240+120 + (260+72+105+100+146) = 2442 + 683(보너스)
				new List<List<int>> { new List<int> { 0, 18 }, new List<int> { 7, 5 }, new List<int> { 8, 6 }, new List<int> { 9, 7 }, new List<int> { 10, 4 }, new List<int> { 11, 4 }, new List<int> { 12, 3 }, new List<int> { 1, 5 }, new List<int> { 2, 2 }, new List<int> { 3, 2 }, new List<int> { 4, 3 }, new List<int> { 5, 1 } }, // 378+535+450+280+344+320+90 + (325+144+70+150+73) = 2397 + 762(보너스)
				new List<List<int>> { new List<int> { 0, 12 }, new List<int> { 7, 7 }, new List<int> { 8, 5 }, new List<int> { 9, 9 }, new List<int> { 10, 6 }, new List<int> { 11, 3 }, new List<int> { 12, 5 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 4, 2 }, new List<int> { 5, 2 } }, // 252+749+375+360+516+240+150 + (260+72+140+100+146) = 2642 + 718(보너스)
				new List<List<int>> { new List<int> { 0, 16 }, new List<int> { 7, 6 }, new List<int> { 8, 4 }, new List<int> { 9, 8 }, new List<int> { 10, 5 }, new List<int> { 11, 4 }, new List<int> { 12, 3 }, new List<int> { 1, 3 }, new List<int> { 2, 2 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 5, 1 } }, // 336+642+300+320+430+320+90 + (195+144+105+150+73) = 2438 + 667(보너스)
				new List<List<int>> { new List<int> { 0, 14 }, new List<int> { 7, 5 }, new List<int> { 8, 6 }, new List<int> { 9, 9 }, new List<int> { 10, 6 }, new List<int> { 11, 2 }, new List<int> { 12, 4 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 2 }, new List<int> { 5, 2 } }, // 294+535+450+360+516+160+120 + (260+72+105+100+146) = 2435 + 683(보너스)
				new List<List<int>> { new List<int> { 0, 17 }, new List<int> { 7, 6 }, new List<int> { 8, 5 }, new List<int> { 9, 7 }, new List<int> { 10, 5 }, new List<int> { 11, 3 }, new List<int> { 12, 4 }, new List<int> { 1, 5 }, new List<int> { 2, 2 }, new List<int> { 3, 2 }, new List<int> { 4, 2 }, new List<int> { 5, 1 } } // 357+642+375+280+430+240+120 + (325+144+70+100+73) = 2444 + 712(보너스)
			}},

			// 4일차 목표: 일반몹 ~3000 (귀신은 보너스, 최대 20마리)
			// 일반몹: 7(죽음장승), 8(석등령), 9(탈혼귀), 10(땅상어), 11(멧돼지)
			// 귀신: 1(처녀귀신), 2(음양), 3(불귀신), 5(소면귀)
			{ 3, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 7, 8 }, new List<int> { 8, 8 }, new List<int> { 9, 10 }, new List<int> { 10, 7 }, new List<int> { 11, 4 }, new List<int> { 1, 5 }, new List<int> { 2, 2 }, new List<int> { 3, 3 }, new List<int> { 5, 3 } }, // 856+600+400+602+320 + (325+144+105+219) = 2778 + 793(보너스)
				new List<List<int>> { new List<int> { 7, 9 }, new List<int> { 8, 7 }, new List<int> { 9, 11 }, new List<int> { 10, 6 }, new List<int> { 11, 5 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 5, 2 } }, // 963+525+440+516+400 + (390+72+140+146) = 2844 + 748(보너스)
				new List<List<int>> { new List<int> { 7, 7 }, new List<int> { 8, 9 }, new List<int> { 9, 9 }, new List<int> { 10, 8 }, new List<int> { 11, 3 }, new List<int> { 1, 5 }, new List<int> { 2, 2 }, new List<int> { 3, 3 }, new List<int> { 5, 4 } }, // 749+675+360+688+240 + (325+144+105+292) = 2712 + 866(보너스)
				new List<List<int>> { new List<int> { 7, 8 }, new List<int> { 8, 8 }, new List<int> { 9, 12 }, new List<int> { 10, 6 }, new List<int> { 11, 4 }, new List<int> { 1, 4 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 5, 3 } }, // 856+600+480+516+320 + (260+72+140+219) = 2772 + 691(보너스)
				new List<List<int>> { new List<int> { 7, 9 }, new List<int> { 8, 7 }, new List<int> { 9, 10 }, new List<int> { 10, 7 }, new List<int> { 11, 5 }, new List<int> { 1, 6 }, new List<int> { 2, 2 }, new List<int> { 3, 2 }, new List<int> { 5, 2 } }, // 963+525+400+602+400 + (390+144+70+146) = 2890 + 750(보너스)
				new List<List<int>> { new List<int> { 7, 7 }, new List<int> { 8, 9 }, new List<int> { 9, 11 }, new List<int> { 10, 7 }, new List<int> { 11, 3 }, new List<int> { 1, 5 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 5, 3 } } // 749+675+440+602+240 + (325+72+105+219) = 2706 + 721(보너스)
			}},

			// 5일차 목표: 일반몹 ~3750 (귀신은 보너스, 최대 20마리)
			// 일반몹: 6(두억시니), 7(죽음장승), 8(석등령), 9(탈혼귀), 10(땅상어)
			// 귀신: 1(처녀귀신), 5(소면귀)
			{ 4, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 6, 5 }, new List<int> { 7, 8 }, new List<int> { 8, 9 }, new List<int> { 9, 12 }, new List<int> { 10, 7 }, new List<int> { 1, 6 }, new List<int> { 5, 4 } }, // 1250+856+675+480+602 + (390+292) = 3863 + 682(보너스)
				new List<List<int>> { new List<int> { 6, 6 }, new List<int> { 7, 7 }, new List<int> { 8, 8 }, new List<int> { 9, 13 }, new List<int> { 10, 6 }, new List<int> { 1, 7 }, new List<int> { 5, 3 } }, // 1500+749+600+520+516 + (455+219) = 3885 + 674(보너스)
				new List<List<int>> { new List<int> { 6, 4 }, new List<int> { 7, 9 }, new List<int> { 8, 9 }, new List<int> { 9, 11 }, new List<int> { 10, 8 }, new List<int> { 1, 6 }, new List<int> { 5, 5 } }, // 1000+963+675+440+688 + (390+365) = 3766 + 755(보너스)
				new List<List<int>> { new List<int> { 6, 5 }, new List<int> { 7, 8 }, new List<int> { 8, 8 }, new List<int> { 9, 13 }, new List<int> { 10, 7 }, new List<int> { 1, 5 }, new List<int> { 5, 4 } }, // 1250+856+600+520+602 + (325+292) = 3828 + 617(보너스)
				new List<List<int>> { new List<int> { 6, 6 }, new List<int> { 7, 7 }, new List<int> { 8, 9 }, new List<int> { 9, 12 }, new List<int> { 10, 6 }, new List<int> { 1, 7 }, new List<int> { 5, 3 } }, // 1500+749+675+480+516 + (455+219) = 3920 + 674(보너스)
				new List<List<int>> { new List<int> { 6, 4 }, new List<int> { 7, 9 }, new List<int> { 8, 8 }, new List<int> { 9, 12 }, new List<int> { 10, 8 }, new List<int> { 1, 6 }, new List<int> { 5, 5 } } // 1000+963+600+480+688 + (390+365) = 3731 + 755(보너스)
			}},

			// 6일차 목표: 일반몹 ~4500 (귀신은 보너스, 최대 20마리)
			// 일반몹: 6(두억시니), 7(죽음장승), 8(석등령), 9(탈혼귀), 10(땅상어)
			// 귀신: 5(소면귀)
			{ 5, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 6, 7 }, new List<int> { 7, 9 }, new List<int> { 8, 10 }, new List<int> { 9, 14 }, new List<int> { 10, 8 }, new List<int> { 5, 5 } }, // 1750+963+750+560+688 + (365) = 4711 + 365(보너스)
				new List<List<int>> { new List<int> { 6, 8 }, new List<int> { 7, 8 }, new List<int> { 8, 9 }, new List<int> { 9, 15 }, new List<int> { 10, 7 }, new List<int> { 5, 6 } }, // 2000+856+675+600+602 + (438) = 4733 + 438(보너스)
				new List<List<int>> { new List<int> { 6, 6 }, new List<int> { 7, 10 }, new List<int> { 8, 10 }, new List<int> { 9, 13 }, new List<int> { 10, 9 }, new List<int> { 5, 4 } }, // 1500+1070+750+520+774 + (292) = 4614 + 292(보너스)
				new List<List<int>> { new List<int> { 6, 7 }, new List<int> { 7, 9 }, new List<int> { 8, 9 }, new List<int> { 9, 14 }, new List<int> { 10, 8 }, new List<int> { 5, 6 } }, // 1750+963+675+560+688 + (438) = 4636 + 438(보너스)
				new List<List<int>> { new List<int> { 6, 8 }, new List<int> { 7, 8 }, new List<int> { 8, 10 }, new List<int> { 9, 15 }, new List<int> { 10, 7 }, new List<int> { 5, 5 } }, // 2000+856+750+600+602 + (365) = 4808 + 365(보너스)
				new List<List<int>> { new List<int> { 6, 6 }, new List<int> { 7, 10 }, new List<int> { 8, 9 }, new List<int> { 9, 14 }, new List<int> { 10, 9 }, new List<int> { 5, 5 } } // 1500+1070+675+560+774 + (365) = 4579 + 365(보너스)
			}},

			// 7일차 목표: 일반몹 ~5250 (귀신은 보너스, 최대 20마리)
			// 일반몹: 0(어둑쥐), 6(두억시니), 7(죽음장승), 8(석등령), 9(탈혼귀), 10(땅상어), 11(멧돼지), 12(저주인형)
			// 귀신: 1(처녀귀신), 2(음양), 3(불귀신), 4(약탈귀), 5(소면귀), 13(그슨대)
			{ 6, new List<List<List<int>>> {
				new List<List<int>> { new List<int> { 0, 12 }, new List<int> { 6, 7 }, new List<int> { 7, 10 }, new List<int> { 8, 11 }, new List<int> { 9, 15 }, new List<int> { 10, 9 }, new List<int> { 11, 4 }, new List<int> { 12, 5 }, new List<int> { 1, 6 }, new List<int> { 2, 2 }, new List<int> { 3, 4 }, new List<int> { 4, 3 }, new List<int> { 5, 5 }, new List<int> { 13, 3 } }, // 252+1750+1070+825+600+774+320+150 + (390+144+140+150+365+150) = 5741 + 1339(보너스)
				new List<List<int>> { new List<int> { 0, 15 }, new List<int> { 6, 6 }, new List<int> { 7, 9 }, new List<int> { 8, 10 }, new List<int> { 9, 16 }, new List<int> { 10, 8 }, new List<int> { 11, 5 }, new List<int> { 12, 4 }, new List<int> { 1, 7 }, new List<int> { 2, 1 }, new List<int> { 3, 3 }, new List<int> { 4, 4 }, new List<int> { 5, 4 }, new List<int> { 13, 2 } }, // 315+1500+963+750+640+688+400+120 + (455+72+105+200+292+100) = 5376 + 1224(보너스)
				new List<List<int>> { new List<int> { 0, 10 }, new List<int> { 6, 8 }, new List<int> { 7, 10 }, new List<int> { 8, 9 }, new List<int> { 9, 14 }, new List<int> { 10, 10 }, new List<int> { 11, 3 }, new List<int> { 12, 6 }, new List<int> { 1, 6 }, new List<int> { 2, 2 }, new List<int> { 3, 5 }, new List<int> { 4, 2 }, new List<int> { 5, 6 }, new List<int> { 13, 3 } }, // 210+2000+1070+675+560+860+240+180 + (390+144+175+100+438+150) = 5795 + 1397(보너스)
				new List<List<int>> { new List<int> { 0, 14 }, new List<int> { 6, 7 }, new List<int> { 7, 9 }, new List<int> { 8, 11 }, new List<int> { 9, 15 }, new List<int> { 10, 8 }, new List<int> { 11, 4 }, new List<int> { 12, 4 }, new List<int> { 1, 5 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 4, 3 }, new List<int> { 5, 5 }, new List<int> { 13, 2 } }, // 294+1750+963+825+600+688+320+120 + (325+72+140+150+365+100) = 5560 + 1152(보너스)
				new List<List<int>> { new List<int> { 0, 16 }, new List<int> { 6, 6 }, new List<int> { 7, 10 }, new List<int> { 8, 10 }, new List<int> { 9, 17 }, new List<int> { 10, 7 }, new List<int> { 11, 5 }, new List<int> { 12, 3 }, new List<int> { 1, 7 }, new List<int> { 2, 2 }, new List<int> { 3, 3 }, new List<int> { 4, 3 }, new List<int> { 5, 4 }, new List<int> { 13, 3 } }, // 336+1500+1070+750+680+602+400+90 + (455+144+105+150+292+150) = 5428 + 1296(보너스)
				new List<List<int>> { new List<int> { 0, 13 }, new List<int> { 6, 7 }, new List<int> { 7, 9 }, new List<int> { 8, 11 }, new List<int> { 9, 16 }, new List<int> { 10, 9 }, new List<int> { 11, 4 }, new List<int> { 12, 5 }, new List<int> { 1, 6 }, new List<int> { 2, 1 }, new List<int> { 3, 4 }, new List<int> { 4, 4 }, new List<int> { 5, 5 }, new List<int> { 13, 2 } } // 273+1750+963+825+640+774+320+150 + (390+72+140+200+365+100) = 5695 + 1267(보너스)
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
