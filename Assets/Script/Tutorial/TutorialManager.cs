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
    public static Action<Item> OnItemSelled;
}

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialBackground;
    public TextMeshProUGUI tutorialText;
    public float messageWaitTime;
    public List<TutorialStep> steps;

    public GameObject lowlightUI;
    public GameObject[] highlightUIs;
    private int highlightUIObjectIndex;

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
        GameManager.Instance.playerData.isFindNearestItem = true;
        tutorialBackground.SetActive(true);

        while (currentIndex < steps.Count)
        {
            yield return StartCoroutine(HandleStep(steps[currentIndex]));
            currentIndex = Mathf.Clamp(currentIndex + 1, 0, steps.Count);
        }

        Debug.Log("Tutorial Á¾·á");
        tutorialText.gameObject.SetActive(false);
        tutorialBackground.SetActive(false);
        GameManager.Instance.isTutorial = false;
        GameManager.Instance.AlldataReset();
        GameManager.Instance.LoadScene("Stage_Scene");
    }

    private IEnumerator HandleStep(TutorialStep step)
    {
        for (int i = 0; i < step.messages.Length; i++)
        {
            tutorialText.text = step.messages[i];

            if (i < step.messages.Length - 1)
            {
                yield return new WaitForSeconds(step.waitTime);
            }
        }

        switch (step.stepType)
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
            case TutorialStepType.HightlightUI:
                LowlightingUI();
                HighlightingUI();
                if(step.waitForInput)
                    yield return new WaitUntil(() => step.condition.IsSatisfied());
                else
                    yield return new WaitForSeconds(step.waitTime);
                LowlightingOffUI();
                HighlightingOffUI();
                break;
            case TutorialStepType.SpawnItemObject:
            case TutorialStepType.WaitForInput:
                step.condition.Initialize();
                StartStep(step);
                yield return new WaitUntil(() => step.condition.IsSatisfied());
                EndStep(step);
                break;
        }
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

    public void LowlightingUI()
    {
        if (lowlightUI != null)
            lowlightUI.SetActive(true);
    }

    public void LowlightingOffUI()
    {
        if (lowlightUI != null)
            lowlightUI.SetActive(false);
    }

    public void HighlightingUI()
    {
        if (highlightUIs[highlightUIObjectIndex] != null)
            highlightUIs[highlightUIObjectIndex].SetActive(true);
    }

    public void HighlightingOffUI()
    {
        if (highlightUIs[highlightUIObjectIndex] != null)
            highlightUIs[highlightUIObjectIndex].SetActive(false);
        Mathf.Clamp(highlightUIObjectIndex += 1, 0, highlightUIs.Length);
    }

    public void SkipTutorial()
    {
        StopCoroutine(PlayTutorial());
        StartCoroutine(SkipTutorialRoutine());
    }
    private IEnumerator SkipTutorialRoutine()
    {
        //HighlightingOffUI();
        currentIndex = steps.Count - 1;

        yield return StartCoroutine(HandleStep(steps[currentIndex]));

        tutorialText.gameObject.SetActive(false);
        tutorialBackground.SetActive(false);
        GameManager.Instance.isTutorial = false;
        GameManager.Instance.AlldataReset();
        GameManager.Instance.LoadScene("Stage_Scene");
    }
}
