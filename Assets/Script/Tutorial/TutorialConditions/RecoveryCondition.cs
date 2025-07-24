using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Condition/RecoveryCondition")]
public class RecoveryCondition : TutorialCondition
{
    public override bool IsSatisfied()
    {
        return Input.GetKeyDown(KeyCode.C);
    }
}
