using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialGameObjectSpawnStep", menuName = "Tutorial/Step/TutorialGameObjectSpawnStep")]
public class TutorialGameObjectSpawnStep : TutorialStep
{
    GameObject spawnObject;

    public override void OnStepEnter()
    {
        SpawnGameObject();
    }

    public override void OnStepEnd()
    {
        RemoveGameObject();
    }

    public void SpawnGameObject()
    {
        GameObject playerController = FindObjectOfType<PlayerController>().gameObject;
        spawnObject = Instantiate(spawnPrefab, playerController.transform.position + Vector3.up, Quaternion.identity);
    }

    public void RemoveGameObject()
    {
        Destroy(spawnObject);
    }
}
