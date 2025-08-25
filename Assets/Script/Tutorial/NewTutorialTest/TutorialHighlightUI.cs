using UnityEngine;

[RequireComponent(typeof(DialogSystem))]
public class TutorialHighlightUI : TutorialBase
{
    [SerializeField] private GameObject highlightUI;
    [SerializeField] private GameObject lowlightUI;

    private DialogSystem dialogSystem;

    public override void Enter()
    {
        dialogSystem = GetComponent<DialogSystem>();
        dialogSystem.Setup();

        lowlightUI.SetActive(true);
        highlightUI.SetActive(true);
    }

    public override void Execute(TutorialController controller)
    {
        bool isCompleted = dialogSystem.UpdateDialog();
        if (isCompleted == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        lowlightUI.SetActive(false);
        highlightUI.SetActive(false);
    }
}
