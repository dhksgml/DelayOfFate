using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSaveAbilityOnRevive : IPassiveEffect
{
    PlayerData playerData;

    public ItemSaveAbilityOnRevive(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public void ApplyEffect()
    {
        playerData.isDropWhenRevive = true;
    }

    public void RemoveEffect()
    {
        playerData.isDropWhenRevive = false;
    }
}
