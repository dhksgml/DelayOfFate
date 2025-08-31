using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialItemSpawn : TutorialBase
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemData spawnItemData;
    [SerializeField] private Player_Item_Use playerItemUse;
    private bool isCompleted;

    private void OnEnable()
    {
        GameEvents.OnPickupItem += HandlePickupitem;
    }
    private void OnDisable()
    {
        GameEvents.OnPickupItem -= HandlePickupitem;
    }

    public override void Enter()
    {
        SpawnTutorialItem();
    }

    public override void Execute(TutorialController controller)
    {
       if(isCompleted == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }

    public void SpawnTutorialItem()
    {
        ItemObject itemObject = FindObjectOfType<ItemObject>();
        if (itemObject != null)
        {
            Destroy(itemObject.gameObject);
        }

        GameObject playerController = FindObjectOfType<PlayerController>().gameObject;
        GameObject itemObj = Instantiate(itemPrefab, playerController.transform.position + Vector3.down, Quaternion.identity);
        ItemObject itemObjComp = itemObj.GetComponentInChildren<ItemObject>();
        if (itemObjComp != null)
        {
            itemObjComp.itemDataTemplate = spawnItemData;
            itemObjComp.itemData = new Item(spawnItemData);
        }
    }
    
    public void HandlePickupitem()
    {
        isCompleted = true;
    }
}
