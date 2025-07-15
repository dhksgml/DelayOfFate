using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopQuickSlot : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4개의 퀵슬롯
    public int selectedSlotIndex = 0; // 현재 선택된 슬롯

    private void Update()
    {
        HandleSlotSelection();
    }

    void HandleSlotSelection()
    {
        // 슬롯 선택 (1~4 키)
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlotIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlotIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlotIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlotIndex = 3;

        // 마우스 휠로 슬롯 변경
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) selectedSlotIndex = (selectedSlotIndex + 1) % 4;  // 0~3 범위로 돌아가도록
        if (scroll < 0f) selectedSlotIndex = (selectedSlotIndex + 3) % 4;  // 역방향 스크롤 (0~3 범위로 돌아가도록)

        // selectedSlotIndex가 0~3 범위 내로 유지되도록 보장
        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, 3);

        UpdateQuickSlotUI();
    }

    public void UpdateQuickSlotUI()
    {
        QuickSlotUI quickSlotUI = FindObjectOfType<QuickSlotUI>();
        if (quickSlotUI != null)
        {
            quickSlotUI.UpdateUI(quickSlots, selectedSlotIndex);
        }
    }
}
