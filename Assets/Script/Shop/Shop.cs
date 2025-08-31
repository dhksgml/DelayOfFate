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
    private bool[] soulPurchased = new bool[4]; // ��ȥ 4�� ���� ����

    private List<string> allSoulIds = new List<string>();

    public Image[] soulIcons; // UI�� ������ ������ 4��

    public TMP_Text[] weaponSlots; // ��ǰ ��ϵ� ����, ��ȥ, �ʷ�
    public ItemData[] weaponData; // ���� ������
    public GameObject ch_soul_gold_bt;//��ȯ ��ư (��Ȱ��ȭ ��)
    public GameObject ch_gold_soul_bt;//��ȯ ��ư (��Ȱ��ȭ ��)
    public QuickSlotUI quickSlotUI; // ������ ����
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
        if (GameManager.Instance.Day == 1) ch_bt_1day_no(); //1������ ��ȯ ���� + ȯ�� ���� ����
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
            if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_3_2")) //�ٴ��ͼ� ������
            {
                weaponSlots_text(i, 0, "Gold");
            }
            else
            {
                weaponSlots_text(i, GameManager.Instance.Day * 100, "Gold");
            }
        }

        weaponSlots_text(9, lantern_1, "Soul"); // �ʷհ���
        weaponSlots_text(10, rerollCost, "Soul"); // ���� ����

        // ��ȥ ���� ���� �ʱ�ȭ
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
    public void BuyWeapon(int index) // ���� ����
    {
        if (index < 0 || index >= 5) return;

        int price = weaponPrices[index];

        if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_3_2"))
            price = 0;

        if (Gold < price) return;

        // ���ο��� �ٷ� ������ ����
        ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
        if (shopQuickSlot == null) return;

        // �� ���� ã��
        int emptySlotIndex = -1;
        for (int i = 0; i < shopQuickSlot.weaponSlotsData.Length; i++)
        {
            ItemData item = shopQuickSlot.weaponSlotsData[i];
            if (item == null || string.IsNullOrEmpty(item.itemName))
            {
                emptySlotIndex = i;
                break;
            }
        }

        if (emptySlotIndex == -1)
        {
            Debug.Log("�������� ��� á���ϴ�.");
            return;
        }

        // �ٴ��ͼ� ȿ���� ���� ���� �ҿ� ����
        bool hasSoulAddEffect = PassiveItemManager.Instance != null &&
                                PassiveItemManager.Instance.HasEffect("Soul_Add_3_2");
        if (!hasSoulAddEffect)
        {
            GameManager.Instance.Sub_Gold(price);
        }

        weaponSlots[index].text = "���� �Ϸ�";
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
            if (PassiveItemManager.Instance.HasEffect("Soul_Add_3_2")) //�ٴ��ͼ� ������
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

            weaponSlots[index + 5].text = "���� �Ϸ�";
            Button btn = weaponSlots[index + 5].GetComponentInParent<Button>();
            Soul_in_text slot = soulIcons[index].GetComponentInParent<Soul_in_text>();
            if (btn != null)
            {
                btn.interactable = false;
                slot.show = false; // ���� �Ϸ� �Ѱ� ���� ���� �ص� �Ⱥ��̰� �κ��丮 ���� ������
            }
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_2"));
            // ���� ȿ�� ���� ��ȣ ������
            string itemId = soulNames[index]; // �� �̹� RerollSouls()���� �Ҵ��
            PassiveItemManager.Instance.PurchaseItem(itemId);
            UpdateWeaponPrice();
        }
        else
        {
            Debug.Log("Not enough coins to buy soul.");
        }
    }
    public void BuyLantern() // ȣ�� ����
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
            price = lantern_1; // 2�ܰ�
        }
        else if (F_leval == 1)
        {
            price = lantern_2; // 3�ܰ�
        }

        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            GameManager.Instance.playerData.flashLightLevel = Mathf.Clamp(GameManager.Instance.playerData.flashLightLevel + 1, 1, 3);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_2"));
            // ���� �ܰ� ���� ǥ�� �Ǵ� "���� �Ϸ�"
            if (F_leval == 2)
            {
                weaponSlots[9].text = "���� �Ϸ�";
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
            if (!PassiveItemManager.Instance.IsPurchased(id)) // ���� �� �� �͸�
                availableSouls.Add(id);
        }

        // 2. ���� ���� & 4�� ���� (�ߺ� ����)
        availableSouls = availableSouls.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < 4; i++)
        {
            if (soulPurchased[i]) continue;

            if (i >= availableSouls.Count)
            {
                Debug.LogWarning("�̱��� �������� 4�� �̸��Դϴ�!");
                soulNames[i] = "";
                soulPrices[i] = 0;
                weaponSlots_text(5 + i, 0, "Gold");
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
            // UI �ؽ�Ʈ ����
            weaponSlots_text(5 + i, soulPrices[i], "Gold");

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

    public void Soul_c_Gold() // 100 ȥ �� 50 ��
    {
        if (Soul >= 100f)
        {
            GameManager.Instance.Sub_Soul(100);
            GameManager.Instance.Add_Gold(50);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else
        {
            Debug.Log("ȥ�� �����մϴ�.");
        }
    }

    public void Goul_c_Soul() // 100 �� �� 50 ȥ
    {
        if (Gold >= 100f)
        {
            GameManager.Instance.Sub_Gold(100);
            GameManager.Instance.Add_Soul(50);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else
        {
            Debug.Log("���� �����մϴ�.");
        }
    }

    void ch_bt_1day_no()//1������ ��ư ��Ȱ��ȭ
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
