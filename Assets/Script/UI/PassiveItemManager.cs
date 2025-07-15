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
		// ���� ���� �ڽŰ� ���� Ÿ���� ������Ʈ�� 2�� �̻� �ִ� ��� ��� ����
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

		// ������ 2���� �� 7����
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
		// ����� �� �ҷ�����
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
			case "1_1": return "õ�����";
			case "1_2": return "�������";
			case "2_1": return "�����ڴ�";
			case "2_2": return "��߹���";
			case "2_3": return "�赵����";
			case "3_1": return "����ȯ��";
			case "3_2": return "�ٴ��ͼ�";
			case "4_1": return "�ݰ��ұ�";
			case "4_2": return "�ܰ�����";
			case "4_3": return "��������";
			case "5_1": return "�����׼�";
			case "5_2": return "��缱��";
			case "6_1": return "��빮";
			case "6_2": return "�½��屸";
			case "6_3": return "��������";
			case "7_1": return "�����ϻ�";
			case "7_2": return "�ÿ���å";
			// ...
			default: return "�� �� ����";
		}
	}
	string GetPassiveDescription(int group, int number)
	{
		switch ($"{group}_{number}")
		{
			case "1_1": return "������ ���Է� ���� �̵��ӵ� ������ ���ŵ�";
			case "1_2": return "�ǱͿ��� �ִ� ���ذ� 10�� ���� �Ǳ��� ������ ���� �� �� ����";
			case "2_1": return "������� ���� ������ 10�� ���� ����̰� ��� ���� ������� ���ط��� 5�� ����";
			case "2_2": return "������ �߰� ������ 20�� ���� ������ ���ط��� 5�� ����";
			case "2_3": return "ȯ���� ���ݼӵ� 5�� ���� ȯ���� ���ذ� 5�� ����";
			case "3_1": return "�ప ���� �� ������ ���� 1�� ��ŭ ȹ��";
			case "3_2": return "���⸦ 0ȥ ���� ���� ����";
			case "4_1": return "�Ǳͷ� �޴� ü�����ذ� 5�� ����";
			case "4_2": return "ü���� 75 ���� ������ 25 ����";
			case "4_3": return "������ 75 ���� ü���� 25 ����";
			case "5_1": return "���� ����� ������ ��ġ�� �ľ���";
			case "5_2": return "��� �ִ� �� ��ŭ �̵��ӵ� 1�� ����";
			case "6_1": return "��� �ǸŽ� �̵��ӵ� 1�� ���� (�ִ� 3��) �Ϸ簡 ������ �ʱ�ȭ";
			case "6_2": return "�Ǳ͸� óġ�� ���������� ���ط� 1�� ����";
			case "6_3": return "�Ϸ簡 ������ ��� �ִ� ������ ��ġ�� 5�� ����";
			case "7_1": return "��Ȱ �� ������ ����߸��� �ʰ� ��Ȱ";
			case "7_2": return "18�� �̻�� �޸��� �ӵ��� 5�� ����";
			// ...
			default: return "������ �����ϴ�.";
		}
	}
	int GetPassiveEmdrmq(int group, int number) //�������� ���
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
			Debug.LogWarning($"GetIcon ����: group={group}, number={number}");
			return null;
		}

		return groupArray[number - 1];
	}

	public bool HasEffect(string id)//�� �ҷ�����
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

	#region ��ȥ��ȭ

	private void TryApplyEffect(IPassiveEffect effect)
	{
		if (effect == null) return;
		effect.ApplyEffect();
	}

	//�������
	public void DoPassive_1_2()
	{
		TryApplyEffect(new IncreaseDamageEffect(GameManager.Instance.playerData, 0.1f));
		//�޼Ұ��� �Ұ���
	}

	//�����ڴ�
	public void DoPassive_2_1()
	{

	}

	//��߹���
	public void DoPassive_2_2()
	{

	}

	//�赵����
	public void DoPassive_2_3()
	{

	}

	//����ȯ��
	public void DoPassive_3_1()
	{
		GameManager.Instance.playerData.gold *= 1.1f;
	}

	//�ٴ��ͼ�
	public void DoPassive_3_2()
	{

	}

	//�ݰ��ұ�
	public void DoPassive_4_1()
	{
		TryApplyEffect(new DecreaseDamageTakenEffect(GameManager.Instance.playerData, 0.5f));
	}

	//�ܰ�����
	public void DoPassive_4_2()
	{
		TryApplyEffect(new IncreaseMaxHPEffect(GameManager.Instance.playerData, 75));
		TryApplyEffect(new DecreaseMaxSPEffect(GameManager.Instance.playerData, 25));
	}

	//��������
	public void DoPassive_4_3()
	{
		TryApplyEffect(new IncreaseMaxSPEffect(GameManager.Instance.playerData, 75));
		TryApplyEffect(new DecreaseMaxHPEffect(GameManager.Instance.playerData, 25));
	}

	//�����׼�
	public void DoPassive_5_1()
	{

	}

	//��缱��
	public void DoPassive_5_2()
	{
		var player_item_use = FindObjectOfType<Player_Item_Use>();
		if(player_item_use)
        {
			int emptyItemSlotCount = player_item_use.CheckEmptySlotsCount();
			TryApplyEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.1f * emptyItemSlotCount));
		}
		
	}

	//��빮
	public void DoPassive_6_1()
	{

	}

	//�½��屸
	public void DoPassive_6_2()
	{

	}
	//��������
	public void DoPassive_6_3()
	{

	}

	//�����ϻ�
	public void DoPassive_7_1()
	{

	}

	//�ÿ���å
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
