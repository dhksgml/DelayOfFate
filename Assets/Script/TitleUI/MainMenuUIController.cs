using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    public void OnClickStartButton(string loadSceneName)
    {
        GameManager.Instance.LoadScene(loadSceneName);
    }

    public void OnClickTutorialButton()
    {
        GameManager.Instance.isTutorial = true;
        GameManager.Instance.LoadScene("Tutorial_Scenes");
    }

    public void OnClickExitButton()
    {
        GameManager.Instance.QuitGame();
    }
}
