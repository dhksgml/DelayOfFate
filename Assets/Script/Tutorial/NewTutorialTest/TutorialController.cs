using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<TutorialBase> tutorials;
    [SerializeField] private string nextSceneName = "";
    [SerializeField] private bool isBattleTutorial;
    private TutorialBase currentTutorial = null;
    private int currentIndex = -1;

    private void Start()
    {
        SetNextTutorial();
    }

    private void Update()
    {
        if(currentTutorial != null)
        {
            currentTutorial.Execute(this);
        }
    }

    public void SetNextTutorial()
    {
        if(currentTutorial != null)
        {
            currentTutorial.Exit();
        }    

        if( currentIndex >= tutorials.Count-1)
        {
            CompleteAllTutorials();
            return;
        }

        currentIndex++;
        currentTutorial = tutorials[currentIndex];

        currentTutorial.Enter();
    }

    private void CompleteAllTutorials()
    {
        currentTutorial = null;
        if(!nextSceneName.Equals(""))
        {
            if (isBattleTutorial)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.AlldataReset();
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}