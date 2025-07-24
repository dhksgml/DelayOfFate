using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopQuickSlot : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4개의 퀵슬롯
    public ItemData[] SlotsData; // 슬롯이 가지고 있는 아이템 데이터
    public int selectedSlotIndex = 0; // 현재 선택된 슬롯

    public Image[] slotImages;          // 각 슬롯의 아이템 아이콘
    public Image[] slotBackgrounds;     // 각 슬롯의 배경 이미지 (활성화 표시)
    public TMP_Text[] slotCounts;       // 각 슬롯의 아이템 갯수 텍스트
    public Sprite default_Item_Sprite;  // 기본 아이템 아이콘
    public Sprite defaultSlotSprite;    // 기본 슬롯 배경
    public Sprite selectedSlotSprite;   // 선택된 슬롯 배경
    public TMP_Text Item_Name;          // 선택한 아이템의 이름
    public TMP_Text Item_Coin;          // 선택한 아이템의 가치
    public TMP_Text Item_Weight;        // 선택한 아이템의 무게
    public TMP_Text timeText;           // UI 텍스트 오브젝트

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

        //UpdateUI(quickSlots, selectedSlotIndex);
    }
    public void Update_UI()
    {
        timeText.text = GameManager.Instance.Day + " 일";

        for (int i = 0; i < 4; i++)
        {
            ItemData item = SlotsData[i];
            if (item != null && !string.IsNullOrEmpty(item.itemName))
            {
                // 아이템 아이콘 설정
                slotImages[i].sprite = item.icon;

                // 아이템 곗수 표시 여부
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
                // 빈 슬롯 처리
                slotImages[i].sprite = default_Item_Sprite;
                slotCounts[i].gameObject.SetActive(false);
            }

            // 선택된 슬롯 배경 표시
            slotBackgrounds[i].sprite = (i == selectedSlotIndex)
                ? selectedSlotSprite
                : defaultSlotSprite;
        }
        // 선택된 슬롯의 아이템 정보만 UI에 표시
        ItemData selectedItem = SlotsData[selectedSlotIndex];
        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.itemName))
        {
            int total_coin = selectedItem.Coin * selectedItem.Count;
            int total_Weight = selectedItem.Weight * selectedItem.Count;
            Item_Name.text = string.Format("[{0}]", selectedItem.itemName);
            Item_Coin.text = total_coin.ToString() + " 냥";
            Item_Weight.text = total_Weight.ToString() + " 근";
        }
        else
        {
            Item_Name.text = null;
            Item_Coin.text = null;
            Item_Weight.text = null;
        }
    }
}
