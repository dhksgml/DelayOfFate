using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Condition/SellItemCondition")]
public class SellItemCondition : TutorialCondition
{
    private bool selled = false;

    public override void Initialize()
    {
        selled = false;
        TutorialEvents.OnItemSelled += OnItemSelled;
    }

    private void OnDisable() => TutorialEvents.OnItemSelled -= OnItemSelled;

    private void OnItemSelled(Item item) => selled = true;

    public override bool IsSatisfied() => selled;
}