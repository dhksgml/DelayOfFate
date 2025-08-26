using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public float Gold;
    public float Soul;

    private int rerollCost = 30;

    private const int lantern_1 = 500;
    private const int lantern_2 = 1000;

    private List<int> weaponPrices = new List<int>();

    private List<string> soulNames = new List<string>();
    private List<int> soulPrices = new List<int>();
    private bool[] soulPurchased = new bool[4]; // 영혼 4개 구매 여부

    private List<string> allSoulIds = new List<string>();

    public Image[] soulIcons; // UI에 보여줄 아이콘 4개

    public TMP_Text[] weaponSlots; // 상품 목록들 무기, 영혼, 초롱
    public ItemData[] weaponData; // 무기 데이터
    public GameObject ch_soul_gold_bt;//교환 버튼 (비활성화 용)
    public GameObject ch_gold_soul_bt;//교환 버튼 (비활성화 용)
    public QuickSlotUI quickSlotUI; // 퀵슬롯 연결
    private PassiveItemManager passiveItemManager;
    void Awake()
    {
        passiveItemManager = FindObjectOfType<PassiveItemManager>();

        allSoulIds.Clear();

        // Build base list: groups 1..7, numbers 1..2
        for (int g = 1; g <= 7; g++)
        {
            // Skip all group 2 (refining, temporary)
            if (g == 2) continue;

            for (int n = 1; n <= 2; n++)
            {
                // Skip 6_1 only (temporary)
                if (g == 6 && n == 1) continue;

                allSoulIds.Add($"Soul_Add_{g}_{n}");
            }
        }

        // Manually add known _3 variants (except 2_3 which we are excluding)
        allSoulIds.Add("Soul_Add_4_3");
        allSoulIds.Add("Soul_Add_6_3"); // allowed; only 6_1 is excluded
        
        RerollSouls(); // run before Start
    }


    void Start()
    {
        InitializeShop();
        if (GameManager.Instance.Day == 1) ch_bt_1day_no(); //1일차면 교환 막기 + 환도 강제 구매
        passiveItemManager = FindObjectOfType<PassiveItemManager>();
    }
    void Update()
    {
        Gold = GameManager.Instance.Gold;
        Soul = GameManager.Instance.Soul;
    }
    void InitializeShop()
    {
        weaponPrices.Clear();
        for (int i = 0; i < 5; i++)
        {
            weaponPrices.Add(GameManager.Instance.Day * 100);
            if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_3_2")) //다다익선 보유시
            {
                weaponSlots_text(i, 0, "Gold");
            }
            else
            {
                weaponSlots_text(i, GameManager.Instance.Day * 100, "Gold");
            }
        }

        weaponSlots_text(9, lantern_1, "Soul"); // 초롱가격
        weaponSlots_text(10, rerollCost, "Soul"); // 리롤 가격

        // 영혼 구매 상태 초기화
        soulPurchased = new bool[4];
        RerollSouls();
    }
    void weaponSlots_text(int Slot,int coin,string name)
    {
        weaponSlots[Slot].text = (coin).ToString();
        if (name == "Soul")
        {
            weaponSlots[Slot].text += "<sprite=8> ";
        }
        else if (name == "Gold")
        {
            weaponSlots[Slot].text += "<sprite=9> ";
        }
    }
    public void BuyWeapon(int index) // 무기 구매
    {
        if (index < 0 || index >= 5) return;

        int price = weaponPrices[index];

        if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_3_2"))
            price = 0;

        if (Gold < price) return;

        // 내부에서 바로 퀵슬롯 참조
        ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
        if (shopQuickSlot == null) return;

        // 빈 슬롯 찾기
        int emptySlotIndex = -1;
        for (int i = 0; i < shopQuickSlot.quickSlots.Length; i++)
        {
            ItemData item = shopQuickSlot.SlotsData[i];
            if (item == null || string.IsNullOrEmpty(item.itemName))
            {
                emptySlotIndex = i;
                break;
            }
        }

        if (emptySlotIndex == -1)
        {
            Debug.Log("퀵슬롯이 모두 찼습니다.");
            return;
        }

        // 다다익선 효과가 없을 때만 소울 차감
        bool hasSoulAddEffect = PassiveItemManager.Instance != null &&
                                PassiveItemManager.Instance.HasEffect("Soul_Add_3_2");
        if (!hasSoulAddEffect)
        {
            GameManager.Instance.Sub_Gold(price);
        }

        weaponSlots[index].text = "구매 완료";

        GameEvents.CallBuyWeapon();

        Button btn = weaponSlots[index].GetComponentInParent<Button>();
        if (btn != null) btn.interactable = false;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));

        shopQuickSlot.SlotsData[emptySlotIndex] = weaponData[index];
        OnItemHover(emptySlotIndex, weaponData[index]);
    }

    public void UpdateWeaponPrice()
    {
        if (PassiveItemManager.Instance == null) return;

        for (int i = 0; i < 5; i++)
        {
            if (PassiveItemManager.Instance.HasEffect("Soul_Add_3_2")) //다다익선 보유시
            {
                weaponSlots_text(i, 0, "Soul");
            }
            else
            {
                weaponSlots_text(i, GameManager.Instance.Day * 100, "Soul");
            }
        }
    }
    public void OnItemHover(int i, ItemData item)
    {
        QuickSlotUI quickSlotUI = FindObjectOfType<QuickSlotUI>();
        if (quickSlotUI != null)
        {
            quickSlotUI.DisplayItemInfo(i, item);
        }
    }
    public void BuySoul(int index)
    {
        if (index < 0 || index >= soulNames.Count) return;
        if (soulPurchased[index]) return;

        int price = soulPrices[index];
        if (Gold >= price)
        {
            GameManager.Instance.Sub_Gold(price);
            soulPurchased[index] = true;

            weaponSlots[index + 5].text = "구매 완료";
            Button btn = weaponSlots[index + 5].GetComponentInParent<Button>();
            Soul_in_text slot = soulIcons[index].GetComponentInParent<Soul_in_text>();
            if (btn != null)
            {
                btn.interactable = false;
                slot.show = false; // 구매 완료 한건 살펴 보기 해도 안보이고 인벤토리 가서 봐야함
            }
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_2"));
            // 구매 효과 적용 신호 보내기
            string itemId = soulNames[index]; // ← 이미 RerollSouls()에서 할당됨
            PassiveItemManager.Instance.PurchaseItem(itemId);
            UpdateWeaponPrice();
        }
        else
        {
            Debug.Log("Not enough coins to buy soul.");
        }
    }
    public void BuyLantern() // 호롱 업글
    {
        int F_leval = GameManager.Instance.playerData.flashLightLevel;
        if (F_leval >= 2)
        {
            Debug.Log("Lantern cannot be purchased anymore.");
            return;
        }

        int price = 0;

        if (F_leval == 0)
        {
            price = lantern_1; // 2단계
        }
        else if (F_leval == 1)
        {
            price = lantern_2; // 3단계
        }

        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            GameManager.Instance.playerData.flashLightLevel = Mathf.Clamp(GameManager.Instance.playerData.flashLightLevel + 1, 1, 3);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_2"));
            // 다음 단계 가격 표시 또는 "구매 완료"
            if (F_leval == 2)
            {
                weaponSlots[9].text = "구매 완료";
            }
            else
            {
                int nextPrice = (F_leval == 1) ? lantern_2 : 0;
                weaponSlots_text(9, nextPrice, "Soul");
            }

            Debug.Log($"Purchased lantern ({F_leval}/2) for {price} coins.");
        }
        else
        {
            Debug.Log("Not enough coins to buy lantern.");
        }
    }


    public void RerollSouls()
    {
        // 리스트 초기화 보장
        if (soulNames.Count < 4)
        {
            soulNames.Clear();
            soulPrices.Clear();
            for (int i = 0; i < 4; i++)
            {
                soulNames.Add("");
                soulPrices.Add(0);
            }
        }

        // 1. 후보군 만들기 (미구매 아이템만)
        List<string> availableSouls = new List<string>();
        foreach (var id in allSoulIds)
        {
            if (!PassiveItemManager.Instance.IsPurchased(id)) // 구매 안 한 것만
                availableSouls.Add(id);
        }

        // 2. 랜덤 섞기 & 4개 추출 (중복 제거)
        availableSouls = availableSouls.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < 4; i++)
        {
            if (soulPurchased[i]) continue;

            if (i >= availableSouls.Count)
            {
                Debug.LogWarning("미구매 아이템이 4개 미만입니다!");
                soulNames[i] = "";
                soulPrices[i] = 0;
                weaponSlots_text(5 + i, 0, "Gold");
                soulIcons[i].sprite = null;
                continue;
            }

            string id = availableSouls[i];
            soulNames[i] = id;
            PassiveItemData itemData = passiveItemManager.passiveItems.Find(x => x.id == id);
            int rating = itemData != null ? itemData.rating : 1; // 기본값은 1
            switch (rating)
            {
                case 1:
                    soulPrices[i] = 150;
                    soulPrices[i] += Random.Range(-10, +11);
                    break;
                case 2:
                    soulPrices[i] = 215;
                    soulPrices[i] += Random.Range(-15, +16);
                    break;
                case 3:
                    soulPrices[i] = 300;
                    soulPrices[i] += Random.Range(-20, +21);
                    break;
                case 4:
                    soulPrices[i] = 400;
                    soulPrices[i] += Random.Range(-25, +26);
                    break;
                default:
                    break;
            }
            // UI 텍스트 갱신
            weaponSlots_text(5 + i, soulPrices[i], "Gold");

            // 아이콘 갱신
            SetSoulIcon(i, id);

            // 슬롯에 있는 ShopSlot 컴포넌트에 itemId 전달
            Soul_in_text slot = soulIcons[i].GetComponentInParent<Soul_in_text>();
            if (slot != null)
            {
                slot.itemId = id;
            }

            // 콘솔에 어떤 아이템이 배치되었는지 출력 (디버그용)
            Debug.Log($"슬롯 {i}번 → {id}, 가격: {soulPrices[i]} 혼");
        }
    }

    void SetSoulIcon(int slotIndex, string id)
    {
        // 예: "Soul_Add_2_3" → group = 2, num = 3
        string[] parts = id.Split('_');
        int group = int.Parse(parts[2]); // 1~7
        int number = int.Parse(parts[3]); // 1~3

        Sprite icon = passiveItemManager.GetIcon(group, number);
        if (soulIcons[slotIndex] != null)
            soulIcons[slotIndex].sprite = icon;
    }

    public void Soul_c_Gold() // 100 혼 → 50 전
    {
        if (Soul >= 100f)
        {
            GameManager.Instance.Sub_Soul(100);
            GameManager.Instance.Add_Gold(50);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else
        {
            Debug.Log("혼이 부족합니다.");
        }
    }

    public void Goul_c_Soul() // 100 전 → 50 혼
    {
        if (Gold >= 100f)
        {
            GameManager.Instance.Sub_Gold(100);
            GameManager.Instance.Add_Soul(50);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else
        {
            Debug.Log("전이 부족합니다.");
        }
    }

    void ch_bt_1day_no()//1일차에 버튼 비활성화
    {
        ch_soul_gold_bt.gameObject.SetActive(false);
        ch_gold_soul_bt.gameObject.SetActive(false);
    }

    public void Reroll()
    {
        if (Soul >= rerollCost)
        {
            GameManager.Instance.Sub_Soul(rerollCost);
            rerollCost += 30;
            weaponSlots_text(10, rerollCost, "Soul");
            RerollSouls();
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
            Debug.Log($"Rerolled souls. Next reroll cost: {rerollCost}");
        }
        else
        {
            Debug.Log(Gold);
        }
    }
}
