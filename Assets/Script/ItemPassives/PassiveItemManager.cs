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
	private int passive_6_1_count = 0;

	void Awake()
	{
		PlayerPrefs.DeleteAll();

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
				AddPassiveItem(g, n);
			}
		}
		// 3��° ȿ�� Soul_Add_2_3, Soul_Add_4_3, Soul_Add_6_3
		for (int n = 2; n <= 6; n += 2)
		{
			AddPassiveItem(n, 3);
		}
		// Soul_Add_8_1 ~ Soul_Add_8_3
		for (int n = 1; n <= 3; n++)
		{
			AddPassiveItem(8, n);
		}
		// Soul_Add_9_1 ~ Soul_Add_9_2
		for (int n = 1; n <= 3; n++)
		{
			AddPassiveItem(9, n);
		}
	}

	private void OnEnable()
    {
		GameEvents.OnNextDay += HandleNextDay;
		GameEvents.OnSaleItemImmediately += HandleSaleItemImmediately;
	}

    private void OnDisable()
    {
		GameEvents.OnNextDay-= HandleNextDay;
		GameEvents.OnSaleItemImmediately -= HandleSaleItemImmediately;
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
	private void AddPassiveItem(int g, int n)
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

	string GetPassiveName(int group, int number)
	{
		switch ($"{group}_{number}")
		{
			case "1_1": return "õ�����";
			case "1_2": return "�������";
			//case "2_1": return "�����ڴ�";
			//case "2_2": return "��߹���";
			//case "2_3": return "�赵����";
			case "3_1": return "����ȯ��";
			case "3_2": return "�ٴ��ͼ�";
			case "4_1": return "�ݰ��ұ�";
			case "4_2": return "�ܰ�����";
			case "4_3": return "��������";
			case "5_1": return "�����׼�";
			case "5_2": return "��缱��";
			//case "6_1": return "��빮";
			case "6_2": return "�½��屸";
			case "6_3": return "��������";
			case "7_1": return "�����ϻ�";
			case "7_2": return "�ÿ���å";
				//�нú� �ƴ�
			case "8_1": return "ȯ��";
			case "8_2": return "�����";
			case "8_3": return "���� 20��";
			case "9_1": return "ȣ��";
			case "9_2": return "ȣ��";
			// ...
			default: return "�� �� ����";
		}
	}
	string GetPassiveDescription(int group, int number)
	{
		//16���� ���� �� �ٲ��� ��
		switch ($"{group}_{number}")
		{
			case "1_1": return "������ ���Է� ���� �̵��ӵ�\n������ ���ŵ�";
			case "1_2": return "�ǱͿ��� �ִ� ���ذ� 10�� ����\n�Ǳ��� ������ ���� �� �� ����";
			//case "2_1": return "������� ���� ������ 10�� ����\n����̰� ��� ���� ������� ���ط��� 5�� ����";
			//case "2_2": return "������ �߰� ������ 20�� ����\n������ ���ط��� 5�� ����";
			//case "2_3": return "ȯ���� ���ݼӵ� 5�� ����\nȯ���� ���ذ� 5�� ����";
			case "3_1": return "�ప ���� �� ������ ����\n1�� ��ŭ ȹ��";
			case "3_2": return "���⸦ 0ȥ ���� ���� ����";
			case "4_1": return "�Ǳͷ� �޴� ü�����ذ�\n5�� ����";
			case "4_2": return "ü���� 75 ����\n������ 25 ����";
			case "4_3": return "������ 75 ����\nü���� 25 ����";
			case "5_1": return "���� ����� ������\n��ġ�� �ľ���";
			case "5_2": return "��� �ִ� �� ��ŭ\n�̵��ӵ� 1�� ����";
			//case "6_1": return "��� �ǸŽ� �̵��ӵ� 1�� ����\n�Ϸ簡 ������ �ʱ�ȭ\n(�ִ� 3��)";
			case "6_2": return "�Ǳ͸� óġ��\n���������� ���ط� 1�� ����";
			case "6_3": return "�Ϸ簡 ������ ��� �ִ� ������\n��ġ�� 5�� ����";
			case "7_1": return "��Ȱ ��\n������ ����߸��� �ʰ� ��Ȱ";
			case "7_2": return "18�� �̻��\n�޸��� �ӵ��� 5�� ����";
				//�нú� �ƴ�
			case "8_1": return "������ ������ 10~14�� ���ظ� ����\n������ ��� ����Ŵ";
			case "8_2": return "������ ������ 20~30�� ���ظ� ����\n������ ��� ����Ŵ";
			case "8_3": return "[�Ҹ���]\n���濡 ������ ���� 10~12�� ���ظ� ����\n������ ���\n�ִ�ü�� 5���� ���ظ� ����";
			case "9_1": return "���� �� ������";
			case "9_2": return "�޷��� ���� ������ ����";
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
			//case "2_1": return 1;
			//case "2_2": return 1;
			//case "2_3": return 1;
			case "3_1": return 3;
			case "3_2": return 1;
			case "4_1": return 4;
			case "4_2": return 1;
			case "4_3": return 1;
			case "5_1": return 1;
			case "5_2": return 2;
			//case "6_1": return 1;
			case "6_2": return 2;
			case "6_3": return 3;
			case "7_1": return 2;
			case "7_2": return 1;
				//�нú� �ƴ�
			case "8_1": return 5;
			case "8_2": return 5;
			case "8_3": return 5;
			case "9_1": return 6;
			case "9_2": return 6;
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
				DoPassive_5_1();
				break;
			case "Soul_Add_5_2":// 
				DoPassive_5_2();
				break;
			case "Soul_Add_5_3":// 
				break;
			case "Soul_Add_6_1":// 
				break;
			case "Soul_Add_6_2":// 
				DoPassive_6_2();
				break;
			case "Soul_Add_6_3":// 
				//DoPassive_6_3();
				break;
			case "Soul_Add_7_1":// 
				DoPassive_7_1();
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

	private void TryRemoveEffect(IPassiveEffect effect)
	{
		if (effect == null) return;
		effect.RemoveEffect();
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
		GameManager.Instance.Soul *= 1.1f;
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
		TryApplyEffect(new ItemFindAbilityOn(GameManager.Instance.playerData));
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
		if (passive_6_1_count < 3)
		{
			passive_6_1_count += 1;
			TryApplyEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.1f * passive_6_1_count));
		}
	}

	//�½��屸
	public void DoPassive_6_2()
	{
		TryApplyEffect(new IncreaseDamageEffect(GameManager.Instance.playerData, 0.1f * GameManager.Instance.killcount));
	}
	//��������
	public void DoPassive_6_3()
	{
		var quickSlotItems = GameManager.Instance.currentQuickSlot;
		if (quickSlotItems != null)
		{
			int totalGold = 0;
			foreach (var item in quickSlotItems)
			{
				if(item != null)
					totalGold += item.Coin;
			}
			GameManager.Instance.Add_Gold(totalGold * 0.5f);
		}
	}

	//�����ϻ�
	public void DoPassive_7_1()
	{
		TryApplyEffect(new ItemSaveAbilityOnRevive(GameManager.Instance.playerData));
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

	public void HandleNextDay()
    {
		//����ȯ��
		if(HasEffect("Soul_Add_3_1"))
        {
			DoPassive_3_1();
        }

		//��빮
		if (HasEffect("Soul_Add_6_1"))
		{
			TryRemoveEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.1f * passive_6_1_count));
			passive_6_1_count = 0;
		}

		//��������
		if (HasEffect("Soul_Add_6_3"))
        {
			DoPassive_6_3();
		}
    }

	public void HandleSaleItemImmediately()
    {
		//��빮
		if (HasEffect("Soul_Add_6_1"))
		{
			DoPassive_6_1();
		}
	}

	#endregion
}
