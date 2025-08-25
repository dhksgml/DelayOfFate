using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHighlightButton : TutorialBase
{
    [SerializeField] private List<Button> buttons;
    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject highlightButton;
    private bool isCompleted;

    public void HadleClickButton()
    {
        isCompleted = true;
    }

    public override void Enter()
    {
        foreach(Button button in buttons)
        {
            button.interactable = false;
        }
        highlightButton.SetActive(true);
        dialogCanvas.SetActive(false);
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
        highlightButton.SetActive(false);
        dialogCanvas.SetActive(true);
    }
}
