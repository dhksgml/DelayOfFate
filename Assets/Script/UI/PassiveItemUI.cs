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
			default:
				EMDRMQ = " ";
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
		// 화면 크기 (UI Canvas가 Screen Space Overlay 기준)
		float screenWidth = 1920f;
		float screenHeight = 1080f;

		// 설명창 크기
		float popupWidth = 630f;
		float popupHeight = 450f;

		// 버튼 기준 왼쪽에 붙이려는 x 위치 시도
		float leftX = worldPos.x - 380f;
		float rightX = worldPos.x + 380f;

		// 최소/최대 위치 계산 (왼쪽 위가 기준일 경우)
		float minX = popupWidth / 2f;
		float maxX = screenWidth - popupWidth / 2f;
		float minY = popupHeight / 2f;
		float maxY = screenHeight - popupHeight / 2f;

		// x 위치를 왼쪽 우선으로 잡되, 왼쪽 공간 부족 시 오른쪽으로 바꿈
		if (leftX < minX)
		{
			worldPos.x = rightX;
		}
		else
		{
			worldPos.x = leftX;
		}

		// y 위치는 그대로 클램프
		worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);

		transform.position = worldPos;
	}
}
