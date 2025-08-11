using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSwap : MonoBehaviour
{
    [SerializeField] GameObject itemSlot_1;
    [SerializeField] GameObject itemSlot_2;
    [SerializeField] GameObject itemSlot_3;
    [SerializeField] GameObject passiveSlot;
    [SerializeField] bool isItemSlot = false;

    private void Start()
    {
        if (!isItemSlot)
        {
            SwapItemSlot();
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) SwapButton();
    }

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
        itemSlot_1.SetActive(true);
        itemSlot_2.SetActive(true);
        itemSlot_3.SetActive(true);
        passiveSlot.SetActive(false);
    }
    
    void SwapPassiveSlot()
    {
        Debug.Log("페시브on");
        isItemSlot = false;
        itemSlot_1.SetActive(false);
        itemSlot_2.SetActive(false);
        itemSlot_3.SetActive(false);
        passiveSlot.SetActive(true);
    }
}
