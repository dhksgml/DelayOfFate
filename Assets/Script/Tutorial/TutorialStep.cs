using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialStepType
{
    ShowDialog,
    SpawnGameObject,
    MoveToPosition,
    WaitForInput,
    SpawnItemObject
}

[CreateAssetMenu(fileName = "TutorialStep", menuName ="Tutorial/Step/TutorialStep")]
public class TutorialStep : ScriptableObject
{
    public string stepName;
    [TextArea] public string[] messages;
    //public bool waitForInput = true;
    public TutorialCondition condition;

    public virtual void OnStepEnter() { }
    public virtual void OnStepEnd() { }

    public TutorialStepType stepType;
    public GameObject spawnPrefab;
    public Vector3 moveToPosition;
    public float waitTime = 2;
}
