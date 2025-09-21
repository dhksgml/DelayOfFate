using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Item_p : MonoBehaviour //플레이어 아이템 패시브 발동 관련 코드들
{
	private Player_Item_Use player_Item_Use;

	void Start()
	{
		player_Item_Use = GetComponent<Player_Item_Use>();
	}

	void Update()
	{
		if (player_Item_Use == null) return;

		// 퀵슬롯 검사
		foreach (Item item in player_Item_Use.quickSlots)
		{
			if (item != null) // id == 001 → int로 1
			{
				print("테스트");
				break; // 한 번만 찍고 종료
			}
		}

		// 무기 슬롯 검사
		/*foreach (Item item in player_Item_Use.weaponSlots)
		{
			if (item != null && item.id == 1)
			{
				print("테스트");
				break;
			}
		}*/
	}
}
