using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
	[SerializeField] private List<Transform> menuItems; // 각 버튼의 Transform
	[SerializeField] private int currentIndex = 0;

	[SerializeField] private Transform selector; // 선택 표시용 오브젝트

	private void Start()
	{
		UpdateSelectorPosition();
	}

	private void Update()
	{
		// 위로 이동
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			currentIndex--;
			if (currentIndex < 0) currentIndex = menuItems.Count - 1;
			UpdateSelectorPosition();
		}

		// 아래로 이동
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			currentIndex++;
			if (currentIndex >= menuItems.Count) currentIndex = 0;
			UpdateSelectorPosition();
		}

		// 선택 실행
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C))
		{
			ExecuteOption(currentIndex);
		}
	}

	private void UpdateSelectorPosition()
	{
		if (selector != null && menuItems.Count > 0)
		{
			// 선택된 메뉴의 위쪽으로 이동
			Vector3 targetPos = menuItems[currentIndex].position;
			selector.position = targetPos;
		}
	}

	private void ExecuteOption(int index)
	{
		switch (index)
		{
			case 0: // Start
				GameManager.Instance.LoadScene("Stage_Scene");
				break;
			/*case 1: // Tutorial
				GameManager.Instance.isTutorial = true;
				GameManager.Instance.LoadScene("Tutorial_ShopScenes");
				break;*/
			case 1: // Exit
				GameManager.Instance.QuitGame();
				break;
		}
	}
}
