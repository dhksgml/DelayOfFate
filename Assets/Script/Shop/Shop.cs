using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject ShopPrefab; // ��� ���� ���
    public GameObject QuestPrefab; //�̼� ī�� 3��

    public float Gold;
    public float Soul;
    public int day;

    private int rerollCost = 30;
    private int lanternBuyCount = 0;

    private const int lanternFirstPrice = 750;
    private const int lanternSecondPrice = 1500;

    public TMP_Text coin_text;
    public TMP_Text soul_text;
    // �׽�Ʈ�� ����/��ȥ ������
    private List<string> weaponNames = new List<string> { "Sword", "Axe", "Bow", "Spear", "Gun" };
    private List<int> weaponPrices = new List<int>();

    private List<string> soulNames = new List<string>();
    private List<int> soulPrices = new List<int>();
    private bool[] soulPurchased = new bool[4]; // ��ȥ 4�� ���� ����

    public TMP_Text[] weaponSlots; // ��ǰ ��ϵ� ����, ��ȥ, �ʷ�
    public Item[] weaponData; // ���� ������
    public QuickSlotUI quickSlotUI; // ������ ����

    void Start()
    {
        InitializeShop();
    }
    private void Update()
    {
        Gold = GameManager.Instance.Gold;
        Soul = GameManager.Instance.Soul;
        coin_text.text = $"{Gold} :��";
        soul_text.text = $"{Soul} :ȥ";
    }

    public void InitializeShop()
    {
        weaponPrices.Clear();
        for (int i = 0; i < weaponNames.Count; i++)
        {
            weaponPrices.Add(day * 100);
            weaponSlots_text(i, day * 100, "Soul");
        }

        weaponSlots_text(9, lanternFirstPrice, "Gold"); // �ʷհ���
        weaponSlots_text(10, rerollCost, "Gold"); // ���� ����

        // ��ȥ ���� ���� �ʱ�ȭ
        soulPurchased = new bool[4];
        RerollSouls();

    }
    void weaponSlots_text(int Slot,int coin,string name)
    {
        weaponSlots[Slot].text = (coin).ToString();
        if (name == "Gold")
        {
            weaponSlots[Slot].text += " ��";
        }
        else if (name == "Soul")
        {
            weaponSlots[Slot].text += " ȥ";
        }
        
    }
    public void BuyWeapon(int index) // ���� ����
    {
        if (index < 0 || index >= weaponNames.Count) return;

        int price = weaponPrices[index];
        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            weaponSlots[index].text = "���� �Ϸ�";

            Button btn = weaponSlots[index].GetComponentInParent<Button>();
            if (btn != null) btn.interactable = false;

            // ���ο��� �ٷ� ������ ����
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
                Debug.Log("�������� ��� á���ϴ�.");
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
    public void BuySoul(int index) //ȥ�� ��ȭ
    {
        if (index < 0 || index >= soulNames.Count) return;

        if (soulPurchased[index]) return; // �̹� ���������� ����

        int price = soulPrices[index];
        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            soulPurchased[index] = true; // ���� ���� ����

            weaponSlots[index + 5].text = "���� �Ϸ�";
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


    public void BuyLantern() // ȣ�� ����
    {
        if (lanternBuyCount >= 2)
        {
            Debug.Log("Lantern cannot be purchased anymore.");
            return;
        }

        int price = 0;

        if (lanternBuyCount == 0)
        {
            price = lanternFirstPrice; // 2�ܰ�
        }
        else if (lanternBuyCount == 1)
        {
            price = lanternSecondPrice; // 3�ܰ�
        }

        if (Gold >= price)
        {
            GameManager.Instance.Sub_Gold(price);
            lanternBuyCount++;

            // ���� �ܰ� ���� ǥ�� �Ǵ� "���� �Ϸ�"
            if (lanternBuyCount == 2)
            {
                weaponSlots[9].text = "���� �Ϸ�";
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
        // ����Ʈ ũ�� ����
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
            if (soulPurchased[i]) continue; // ���ŵ� �� ����

            string name = $"Soul_{Random.Range(1, 100)}";
            int price = Random.Range(80, 201);

            soulNames[i] = name;
            soulPrices[i] = price;
            weaponSlots_text(5 + i, price, "Soul");
        }
    }
    public void Quest_ok() // �̼��� �� �� ���� �������� ��ȯ
    {
        ShopPrefab.SetActive(true);
        QuestPrefab.SetActive(false);
    }
    public void Shop_end() // ���� ���� �� �� ���������� �Ѿ��
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
