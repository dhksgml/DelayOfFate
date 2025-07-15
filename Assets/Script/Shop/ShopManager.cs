using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Shop shop;
    [SerializeField] ShopQuickSlot shopQuickSlot;
    PlayerData playerData;
    
    void Start()
    {
        if(GameManager.Instance)
        {
            playerData = GameManager.Instance.playerData;

            //shop.day = GameManager.Instance.Day;

            shop.Gold = playerData.gold;
            shop.Soul = playerData.soul;

            shopQuickSlot.quickSlots = playerData.quickSlots;
        }
    }
}
