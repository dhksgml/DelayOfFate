using UnityEngine;
using UnityEngine.EventSystems; // 포인터 이벤트를 사용하려면 필요

public class Soul_in_text : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public string itemId; // 예: "Soul_Add_2_3"
	private PassiveItemUI passiveItemUI;
	private PassiveItemManager passiveItemManager;
	public bool show = true;

	void Start()
	{
		passiveItemUI = FindObjectOfType<PassiveItemUI>();
		passiveItemManager = FindObjectOfType<PassiveItemManager>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		var item = passiveItemManager.passiveItems.Find(i => i.id == itemId);
		if (item != null)
		{
			if (show)
            {
				passiveItemUI.Show(item.itemName, item.description, item.rating);
				passiveItemUI.SetPosition(transform.position/*Input.mousePosition*/);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		passiveItemUI.Hide();
	}
}
