using System.Collections.Generic;
using UnityEngine;

public class PassiveItemManager : MonoBehaviour
{
	public static PassiveItemManager Instance { get; private set; }

	public List<PassiveItemData> passiveItems;
	public Sprite[] Passive_Item_Icon_1;
	public Sprite[] Passive_Item_Icon_2;
	public Sprite[] Passive_Item_Icon_3;
	public Sprite[] Passive_Item_Icon_4;
	public Sprite[] Passive_Item_Icon_5;
	public Sprite[] Passive_Item_Icon_6;
	public Sprite[] Passive_Item_Icon_7;

	private List<IPassiveEffect> activeEffects = new();

	void Awake()
	{
		// 현재 씬에 자신과 같은 타입의 오브젝트가 2개 이상 있는 경우 즉시 삭제
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		passiveItems = new List<PassiveItemData>();

		// 종류별 2개씩 총 7종류
		for (int g = 1; g <= 7; g++)
		{
			for (int n = 1; n <= 2; n++)
			{
				string id = $"Soul_Add_{g}_{n}";
				string name = GetPassiveName(g, n);
				string desc = GetPassiveDescription(g, n);
				int emdrmq = GetPassiveEmdrmq(g, n);

				bool purchased = PlayerPrefs.GetInt(id, 0) == 1;

				passiveItems.Add(new PassiveItemData
				{
					id = id,
					itemName = name,
					description = desc,
					isPurchased = purchased,
					rating = emdrmq
				});

				if (purchased)
					ApplyPassiveEffect(id);
			}
		}
		for (int n = 2; n <= 6; n+=2)
		{
			string a_id = $"Soul_Add_{n}_{3}";
			string a_name = GetPassiveName(n, 3);
			string a_desc = GetPassiveDescription(n, 3);
			int emdrmq = GetPassiveEmdrmq(n, 3);

			bool a_purchased = PlayerPrefs.GetInt(a_id, 0) == 1;

			passiveItems.Add(new PassiveItemData
			{
				id = a_id,
				itemName = a_name,
				description = a_desc,
				isPurchased = a_purchased,
				rating = emdrmq
			});

			if (a_purchased) ApplyPassiveEffect(a_id);
		}
	}

	void Start()
	{
		// 저장된 값 불러오기
		foreach (var item in passiveItems)
		{
			if (PlayerPrefs.GetInt(item.id, 0) == 1)
			{
				item.isPurchased = true;
				ApplyPassiveEffect(item.id);
			}
		}
	}
	string GetPassiveName(int group, int number)
	{
		switch ($"{group}_{number}")
		{
			case "1_1": return "천하장사";
			case "1_2": return "정정당당";
			case "2_1": return "문전박대";
			case "2_2": return "백발백중";
			case "2_3": return "쾌도난마";
			case "3_1": return "금의환향";
			case "3_2": return "다다익선";
			case "4_1": return "금강불괴";
			case "4_2": return "외강내유";
			case "4_3": return "외유내강";
			case "5_1": return "가담항설";
			case "5_2": return "취사선택";
			case "6_1": return "등용문";
			case "6_2": return "승승장구";
			case "6_3": return "선견지명";
			case "7_1": return "구사일생";
			case "7_2": return "궁여지책";
			// ...
			default: return "알 수 없음";
		}
	}
	string GetPassiveDescription(int group, int number)
	{
		switch ($"{group}_{number}")
		{
			case "1_1": return "물건의 무게로 인한 이동속도 감속이 제거됨";
			case "1_2": return "악귀에게 주는 피해가 10할 증가 악귀의 약점을 공격 할 수 없음";
			case "2_1": return "방망이의 공격 범위가 10할 증가 방망이가 즉시 시전 방망이의 피해량이 5할 증가";
			case "2_2": return "부적의 추격 범위가 20할 증가 부적의 피해량이 5할 증가";
			case "2_3": return "환도의 공격속도 5할 증가 환도의 피해가 5할 증가";
			case "3_1": return "약값 지불 후 보유한 냥의 1할 만큼 획득";
			case "3_2": return "무기를 0혼 으로 구매 가능";
			case "4_1": return "악귀로 받는 체력피해가 5할 감소";
			case "4_2": return "체력이 75 증가 정신이 25 감소";
			case "4_3": return "정신이 75 증가 체력이 25 감소";
			case "5_1": return "가장 가까운 물건의 위치를 파악함";
			case "5_2": return "비어 있는 손 만큼 이동속도 1할 증가";
			case "6_1": return "즉시 판매시 이동속도 1할 증가 (최대 3할) 하루가 지나면 초기화";
			case "6_2": return "악귀를 처치시 영구적으로 피해량 1할 증가";
			case "6_3": return "하루가 지날때 들고 있던 물건의 가치가 5할 증가";
			case "7_1": return "부활 시 물건을 떨어뜨리지 않고 부활";
			case "7_2": return "18각 이상시 달리기 속도가 5할 증가";
			// ...
			default: return "설명이 없습니다.";
		}
	}
	int GetPassiveEmdrmq(int group, int number) //아이템의 등급
	{
		switch ($"{group}_{number}")
		{
			case "1_1": return 1;
			case "1_2": return 3;
			case "2_1": return 1;
			case "2_2": return 1;
			case "2_3": return 1;
			case "3_1": return 3;
			case "3_2": return 1;
			case "4_1": return 4;
			case "4_2": return 1;
			case "4_3": return 1;
			case "5_1": return 1;
			case "5_2": return 2;
			case "6_1": return 1;
			case "6_2": return 2;
			case "6_3": return 3;
			case "7_1": return 2;
			case "7_2": return 1;
			// ...
			default: return 0;
		}
	}
	public void PurchaseItem(string itemId)
	{
		PassiveItemData item = passiveItems.Find(i => i.id == itemId);
		if (item != null && !item.isPurchased)
		{
			item.isPurchased = true;
			PlayerPrefs.SetInt(item.id, 1);
			ApplyPassiveEffect(itemId);
		}
	}
	public bool IsPurchased(string id)
	{
		var item = passiveItems.Find(i => i.id == id);
		return item != null && item.isPurchased;
	}
	public Sprite GetIcon(int group, int number)
	{
		Sprite[] groupArray = null;
		switch (group)
		{
			case 1: groupArray = Passive_Item_Icon_1; break;
			case 2: groupArray = Passive_Item_Icon_2; break;
			case 3: groupArray = Passive_Item_Icon_3; break;
			case 4: groupArray = Passive_Item_Icon_4; break;
			case 5: groupArray = Passive_Item_Icon_5; break;
			case 6: groupArray = Passive_Item_Icon_6; break;
			case 7: groupArray = Passive_Item_Icon_7; break;
		}

		if (groupArray == null || number < 1 || number > groupArray.Length)
		{
			Debug.LogWarning($"GetIcon 실패: group={group}, number={number}");
			return null;
		}

		return groupArray[number - 1];
	}

	public bool HasEffect(string id)//값 불러오기
	{
		var item = passiveItems.Find(i => i.id == id);
		return item != null && item.isPurchased;
	}
	void ApplyPassiveEffect(string itemId)
	{
		switch (itemId)
		{
			case "Soul_Add_1_1":// 
				break;
			case "Soul_Add_1_2":// 
				DoPassive_1_2();
				break;
			case "Soul_Add_1_3":// 
				break;
			case "Soul_Add_2_1":// 
				break;
			case "Soul_Add_2_2":// 
				break;
			case "Soul_Add_2_3":// 
				break;
			case "Soul_Add_3_1":// 
				DoPassive_3_1();
				break;
			case "Soul_Add_3_2":// 
				break;
			case "Soul_Add_3_3":// 
				break;
			case "Soul_Add_4_1":// 
				DoPassive_4_1();
				break;
			case "Soul_Add_4_2":// 
				DoPassive_4_2();
				break;
			case "Soul_Add_4_3":// 
				DoPassive_4_3();
				break;
			case "Soul_Add_5_1":// 
				break;
			case "Soul_Add_5_2":// 
				DoPassive_5_2();
				break;
			case "Soul_Add_5_3":// 
				break;
			case "Soul_Add_6_1":// 
				break;
			case "Soul_Add_6_2":// 
				break;
			case "Soul_Add_6_3":// 
				break;
			case "Soul_Add_7_1":// 
				break;
			case "Soul_Add_7_2":// 
				DoPassive_7_2();
				break;
			case "Soul_Add_7_3":// 
				break;
		}
	}

	#region 영혼강화

	private void TryApplyEffect(IPassiveEffect effect)
	{
		if (effect == null) return;
		effect.ApplyEffect();
	}

	//정정당당
	public void DoPassive_1_2()
	{
		TryApplyEffect(new IncreaseDamageEffect(GameManager.Instance.playerData, 0.1f));
		//급소공략 불가능
	}

	//문전박대
	public void DoPassive_2_1()
	{

	}

	//백발백중
	public void DoPassive_2_2()
	{

	}

	//쾌도난마
	public void DoPassive_2_3()
	{

	}

	//금의환향
	public void DoPassive_3_1()
	{
		GameManager.Instance.playerData.gold *= 1.1f;
	}

	//다다익선
	public void DoPassive_3_2()
	{

	}

	//금강불괴
	public void DoPassive_4_1()
	{
		TryApplyEffect(new DecreaseDamageTakenEffect(GameManager.Instance.playerData, 0.5f));
	}

	//외강내유
	public void DoPassive_4_2()
	{
		TryApplyEffect(new IncreaseMaxHPEffect(GameManager.Instance.playerData, 75));
		TryApplyEffect(new DecreaseMaxSPEffect(GameManager.Instance.playerData, 25));
	}

	//외유내강
	public void DoPassive_4_3()
	{
		TryApplyEffect(new IncreaseMaxSPEffect(GameManager.Instance.playerData, 75));
		TryApplyEffect(new DecreaseMaxHPEffect(GameManager.Instance.playerData, 25));
	}

	//가담항설
	public void DoPassive_5_1()
	{

	}

	//취사선택
	public void DoPassive_5_2()
	{
		var player_item_use = FindObjectOfType<Player_Item_Use>();
		if(player_item_use)
        {
			int emptyItemSlotCount = player_item_use.CheckEmptySlotsCount();
			TryApplyEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.1f * emptyItemSlotCount));
		}
		
	}

	//등용문
	public void DoPassive_6_1()
	{

	}

	//승승장구
	public void DoPassive_6_2()
	{

	}
	//선견지명
	public void DoPassive_6_3()
	{

	}

	//구사일생
	public void DoPassive_7_1()
	{

	}

	//궁여지책
	public void DoPassive_7_2()
	{
		QuickSlotUI quickslotUI = FindObjectOfType<QuickSlotUI>();
		if (quickslotUI && quickslotUI.angleUnit >= 18)
		{
			TryApplyEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.5f));
		}
	}

	#endregion
}
