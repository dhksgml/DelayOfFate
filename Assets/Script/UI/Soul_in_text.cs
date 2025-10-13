using UnityEngine;
using UnityEngine.EventSystems; // 포인터 이벤트를 사용하려면 필요

public class Soul_in_text : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public string itemId; // 예: "Soul_Add_2_3"
	private PassiveItemUI passiveItemUI;
	public bool show = true;

	void Start()
	{
		passiveItemUI = FindObjectOfType<PassiveItemUI>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		var item = PassiveItemManager.Instance.passiveItems.Find(i => i.id == itemId);
		if (item != null)
		{
			if (show)
            {
				if (passiveItemUI != null)
				{
					passiveItemUI.Show(item.itemName, item.description, item.rating);
					//passiveItemUI.SetPosition(Input.mousePosition);
				}
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (passiveItemUI != null)
		{
			passiveItemUI.Hide();
		}
	}
}
