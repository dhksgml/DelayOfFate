using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class TutorialEvents
{
    public static Action<Item> OnItemPickedUp;
    public static Action<Item> OnItemDropped;
    public static Action<Item> OnWeaponUsed;
}

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialBackground;
    public TextMeshProUGUI tutorialText;
    public List<TutorialStep> steps;

    private int currentIndex = 0;

    private void Start()
    {
        StartTutorial();
    }

    public void StartTutorial()
    {
        StartCoroutine(PlayTutorial());
    }

    private IEnumerator PlayTutorial()
    {
        tutorialBackground.SetActive(true);

        while (currentIndex < steps.Count)
        {
            yield return StartCoroutine(HandleStep(steps[currentIndex]));
            currentIndex++;
        }

        Debug.Log("Tutorial Á¾·á");
        tutorialText.gameObject.SetActive(false);
        tutorialBackground.SetActive(false);
    }

    private IEnumerator HandleStep(TutorialStep step)
    {
        for (int i = 0; i < step.messages.Length; i++)
        {
            tutorialText.text = step.messages[i];

            if (i < step.messages.Length - 1)
            {
                yield return new WaitForSeconds(2f);
            }
        }

        switch(step.stepType)
        {
            case TutorialStepType.ShowDialog:
                yield return new WaitForSeconds(step.waitTime);
                break;
            case TutorialStepType.MoveToPosition:
                break;
            case TutorialStepType.SpawnGameObject:
                StartStep(step);
                yield return new WaitForSeconds(step.waitTime);
                EndStep(step);
                break;
            case TutorialStepType.SpawnItemObject:
            case TutorialStepType.WaitForInput:
                step.condition.Initialize();
                StartStep(step);
                yield return new WaitUntil(() => step.condition.IsSatisfied());
                break;
        }
        //if (step.waitForInput && step.condition != null)
        //{
        //    step.condition.Initialize();
        //    StartStep(step);
        //    yield return new WaitUntil(() => step.condition.IsSatisfied());
        //}
        yield return new WaitForSeconds(1f);
    }

    public void StartStep(TutorialStep step)
    {
        step.OnStepEnter();
    }

    public void EndStep(TutorialStep step)
    {
        step.OnStepEnd();
    }
}
