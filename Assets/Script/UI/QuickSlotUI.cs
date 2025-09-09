using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuickSlotUI : MonoBehaviour
{
    public Image[] weaponSlotImage;
    //public Image[] weaponSlotBackgrounds;
    public Image[] slotImages;          // 각 슬롯의 아이템 아이콘
    public Image[] slotBackgrounds;     // 각 슬롯의 배경 이미지 (활성화 표시)
    public TMP_Text[] weapon_Count;     // 무기 개수 (없애야 하나 고민중)
    public TMP_Text weapon_name;     // 무기 이름 
    public Sprite default_Item_Sprite;  // 기본 아이템 아이콘
    public Sprite defaultSlotSprite;    // 기본 슬롯 배경
    public Sprite selectedSlotSprite;   // 선택된 슬롯 배경
    public TMP_Text Item_Name;          // 선택한 아이템의 이름
    public TMP_Text Item_Coin;          // 선택한 아이템의 가치
    //public TMP_Text Use_text;           // 사용 가능한 아이템이라면 표기될 텍스트
    public TMP_Text Discard_text;       // 아이템을 들고 있다면 버리기 텍스트 표기
    //public TMP_Text Item_Weight;        // 선택한 아이템의 무게

    public Player_Item_Use playerItemUse;
    private PlayerController playerController;

    public TMP_Text timeText; // UI 텍스트 오브젝트
    public int angleUnit = 0;
    private float angleStartTime;

    private bool isAngleUnit18;

    void Start()
    {
        Item_Name.text = null;
        Item_Coin.text = null;
        //Item_Weight.text = null;
        playerItemUse = FindObjectOfType<Player_Item_Use>();
        playerController = FindObjectOfType<PlayerController>();
        angleStartTime = Time.time;
        ResetAngleUnit();
        UpdateUI();
    }
    void Update()
    {
        float elapsed = Time.time - angleStartTime;
        //float time = Time.time; // 경과 시간 (초)
        angleUnit = Mathf.FloorToInt(elapsed / 20f); // 20초마다 1각

        if(angleUnit >= 18 && !isAngleUnit18)
        {
            isAngleUnit18 = true;
            GameEvents.CallTimeAngleUnit18();
        }
    }

    public void DisplayItemInfo(int index, ItemData item)
    {

        if (index < 0 || index >= 4) return;

        if (item != null && !string.IsNullOrEmpty(item.itemName))
        {
            slotImages[index].sprite = item.InGameSprite;
            slotImages[index].color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            // 빈 슬롯 처리
            slotImages[index].sprite = default_Item_Sprite;
            slotImages[index].color = new Color(1f, 1f, 1f, 0.3f);
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
        }
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
                slotImages[i].sprite = item.InGameSprite;
                slotImages[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                // 빈 슬롯 처리
                slotImages[i].sprite = default_Item_Sprite;
                slotImages[i].color = new Color(1f, 1f, 1f, 0.3f);
            }

            // 선택된 슬롯 배경 표시
            slotBackgrounds[i].sprite = (i == playerItemUse.selectedSlotIndex)
                ? selectedSlotSprite
                : defaultSlotSprite;
        }
        int currentIndex = playerItemUse.selectedWeaponIndex;
        int otherIndex = (currentIndex + 1) % 2;

        // 현재 무기 / 반대 무기
        Item currentItem = playerItemUse.weaponSlots[currentIndex];
        Item otherItem = playerItemUse.weaponSlots[otherIndex];

        // 메인 슬롯 (선택된 무기)
        if (currentItem != null && !string.IsNullOrEmpty(currentItem.itemName))
        {
            weaponSlotImage[0].sprite = currentItem.InGameSprite;
            weaponSlotImage[0].color = Color.white;

            if (currentItem.Count_Check)
            {
                weapon_Count[0].text = currentItem.Count.ToString();
                weapon_Count[0].gameObject.SetActive(true);
            }
            else
            {
                weapon_Count[0].gameObject.SetActive(false);
            }

            weapon_name.gameObject.SetActive(true);
            weapon_name.text = "[" + currentItem.itemName + "]";
        }
        else
        {
            weaponSlotImage[0].sprite = default_Item_Sprite;
            weaponSlotImage[0].color = new Color(1f, 1f, 1f, 0.3f);
            weapon_Count[0].gameObject.SetActive(false);
            weapon_name.gameObject.SetActive(false);
        }

        // 보조 슬롯 (반대 무기)
        if (otherItem != null && !string.IsNullOrEmpty(otherItem.itemName))
        {
            weaponSlotImage[1].sprite = otherItem.InGameSprite;
            weaponSlotImage[1].color = Color.white;

            if (otherItem.Count_Check)
            {
                weapon_Count[1].text = otherItem.Count.ToString();
                weapon_Count[1].gameObject.SetActive(true);
            }
            else
            {
                weapon_Count[1].gameObject.SetActive(false);
            }
        }
        else
        {
            weaponSlotImage[1].sprite = default_Item_Sprite;
            weaponSlotImage[1].color = new Color(1f, 1f, 1f, 0.3f);
            weapon_Count[1].gameObject.SetActive(false);
        }

        // 선택된 슬롯의 아이템 정보만 UI에 표시
        Item selectedItem = playerItemUse.quickSlots[playerItemUse.selectedSlotIndex];
        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.itemName))
        {
            int total_coin = selectedItem.Coin * selectedItem.Count;
            //int total_Weight = selectedItem.Weight * selectedItem.Count;
            Item_Name.text = string.Format("[{0}]", selectedItem.itemName);
            Item_Coin.text = total_coin.ToString() + " 값";
            //Item_Weight.text = total_Weight.ToString() + " 근";
            //if (selectedItem.isUsable) { Use_text.text = "[<space=15><voffset=14><sprite=1><voffset=0><space=-25>] 사용</voffset>"; } else { Use_text.text = null; } //사용 가능한 경우만 표기
            Discard_text.text = "[<b>F</b>] 즉시판매";
        }
        else
        {
            Item_Name.text = null;
            Item_Coin.text = null;
            //Item_Weight.text = null;
            //Use_text.text = null;
            Discard_text.text = null;
        }
    }
    /*public void UpdateUI(Item[] quickSlots, int selectedIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            Item item = quickSlots[i];

            if (item != null && !string.IsNullOrEmpty(item.itemName))
            {
                slotImages[i].sprite = item.InGameSprite;
            }
            slotBackgrounds[i].sprite = (i == selectedIndex) ? selectedSlotSprite : defaultSlotSprite;
        }

        Item selectedItem = quickSlots[selectedIndex];
        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.itemName))
        {
            int total_coin = selectedItem.Coin * selectedItem.Count;
            //int total_Weight = selectedItem.Weight * selectedItem.Count;
            Item_Name.text = $"[{selectedItem.itemName}]";
            Item_Coin.text = $"{total_coin} 값";
            //Item_Weight.text = $"{total_Weight} 근";
        }
        else
        {
            Item_Name.text = null;
            Item_Coin.text = null;
            //Item_Weight.text = null;
        }
    }*/

    public void ResetAngleUnit()
    {
        angleStartTime = Time.time;
    }
}
