using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopQuickSlot : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4���� ������
    public ItemData[] SlotsData; // ������ ������ �ִ� ������ ������
    public int selectedSlotIndex = 0; // ���� ���õ� ����

    public Image[] slotImages;          // �� ������ ������ ������
    public Image[] slotBackgrounds;     // �� ������ ��� �̹��� (Ȱ��ȭ ǥ��)
    public TMP_Text[] slotCounts;       // �� ������ ������ ���� �ؽ�Ʈ
    public Sprite default_Item_Sprite;  // �⺻ ������ ������
    public Sprite defaultSlotSprite;    // �⺻ ���� ���
    public Sprite selectedSlotSprite;   // ���õ� ���� ���
    public TMP_Text Item_Name;          // ������ �������� �̸�
    public TMP_Text Item_Coin;          // ������ �������� ��ġ
    public TMP_Text Item_Weight;        // ������ �������� ����
    public TMP_Text timeText;           // UI �ؽ�Ʈ ������Ʈ

    private void Start()
    {
        Item_Name.text = null;
        Item_Coin.text = null;
        Item_Weight.text = null;
    }

    private void Update()
    {
        HandleSlotSelection();
        Update_UI();
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

        //UpdateUI(quickSlots, selectedSlotIndex);
    }
    public void Update_UI()
    {
        timeText.text = GameManager.Instance.Day + " ��";

        for (int i = 0; i < 4; i++)
        {
            ItemData item = SlotsData[i];
            if (item != null && !string.IsNullOrEmpty(item.itemName))
            {
                // ������ ������ ����
                slotImages[i].sprite = item.icon;

                // ������ ��� ǥ�� ����
                if (item.Count_Check)
                {
                    slotCounts[i].gameObject.SetActive(true);
                    slotCounts[i].text = item.Count.ToString();
                }
                else
                {
                    slotCounts[i].gameObject.SetActive(false);
                }
            }
            else
            {
                // �� ���� ó��
                slotImages[i].sprite = default_Item_Sprite;
                slotCounts[i].gameObject.SetActive(false);
            }

            // ���õ� ���� ��� ǥ��
            slotBackgrounds[i].sprite = (i == selectedSlotIndex)
                ? selectedSlotSprite
                : defaultSlotSprite;
        }
        // ���õ� ������ ������ ������ UI�� ǥ��
        ItemData selectedItem = SlotsData[selectedSlotIndex];
        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.itemName))
        {
            int total_coin = selectedItem.Coin * selectedItem.Count;
            int total_Weight = selectedItem.Weight * selectedItem.Count;
            Item_Name.text = string.Format("[{0}]", selectedItem.itemName);
            Item_Coin.text = total_coin.ToString() + " ��";
            Item_Weight.text = total_Weight.ToString() + " ��";
        }
        else
        {
            Item_Name.text = null;
            Item_Coin.text = null;
            Item_Weight.text = null;
        }
    }
}
