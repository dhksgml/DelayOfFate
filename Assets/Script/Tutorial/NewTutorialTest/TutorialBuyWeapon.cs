using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBuyWeapon : TutorialBase
{
    [SerializeField] private List<Button> buttons;
    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject highlightWeaponImage;

    private bool isCompleted;

    private void OnEnable()
    {
        GameEvents.OnBuyWeapon += HadleBuyWeapon;
    }

    private void OnDisable()
    {
        GameEvents.OnBuyWeapon -= HadleBuyWeapon;
    }

    private void HadleBuyWeapon()
    {
        isCompleted = true;
    }

    public override void Enter()
    {
        foreach(Button button in buttons)
        {
            button.interactable = false;
        }

        dialogCanvas.SetActive(false);
        highlightWeaponImage.SetActive(true);
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

        dialogCanvas.SetActive(true);
        highlightWeaponImage.SetActive(false);
    }
}
