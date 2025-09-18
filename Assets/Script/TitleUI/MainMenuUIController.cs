using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
	public List<Button> menuButtons; // 순서대로 Start, Tutorial, Exit 버튼 넣기
	private int currentIndex = 0;

	void Start()
	{
		// 초기 선택
		if (menuButtons.Count > 0)
		{
			EventSystem.current.SetSelectedGameObject(menuButtons[0].gameObject);
		}
	}

	void Update()
	{
		HandleNavigation();
		HandleSubmit();
	}

	private void HandleNavigation()
	{
		// 위쪽 입력
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			currentIndex--;
			if (currentIndex < 0) currentIndex = menuButtons.Count - 1;
			EventSystem.current.SetSelectedGameObject(menuButtons[currentIndex].gameObject);
		}
		// 아래쪽 입력
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			currentIndex++;
			if (currentIndex >= menuButtons.Count) currentIndex = 0;
			EventSystem.current.SetSelectedGameObject(menuButtons[currentIndex].gameObject);
		}
	}

	private void HandleSubmit()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			menuButtons[currentIndex].onClick.Invoke();
		}
	}

	// 기존 버튼 처리 메서드들
	public void OnClickStartButton(string loadSceneName)
	{
		GameManager.Instance.LoadScene(loadSceneName);
	}

	public void OnClickTutorialButton()
	{
		GameManager.Instance.isTutorial = true;
		GameManager.Instance.LoadScene("Tutorial_ShopScenes");
	}

	public void OnClickExitButton()
	{
		GameManager.Instance.QuitGame();
	}
}
