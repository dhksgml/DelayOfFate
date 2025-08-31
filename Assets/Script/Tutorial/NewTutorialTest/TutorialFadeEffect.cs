using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialFadeEffect : TutorialBase
{
    [SerializeField] private FadeEffect fadeEffect;
    [SerializeField] bool isFadeIn = false;
    private bool isCompleted = false;
    private PlayerController playerController;
    private Player_Item_Use playerItemUse;

    public override void Enter()
    {
        if (SceneManager.GetActiveScene().name == "InGame_Scenes" || SceneManager.GetActiveScene().name == "New_Tutorial_Scenes")
        {
            playerController = FindObjectOfType<PlayerController>();
            playerItemUse = FindObjectOfType<Player_Item_Use>();
            playerController.isMoveAble = false;
            playerItemUse.isUseAble = false;
        }

        if (isFadeIn == true)
        {
            fadeEffect.FadeIn(OnAfterFadeEffect);
        }
        else
        {
            fadeEffect.FadeOut(OnAfterFadeEffect);
        }
    }

    public void OnAfterFadeEffect()
    {
        isCompleted = true;
    }

    public override void Execute(TutorialController controller)
    {
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
