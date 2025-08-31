using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DialogSystem))]
public class TutorialDialog : TutorialBase
{
    private DialogSystem dialogSystem;
    private PlayerController playerController;
    private Player_Item_Use playerItemUse;

    public override void Enter()
    {
        if(SceneManager.GetActiveScene().name == "InGame_Scenes" || SceneManager.GetActiveScene().name == "New_Tutorial_Scenes")
        {
            playerController = FindObjectOfType<PlayerController>();
            playerItemUse = FindObjectOfType<Player_Item_Use>();
            playerController.isMoveAble = false;
            playerItemUse.isUseAble = false;
        }

        dialogSystem = GetComponent<DialogSystem>();
        dialogSystem.Setup();
    }

    public override void Execute(TutorialController controller)
    {
        bool isCompleted = dialogSystem.UpdateDialog();
        if(isCompleted == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        if (playerController != null)
        {
            playerController.isMoveAble = true;
            playerItemUse.isUseAble = true;
        }
    }
}
