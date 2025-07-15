using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopQuickSlot : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4���� ������
    public int selectedSlotIndex = 0; // ���� ���õ� ����

    private void Update()
    {
        HandleSlotSelection();
    }

    void HandleSlotSelection()
    {
        // ���� ���� (1~4 Ű)
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlotIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlotIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlotIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlotIndex = 3;

        // ���콺 �ٷ� ���� ����
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) selectedSlotIndex = (selectedSlotIndex + 1) % 4;  // 0~3 ������ ���ư�����
        if (scroll < 0f) selectedSlotIndex = (selectedSlotIndex + 3) % 4;  // ������ ��ũ�� (0~3 ������ ���ư�����)

        // selectedSlotIndex�� 0~3 ���� ���� �����ǵ��� ����
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
