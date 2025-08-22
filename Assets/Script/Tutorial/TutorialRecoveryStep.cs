using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialRecoveryStep", menuName = "Tutorial/Step/TutorialRecoveryStep")]
public class TutorialRecoveryStep : TutorialStep
{
    PlayerController playerController;

    public override void OnStepEnter()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DamagedHP(10);
        }
    }
}
