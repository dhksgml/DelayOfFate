using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Item_p : MonoBehaviour
{
	private Player_Item_Use player_Item_Use;
	private PlayerController playerController;

	private float angleStartTime;

	// 패시브 아이템 상태
	public bool[] item_p;
	private float[] itemStartTime;

	void Start()
	{
		player_Item_Use = GetComponent<Player_Item_Use>();
		playerController = GetComponent<PlayerController>();
		angleStartTime = Time.time;

		item_p = new bool[25];
		itemStartTime = new float[25];
	}

	void Update()
	{
		if (player_Item_Use == null) return;
		CheckQuickSlots();
		UpdateItemEffects();
	}

	// 퀵슬롯에 특정 아이템(id=1)이 있는지 검사
	private void CheckQuickSlots()
	{
		foreach (Item item in player_Item_Use.quickSlots)
		{
			if (item != null && item.id == 2)
			{
				item_p[2] = true;
				break;
			}
			if (item != null && item.id == 8)
			{
				item_p[8] = true;
				break;
			}
			if (item != null && item.id == 9)
			{
				item_p[9] = true;
				break;
			}
			if (item != null && item.id == 15)
			{
				item_p[15] = true;
				break;
			}
		}
	}

	// 시간에 따라 아이템 효과 종료
	private void UpdateItemEffects()
	{
		if (item_p[7] && Time.time - itemStartTime[7] >= 80f) {item_p[7] = false; }
		if (item_p[10] && Time.time - itemStartTime[10] >= 80f) {item_p[10] = false; } // 80초 지속
		if (item_p[13] && Time.time - itemStartTime[13] >= 40f) {item_p[13] = false; } // 40초 지속
		if (item_p[18] && Time.time - itemStartTime[18] >= 80f) {item_p[18] = false; }
		if (item_p[19] && Time.time - itemStartTime[19] >= 80f) {item_p[19] = false; }
	}


	// 아이템 판매 시 처리
	public void Sell(int item_id)
	{
		switch (item_id)
		{
			case 1: // 비녀
				player_Item_Use.qlsu = true;
				break;
			case 4: // 축음기
				playerController.DamagedMP(10);
				break;
			case 7: // 사진
				item_p[7] = true;
				itemStartTime[7] = Time.time;
				break;
			case 10: // 팽이
				item_p[10] = true;
				itemStartTime[10] = Time.time;
				break;
			case 13: // 지팡이
				item_p[13] = true;
				itemStartTime[13] = Time.time;
				break;
			case 18: // 주판
				item_p[18] = true;
				itemStartTime[18] = Time.time;
				break;
			case 19: // 책
				item_p[19] = true;
				itemStartTime[19] = Time.time;
				break;
		}
	}
}
