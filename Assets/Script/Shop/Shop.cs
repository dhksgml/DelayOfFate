using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject ShopPrefab; // 모든 상점 요소
    public GameObject QuestPrefab; //미션 카드 3개

    public float Gold;
    public float Soul;
    public int day;

    private int rerollCost = 30;
    private int lanternBuyCount = 0;

    private const int lanternFirstPrice = 750;
    private const int lanternSecondPrice = 1500;

    public TMP_Text coin_text;
    public TMP_Text soul_text;
    // 테스트용 무기/영혼 데이터
    private List<string> weaponNames = new List<string> { "Sword", "Axe", "Bow", "Spear", "Gun" };
    private List<int> weaponPrices = new List<int>();

    private List<string> soulNames = new List<string>();
    private List<int> soulPrices = new List<int>();
    private bool[] soulPurchased = new bool[4]; // 영혼 4개 구매 여부

    public TMP_Text[] weaponSlots; // 상품 목록들 무기, 영혼, 초롱
    public Item[] weaponData; // 무기 데이터
    public QuickSlotUI quickSlotUI; // 퀵슬롯 연결

    void Start()
    {
        InitializeShop();
    }
    private void Update()
    {
        Gold = GameManager.Instance.Gold;
        Soul = GameManager.Instance.Soul;
        coin_text.text = $"{Gold} :냥";
        soul_text.text = $"{Soul} :혼";
    }

    public void InitializeShop()
    {
        weaponPrices.Clear();
        for (int i = 0; i < weaponNames.Count; i++)
        {
            weaponPrices.Add(day * 100);
            weaponSlots_text(i, day * 100, "Soul");
        }

        weaponSlots_text(9, lanternFirstPrice, "Gold"); // 초롱가격
        weaponSlots_text(10, rerollCost, "Gold"); // 리롤 가격

        // 영혼 구매 상태 초기화
        soulPurchased = new bool[4];
        RerollSouls();

    }
    void weaponSlots_text(int Slot,int coin,string name)
    {
        weaponSlots[Slot].text = (coin).ToString();
        if (name == "Gold")
        {
            weaponSlots[Slot].text += " 냥";
        }
        else if (name == "Soul")
        {
            weaponSlots[Slot].text += " 혼";
        }
        
    }
    public void BuyWeapon(int index) // 무기 구매
    {
        if (index < 0 || index >= weaponNames.Count) return;

        int price = weaponPrices[index];
        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            weaponSlots[index].text = "구매 완료";

            Button btn = weaponSlots[index].GetComponentInParent<Button>();
            if (btn != null) btn.interactable = false;

            // 내부에서 바로 퀵슬롯 참조
            ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
            print(shopQuickSlot);
            if (shopQuickSlot == null) return;

            bool slotFilled = false;
            for (int i = 0; i < shopQuickSlot.quickSlots.Length; i++)
            {
                Item item = shopQuickSlot.quickSlots[i];
                if (item == null || string.IsNullOrEmpty(item.itemName))
                {
                    shopQuickSlot.quickSlots[i] = weaponData[index];
                    OnItemHover(i, weaponData[index]);
                    slotFilled = true;
                    quickSlotUI.UpdateUI();
                    break;
                }
            }

            if (!slotFilled)
            {
                Debug.Log("퀵슬롯이 모두 찼습니다.");
            }
        }
    }



    public void OnItemHover(int i, Item item)
    {
        QuickSlotUI quickSlotUI = FindObjectOfType<QuickSlotUI>();
        if (quickSlotUI != null)
        {
            quickSlotUI.DisplayItemInfo(i, item);
        }
    }
    public void BuySoul(int index) //혼령 강화
    {
        if (index < 0 || index >= soulNames.Count) return;

        if (soulPurchased[index]) return; // 이미 구매했으면 무시

        int price = soulPrices[index];
        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            soulPurchased[index] = true; // 구매 상태 저장

            weaponSlots[index + 5].text = "구매 완료";
            Button btn = weaponSlots[index + 5].GetComponentInParent<Button>();
            if (btn != null)
            {
                btn.interactable = false;
            }
        }
        else
        {
            Debug.Log("Not enough coins to buy soul.");
        }
    }


    public void BuyLantern() // 호롱 업글
    {
        if (lanternBuyCount >= 2)
        {
            Debug.Log("Lantern cannot be purchased anymore.");
            return;
        }

        int price = 0;

        if (lanternBuyCount == 0)
        {
            price = lanternFirstPrice; // 2단계
        }
        else if (lanternBuyCount == 1)
        {
            price = lanternSecondPrice; // 3단계
        }

        if (Gold >= price)
        {
            GameManager.Instance.Sub_Gold(price);
            lanternBuyCount++;

            // 다음 단계 가격 표시 또는 "구매 완료"
            if (lanternBuyCount == 2)
            {
                weaponSlots[9].text = "구매 완료";
            }
            else
            {
                int nextPrice = (lanternBuyCount == 1) ? lanternSecondPrice : 0;
                weaponSlots_text(9, nextPrice, "Gold");
            }

            Debug.Log($"Purchased lantern ({lanternBuyCount}/2) for {price} coins.");
        }
        else
        {
            Debug.Log("Not enough coins to buy lantern.");
        }
    }


    public void RerollSouls()
    {
        // 리스트 크기 보장
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

        for (int i = 0; i < 4; i++)
        {
            if (soulPurchased[i]) continue; // 구매된 건 유지

            string name = $"Soul_{Random.Range(1, 100)}";
            int price = Random.Range(80, 201);

            soulNames[i] = name;
            soulPrices[i] = price;
            weaponSlots_text(5 + i, price, "Soul");
        }
    }
    public void Quest_ok() // 미션을 고른 후 상점 페이지로 전환
    {
        ShopPrefab.SetActive(true);
        QuestPrefab.SetActive(false);
    }
    public void Shop_end() // 상점 전부 고른 후 전투씬으로 넘어가기
    {
        SceneManager.LoadScene("InGame_Scenes");
    }


    public void Reroll()
    {
        if (Gold >= rerollCost)
        {
            GameManager.Instance.Sub_Gold(rerollCost);
            rerollCost += 30;
            weaponSlots_text(10, rerollCost, "Gold");
            RerollSouls();
            Debug.Log($"Rerolled souls. Next reroll cost: {rerollCost}");
        }
        else
        {
            Debug.Log("Not enough coins to reroll.");
        }
    }
}
