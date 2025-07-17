using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFindAbilityOn : IPassiveEffect
{
    PlayerData playerData;

    public ItemFindAbilityOn(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public void ApplyEffect()
    {
        playerData.isFindNearestItem = true;
    }

    public void RemoveEffect()
    {
        playerData.isFindNearestItem = false;
    }
}
