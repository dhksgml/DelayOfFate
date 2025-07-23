using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Condition/PickupItemCondition")]
public class PickupItemCondition : TutorialCondition
{
    private bool picked = false;

    public override void Initialize()
    {
        picked = false;
        TutorialEvents.OnItemPickedUp += OnItemPicked;
    }

    private void OnDisable() => TutorialEvents.OnItemPickedUp -= OnItemPicked;

    private void OnItemPicked(Item item) => picked = true;

    public override bool IsSatisfied() => picked;
}