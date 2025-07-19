using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Passive_UI : MonoBehaviour
{
	public Image[] Passive_Item_Icon_1;
	public Image[] Passive_Item_Icon_2;
	public Image[] Passive_Item_Icon_3;
	public Image[] Passive_Item_Icon_4;
	public Image[] Passive_Item_Icon_5;
	public Image[] Passive_Item_Icon_6;
	public Image[] Passive_Item_Icon_7;

	private PassiveItemManager passiveItemManager;

	void Start()
	{
		passiveItemManager = FindObjectOfType<PassiveItemManager>();
		chrlghk();
		UpdateInventoryIcons();
	}
    private void Update()
    {
		UpdateInventoryIcons();
	}
    public void UpdateInventoryIcons()
	{
		// 계열별 인덱스 초기화
		int[] groupCounts = new int[8]; // 1~7번 계열 사용

		foreach (var item in passiveItemManager.passiveItems)
		{
			if (!item.isPurchased) continue;

			// id: Soul_Add_3_2
			string[] parts = item.id.Split('_');
			int group = int.Parse(parts[2]); // 1~7
			int number = int.Parse(parts[3]); // 1~3

			Sprite icon = passiveItemManager.GetIcon(group, number);
			if (icon == null)
			{
				Debug.LogWarning($"아이콘 없음: {item.id}");
				continue;
			}

			Image[] targetArray = GetArrayByGroup(group);

			int index = groupCounts[group]++; // 구매된 만큼 인덱스 증가

			Soul_in_text slotScript = targetArray[index].GetComponentInParent<Soul_in_text>();
			if (slotScript != null)
			{
				slotScript.itemId = item.id;
			}
			if (targetArray != null && index < targetArray.Length)
			{
				targetArray[index].sprite = icon;
				targetArray[index].color = Color.white; // 안보일 경우 대비
			}
		}
	}
	void chrlghk()
    {
		for (int g = 1; g <= 7; g++)
		{
			for (int n = 1; n <= 3; n++)
			{
				PlayerPrefs.SetInt($"Soul_Add_{g}_{n}", 0);
			}
		}
	}
	Image[] GetArrayByGroup(int group)
	{
		switch (group)
		{
			case 1: return Passive_Item_Icon_1;
			case 2: return Passive_Item_Icon_2;
			case 3: return Passive_Item_Icon_3;
			case 4: return Passive_Item_Icon_4;
			case 5: return Passive_Item_Icon_5;
			case 6: return Passive_Item_Icon_6;
			case 7: return Passive_Item_Icon_7;
			default: return null;
		}
	}
}
