using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TutorialStep", menuName ="Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    public string stepName;
    [TextArea] public string[] messages;
    public bool waitForInput = true;
    public TutorialCondition condition;

    public virtual void OnStepEnter() { }
}
