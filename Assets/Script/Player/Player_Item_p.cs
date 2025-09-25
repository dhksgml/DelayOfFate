using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Item_p : MonoBehaviour
{
	private Player_Item_Use player_Item_Use;
	private PlayerController playerController;
	private PlayerInfoUI playerInfoUI;
	private QuickSlotUI quickSlotUI;

	private float angleStartTime;

	// 패시브 아이템 상태
	private bool[] item_p = new bool[4];
	private float[] itemStartTime = new float[4];

	void Start()
	{
		player_Item_Use = GetComponent<Player_Item_Use>();
		playerController = GetComponent<PlayerController>();
		playerInfoUI = FindObjectOfType<PlayerInfoUI>();
		quickSlotUI = FindObjectOfType<QuickSlotUI>();

		angleStartTime = Time.time;
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
			if (item != null && item.id == 1)
			{
				print("테스트");
				break;
			}
		}
	}

	// 시간에 따라 아이템 효과 종료
	private void UpdateItemEffects()
	{
		print(item_p[0]);
		print(Time.time - itemStartTime[0]);
		if (item_p[0] && Time.time - itemStartTime[0] >= 80f) {item_p[0] = false; quickSlotUI.time_ui = true; } // 80초 지속
		if (item_p[1] && Time.time - itemStartTime[1] >= 40f) {item_p[1] = false; } // 40초 지속
		if (item_p[2] && Time.time - itemStartTime[2] >= 80f) {item_p[2] = false; playerInfoUI.coin_ui = true; }
		if (item_p[3] && Time.time - itemStartTime[3] >= 80f) {item_p[3] = false; playerInfoUI.soul_ui = true; }
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
			case 10: // 팽이
				quickSlotUI.time_ui = false;
				item_p[0] = true;
				itemStartTime[0] = Time.time;
				break;
			case 13: // 지팡이
				item_p[1] = true;
				itemStartTime[1] = Time.time;
				break;
			case 18: // 주판
				playerInfoUI.coin_ui = false;
				item_p[2] = true;
				itemStartTime[2] = Time.time;
				break;
			case 19: // 책
				playerInfoUI.soul_ui = false;
				item_p[3] = true;
				itemStartTime[3] = Time.time;
				break;
		}
	}
}
