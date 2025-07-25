using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialItemSpawnStep", menuName = "Tutorial/Step/TutorialItemSpawnStep")]
public class TutorialItemSpawnStep : TutorialStep
{
    [Header("Spawn Item")]
    public GameObject itemPrefab;
    public ItemData spawnItemData;

    public override void OnStepEnter()
    {
        SpawnTutorialItem();
    }

    public void SpawnTutorialItem()
    {
        GameObject playerController = FindObjectOfType<PlayerController>().gameObject;
        GameObject itemObj = Instantiate(itemPrefab, playerController.transform.position + Vector3.up, Quaternion.identity);
        ItemObject itemObjComp = itemObj.GetComponentInChildren<ItemObject>();
        if (itemObjComp != null)
        {
            itemObjComp.itemDataTemplate = spawnItemData;
            itemObjComp.itemData = new Item(spawnItemData);
        }
    }
}

