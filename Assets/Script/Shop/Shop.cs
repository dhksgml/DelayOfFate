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
    private int lanternBuyCount = 0;

    private const int lantern_1 = 750;
    private const int lantern_2 = 1500;

    private List<int> weaponPrices = new List<int>();

    private List<string> soulNames = new List<string>();
    private List<int> soulPrices = new List<int>();
    private bool[] soulPurchased = new bool[4]; // ��ȥ 4�� ���� ����

    private List<string> allSoulIds = new List<string>();

    public Image[] soulIcons; // ȥ�ɰ�ȭ 4��
    public Image[] weaponIcons; // ���� 5(3)��
    public Image LightIcon;

    public TMP_Text[] weaponSlots; // ��ǰ ��ϵ� ����, ��ȥ, �ʷ�
    public ItemData[] weaponData; // ���� ������
    public QuickSlotUI quickSlotUI; // ������ ����
    private PassiveItemManager passiveItemManager;
    void Awake()
    {
        passiveItemManager = FindObjectOfType<PassiveItemManager>();
        //SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/"));
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
        passiveItemManager = FindObjectOfType<PassiveItemManager>();
        for (int i = 1; i < 4; i++) //����鿡�� ���� �ۼ�
        {
            Soul_in_text slot = weaponIcons[i-1].GetComponentInParent<Soul_in_text>();
            Soul_in_text l_slot = LightIcon.GetComponentInParent<Soul_in_text>();
            if (slot != null)
            {
                slot.itemId = "Weapon_" + i;
                l_slot.itemId = "Light_" + i;
            }
        }
    }
    void Update()
    {
        Gold = GameManager.Instance.Gold;
        Soul = GameManager.Instance.Soul;
    }
    void InitializeShop()
    {
        weaponPrices.Clear();
        for (int i = 0; i < 3; i++)
        {
            weaponPrices.Add(GameManager.Instance.Day * 100);
            weaponSlots_text(i, GameManager.Instance.Day * 100, "Soul");
        }

        weaponSlots_text(9, lantern_1, "Gold"); // �ʷհ���
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
        if (index < 0 || index >= 3) return;

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
                ItemData item = shopQuickSlot.SlotsData[i]; // ���⸦ �ٲ�
                if (item == null || string.IsNullOrEmpty(item.itemName))
                {
                    shopQuickSlot.SlotsData[i] = weaponData[index];
                    if (weaponData[index].id == 997) { shopQuickSlot.SlotsData[i].Count = 10; }//������ ���
                    OnItemHover(i, weaponData[index]);
                    slotFilled = true;
                    break;
                }
            }

            if (!slotFilled)
            {
                Debug.Log("�������� ��� á���ϴ�.");
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
        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            soulPurchased[index] = true;

            weaponSlots[index + 5].text = "���� �Ϸ�";
            Button btn = weaponSlots[index + 5].GetComponentInParent<Button>();
            Soul_in_text slot = soulIcons[index].GetComponentInParent<Soul_in_text>();
            if (btn != null)
            {
                btn.interactable = false;
                slot.show = false; // ���� �Ϸ� �Ѱ� ���� ���� �ص� �Ⱥ��̰� �κ��丮 ���� ������
            }

            // ���� ȿ�� ���� ��ȣ ������
            string itemId = soulNames[index]; // �� �̹� RerollSouls()���� �Ҵ��
            passiveItemManager.PurchaseItem(itemId);
            Debug.Log($"ȥ�� ����: {itemId}");
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
            price = lantern_1; // 2�ܰ�
        }
        else if (lanternBuyCount == 1)
        {
            price = lantern_2; // 3�ܰ�
        }

        if (Gold >= price)
        {
            GameManager.Instance.Sub_Gold(price);
            lanternBuyCount++;
            Soul_in_text l_slot = LightIcon.GetComponentInParent<Soul_in_text>();
            l_slot.itemId = "Light_" + lanternBuyCount+1;
            // ���� �ܰ� ���� ǥ�� �Ǵ� "���� �Ϸ�"
            if (lanternBuyCount == 2)
            {
                weaponSlots[9].text = "���� �Ϸ�";
            }
            else
            {
                int nextPrice = (lanternBuyCount == 1) ? lantern_2 : 0;
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
        // ����Ʈ �ʱ�ȭ ����
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

        // 1. �ĺ��� ����� (�̱��� �����۸�)
        List<string> availableSouls = new List<string>();
        foreach (var id in allSoulIds)
        {
            if (!passiveItemManager.IsPurchased(id)) // ���� �� �� �͸�
                availableSouls.Add(id);
        }

        // 2. ���� ���� & 4�� ���� (�ߺ� ����)
        availableSouls = availableSouls.OrderBy(x => Random.value).ToList();
        print(availableSouls);
        for (int i = 0; i < 4; i++)
        {
            if (soulPurchased[i]) continue;

            if (i >= availableSouls.Count)
            {
                Debug.LogWarning("�̱��� �������� 4�� �̸��Դϴ�!");
                soulNames[i] = "";
                soulPrices[i] = 0;
                weaponSlots_text(5 + i, 0, "Soul");
                soulIcons[i].sprite = null;
                continue;
            }

            string id = availableSouls[i];
            soulNames[i] = id;
            PassiveItemData itemData = passiveItemManager.passiveItems.Find(x => x.id == id);
            int rating = itemData != null ? itemData.rating : 1; // �⺻���� 1
            switch (rating)
            {
                case 1:
                    soulPrices[i] = 120;
                    break;
                case 2:
                    soulPrices[i] = 170;
                    break;
                case 3:
                    soulPrices[i] = 130;
                    break;
                case 4:
                    soulPrices[i] = 160;
                    break;
                default:
                    soulPrices[i] = 100;
                    break;
            }
            soulPrices[i] += Random.Range(-10, +11);

            // UI �ؽ�Ʈ ����
            weaponSlots_text(5 + i, soulPrices[i], "Soul");

            // ������ ����
            SetSoulIcon(i, id);

            // ���Կ� �ִ� ShopSlot ������Ʈ�� itemId ����
            Soul_in_text slot = soulIcons[i].GetComponentInParent<Soul_in_text>();
            if (slot != null)
            {
                slot.itemId = id;
            }

            // �ֿܼ� � �������� ��ġ�Ǿ����� ��� (����׿�)
            Debug.Log($"���� {i}�� �� {id}, ����: {soulPrices[i]} ȥ");
        }
    }

    void SetSoulIcon(int slotIndex, string id)
    {
        // ��: "Soul_Add_2_3" �� group = 2, num = 3
        string[] parts = id.Split('_');
        int group = int.Parse(parts[2]); // 1~7
        int number = int.Parse(parts[3]); // 1~3

        Sprite icon = passiveItemManager.GetIcon(group, number);
        if (soulIcons[slotIndex] != null)
            soulIcons[slotIndex].sprite = icon;
    }

    public void Soul_c_Gold()//100ȥ �� > 50 ������
    {
        GameManager.Instance.Sub_Soul(100);
        GameManager.Instance.Add_Gold(50);
    }
    public void Goul_c_Soul()//100�� �� > 50 ȥ����
    {
        GameManager.Instance.Sub_Gold(100);
        GameManager.Instance.Add_Soul(50);
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
            Debug.Log(Gold);
        }
    }
}
