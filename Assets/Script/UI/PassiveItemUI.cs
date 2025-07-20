using UnityEngine;
using TMPro;

public class PassiveItemUI : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI descText;
	public TextMeshProUGUI emdrmqText;
	public void Show(string itemName, string description, int emdrmq)
	{
		string EMDRMQ = "";
		switch (emdrmq)
        {
			case 1:
				EMDRMQ = "�ϱ�";
				break;
			case 2:
				EMDRMQ = "�߱�";
				break;
			case 3:
				EMDRMQ = "���";
				break;
			case 4:
				EMDRMQ = "Ư��";
				break;
			case 5:
				EMDRMQ = "����";
				break;
			case 6:
				EMDRMQ = "��ȭ";
				break;
		}

		nameText.text = itemName;
		descText.text = description;
		emdrmqText.text = EMDRMQ;
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SetPosition(Vector3 worldPos)
	{
		transform.position = worldPos;
	}
}
