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
				EMDRMQ = "하급";
				break;
			case 2:
				EMDRMQ = "중급";
				break;
			case 3:
				EMDRMQ = "상급";
				break;
			case 4:
				EMDRMQ = "특급";
				break;
			case 5:
				EMDRMQ = "무기";
				break;
			case 6:
				EMDRMQ = "강화";
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
