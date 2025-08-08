using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSwap : MonoBehaviour
{
    [SerializeField] GameObject itemSlot;
    [SerializeField] GameObject passiveSlot;
    [SerializeField] bool isItemSlot = false;

    public void SwapButton()
    {
        if(!isItemSlot)
        {
            SwapItemSlot();
        }
        else if (isItemSlot)
        {
            SwapPassiveSlot();
        }
    }

    void SwapItemSlot()
    {
        Debug.Log("아이템on");
        isItemSlot = true;
        itemSlot.SetActive(true);
        passiveSlot.SetActive(false);
    }
    
    void SwapPassiveSlot()
    {
        Debug.Log("페시브on");
        isItemSlot = false;
        itemSlot.SetActive(false);
        passiveSlot.SetActive(true);
    }
}
