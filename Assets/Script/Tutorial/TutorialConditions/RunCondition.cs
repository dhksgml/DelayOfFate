using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Condition/RunCondition")]
public class RunCondition : TutorialCondition
{
    public override bool IsSatisfied()
    {
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                      Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        return moving && Input.GetKey(KeyCode.LeftShift);
    }
}
