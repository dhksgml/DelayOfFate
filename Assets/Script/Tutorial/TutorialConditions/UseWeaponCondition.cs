using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Condition/UseWeaponCondition")]
public class UseWeaponCondition : TutorialCondition
{
    private bool usedWeapon = false;

    public override void Initialize()
    {
        usedWeapon = false;
        TutorialEvents.OnWeaponUsed += OnWeaponUsed;
    }

    private void OnDisable() => TutorialEvents.OnWeaponUsed -= OnWeaponUsed;

    private void OnWeaponUsed(Item item) => usedWeapon = true;

    public override bool IsSatisfied() => usedWeapon;
}