using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuickSlotUI : MonoBehaviour
{
    public Image[] slotImages;          // 각 슬롯의 아이템 아이콘
    public Image[] slotBackgrounds;     // 각 슬롯의 배경 이미지 (활성화 표시)
    public TMP_Text[] slotCounts;       // 각 슬롯의 아이템 갯수 텍스트
    public Sprite default_Item_Sprite;  // 기본 아이템 아이콘
    public Sprite defaultSlotSprite;    // 기본 슬롯 배경
    public Sprite selectedSlotSprite;   // 선택된 슬롯 배경
    public TMP_Text Item_Name;          // 선택한 아이템의 이름
    public TMP_Text Item_Coin;          // 선택한 아이템의 가치
    public TMP_Text Item_Weight;        // 선택한 아이템의 무게

    public Player_Item_Use playerItemUse;
    private PlayerController playerController;

    public TMP_Text timeText; // UI 텍스트 오브젝트
    public int angleUnit = 0;

    void Start()
    {
        Item_Name.text = null;
        Item_Coin.text = null;
        Item_Weight.text = null;
        playerItemUse = FindObjectOfType<Player_Item_Use>();
        playerController = FindObjectOfType<PlayerController>();
        UpdateUI();
    }
    void Update()
    {
        float time = Time.time; // 경과 시간 (초)
        angleUnit = Mathf.FloorToInt(time / 20f); // 20초마다 1각
    }

    public void DisplayItemInfo(int index, ItemData item)
    {
        print(item);
        if (index < 0 || index >= 4) return;

        if (item != null && !string.IsNullOrEmpty(item.itemName))
        {
            slotImages[index].sprite = item.icon;

            if (item.Count_Check)
            {
                slotCounts[index].gameObject.SetActive(true);
                slotCounts[index].text = item.Count.ToString();
            }
            else
            {
                slotCounts[index].gameObject.SetActive(false);
            }
        }
        else
        {
            // 빈 슬롯 처리
            slotImages[index].sprite = default_Item_Sprite;
            slotCounts[index].gameObject.SetActive(false);
        }
    }
    public void UpdateUI()
    {
        if (SceneManager.GetActiveScene().name == "Stage_Scene")
        {
            timeText.text = GameManager.Instance.Day + " 일";
        }
        else
        {
            if (playerController.currentState == PlayerController.PlayerState.Idle)
            {
                timeText.text = angleUnit + " 각";
            }
            else
            {
                timeText.text = null;
            }
        }
        //print(playerItemUse);
        if (playerItemUse == null)
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            Item item = playerItemUse.quickSlots[i];

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
            slotBackgrounds[i].sprite = (i == playerItemUse.selectedSlotIndex)
                ? selectedSlotSprite
                : defaultSlotSprite;
        }
        // 선택된 슬롯의 아이템 정보만 UI에 표시
        Item selectedItem = playerItemUse.quickSlots[playerItemUse.selectedSlotIndex];
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
    public void UpdateUI(Item[] quickSlots, int selectedIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            Item item = quickSlots[i];

            if (item != null && !string.IsNullOrEmpty(item.itemName))
            {
                slotImages[i].sprite = item.icon;

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
                slotImages[i].sprite = default_Item_Sprite;
                slotCounts[i].gameObject.SetActive(false);
            }

            slotBackgrounds[i].sprite = (i == selectedIndex) ? selectedSlotSprite : defaultSlotSprite;
        }

        Item selectedItem = quickSlots[selectedIndex];
        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.itemName))
        {
            int total_coin = selectedItem.Coin * selectedItem.Count;
            int total_Weight = selectedItem.Weight * selectedItem.Count;
            Item_Name.text = $"[{selectedItem.itemName}]";
            Item_Coin.text = $"{total_coin} 냥";
            Item_Weight.text = $"{total_Weight} 근";
        }
        else
        {
            Item_Name.text = null;
            Item_Coin.text = null;
            Item_Weight.text = null;
        }
    }
}
