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
	public int[] item_p_count; // bool 대신 int 배열로 개수 저장

	private void Awake()
    {
		player_Item_Use = GetComponent<Player_Item_Use>();
		playerController = GetComponent<PlayerController>();
		angleStartTime = Time.time;
		item_p = new bool[25]; // 기존 bool 배열 (있는지 없는지만 체크)
		item_p_count = new int[25]; // 개수 저장용 배열 추가
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
		// 먼저 모든 카운트 초기화
		for (int i = 0; i < item_p_count.Length; i++)
		{
			item_p_count[2] = 0;item_p[2] = false;
			item_p_count[6] = 0; item_p[6] = false;
			item_p_count[8] = 0; item_p[8] = false;
			item_p_count[9] = 0; item_p[9] = false;
			item_p_count[11] = 0; item_p[11] = false;
			item_p_count[12] = 0; item_p[12] = false;
			item_p_count[14] = 0; item_p[14] = false;
			item_p_count[15] = 0; item_p[15] = false;
			item_p_count[16] = 0; item_p[16] = false;
			item_p_count[17] = 0; item_p[17] = false;
		}

		// 인벤토리 4칸 모두 체크
		foreach (Item item in player_Item_Use.quickSlots)
		{
			if (item != null)
			{
				// 해당 id의 개수 증가
				if (item.id == 2)
				{
					item_p[2] = true;
					item_p_count[2]++;
				}
				else if (item.id == 6)
				{
					item_p[6] = true;
					item_p_count[6]++;
				}
				else if (item.id == 8)
				{
					item_p[8] = true;
					item_p_count[8]++;
				}
				else if (item.id == 9)
				{
					item_p[9] = true;
					item_p_count[9]++;
				}
				else if (item.id == 11)
				{
					item_p[11] = true;
					item_p_count[11]++;
				}
				else if (item.id == 12)
				{
					item_p[12] = true;
					item_p_count[12]++;
				}
				else if (item.id == 14)
				{
					item_p[14] = true;
					item_p_count[14]++;
				}
				else if (item.id == 15)
				{
					item_p[15] = true;
					item_p_count[15]++;
				}
				else if (item.id == 16)
				{
					item_p[16] = true;
					item_p_count[16]++;
				}
				else if (item.id == 17)
				{
					item_p[17] = true;
					item_p_count[17]++;
				}
			}
		}
	}
	// 시간에 따라 아이템 효과 종료
	private void UpdateItemEffects()
	{
		//if (item_p[13] == true)
			//Debug.Log($"지팡이 활성중... 경과 시간: {Time.time - itemStartTime[13]}");

		if (item_p[7] && Time.time - itemStartTime[7] >= 80f) {item_p[7] = false; }
		if (item_p[10] && Time.time - itemStartTime[10] >= 80f) {item_p[10] = false; }
		if (item_p[13] && Time.time - itemStartTime[13] >= 20f) 
		{ 
			Debug.Log("판매효과 끝"); 
			item_p[13] = false; 
		} // 20초 지속
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
				Debug.Log($"사진 판매됨 — 시작 시간: {itemStartTime[7]}");
				break;
			case 10: // 팽이
				item_p[10] = true;
				itemStartTime[10] = Time.time;
				break;
			case 13: // 지팡이
				item_p[13] = true;
				itemStartTime[13] = Time.time;
				Debug.Log($"지팡이 판매됨 — 시작 시간: {itemStartTime[13]}");
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
