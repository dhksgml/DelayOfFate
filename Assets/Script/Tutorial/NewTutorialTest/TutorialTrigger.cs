using UnityEngine;

public class TutorialTrigger : TutorialBase
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform triggerObject;

    public bool isTrigger { set; get; } = false;

    public override void Enter()
    {
        playerController.isMoveAble = true;
        triggerObject.gameObject.SetActive(true);
    }

    public override void Execute(TutorialController controller)
    {
        transform.position = playerController.transform.position;

        if(isTrigger == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        playerController.isMoveAble = false;
        triggerObject.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.Equals(triggerObject))
        {
            isTrigger = true;
            collision.gameObject.SetActive(false);
        }
    }
}
