using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Condition/DropItemCondition")]
public class DropItemCondition : TutorialCondition
{
    private bool dropped = false;

    public override void Initialize()
    {
        dropped = false;
        TutorialEvents.OnItemDropped += OnItemDropped;
    }

    private void OnDisable() => TutorialEvents.OnItemDropped -= OnItemDropped;

    private void OnItemDropped(Item item) => dropped = true;

    public override bool IsSatisfied() => dropped;
}
