using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverUI : MonoBehaviour
{

    public void TitleSceneButton()
    {
        if (GameManager.Instance)
            GameManager.Instance.AlldataReset();

        SceneManager.LoadScene("TitleScene");
    }
}
