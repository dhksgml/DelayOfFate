using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM(Resources.Load<AudioClip>("BGM/bgm_Main_Menu"));
    }
    public void OnClickStartButton(string loadSceneName)
    {
        GameManager.Instance.LoadScene(loadSceneName);
    }
    public void OnClickExitButton()
    {
        GameManager.Instance.QuitGame();
    }
}
