using UnityEngine;

public class Player_Item_Use : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4���� ������
    public int selectedSlotIndex = 0; // ���� ���õ� ����
    public Item[] weaponSlots = new Item[2];
    public int selectedWeaponIndex = 0;
    public Transform dropPoint; // ������ ��� ��ġ
    public LayerMask itemLayer; // ������ ���̾� ����
    public GameObject item_Prefab; // �������� ���� �Ҷ� ��� �� �� ������
    private const float requiredHoldTime = 1f;
    private ItemUsageManager itemUsageManager;
    private PlayerController playercontroller;
    private Animator animator;
    private Item chargingItem = null;
    private Transform uiCanvas; // ����Ʈ�� ������ ��ġ
    public GameObject Sale_Effect; // ����Ʈ ������Ʈ

    // �Ҹ�Ϳ�
    public bool isItemTouch = false;

    //public static bool isAnyItemBeingSold = false; // ���� �ߺ� ���� �÷���
    private ItemObject currentSellingItem = null;

    void Start()
    {
        itemUsageManager = GetComponent<ItemUsageManager>();
        playercontroller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        uiCanvas = GameObject.Find("Player_Canvas")?.transform;
    }
    void Update()
    {
        //print(quickSlots[selectedSlotIndex]);
        HandleSlotSelection(); // ���� ���� ó��
        float weight = GetTotalItemWeight();
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedWeaponIndex >= 0 && selectedWeaponIndex < weaponSlots.Length)
            {
                Item selectedItem = weaponSlots[selectedWeaponIndex];
                if (selectedItem != null)
                {
                    UseItem(); // ���                   
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.F)) // ������
        {
            //DropItem();
            TutorialEvents.OnItemDropped?.Invoke(quickSlots[selectedSlotIndex]);
            playercontroller.OnPickUpStart(false);
            HandleSellAction();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !playercontroller.isRecovering) // �ݱ�
        {
            //if (!isAnyItemBeingSold)
            //{
                // �ֺ����� ��� �Ǹ� ������ ������ �� ù ��° �ϳ��� ã��
                Collider2D nearestItemCollider = Physics2D.OverlapCircle(transform.position, 1f, itemLayer);
                if (nearestItemCollider != null)
                {
                    ItemObject itemObject = nearestItemCollider.GetComponent<ItemObject>();
                    if (itemObject != null)
                    {
                        PickUpItem();
                        //isAnyItemBeingSold = true;
                        isItemTouch = true;
                        currentSellingItem = itemObject; // �� ����, ���� �Ǹ� ���� ������ ����
                    }
                }
            //}
        }
    }

    //���� ������ ������� �Ǵ�
    bool CheckCurrentSlotEmpty()
    {
        return quickSlots[selectedSlotIndex] == null;
    }

    void HandleSlotSelection()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            selectedWeaponIndex++;
            selectedWeaponIndex = selectedWeaponIndex % 2;
        }

        // ���� ���� (1~4 Ű)
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlotIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlotIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlotIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlotIndex = 3;

        // ���콺 �ٷ� ���� ����
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) selectedSlotIndex = (selectedSlotIndex + 3) % 4;  // ������ ��ũ�� (0~3 ������ ���ư�����)
        if (scroll < 0f) selectedSlotIndex = (selectedSlotIndex + 1) % 4;  // 0~3 ������ ���ư�����

        // selectedSlotIndex�� 0~3 ���� ���� �����ǵ��� ����
        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, 3);

        UpdateQuickSlotUI();
    }

    void TryUseItem(Item selectedItem)
    {
        if(!playercontroller.isRecovering)
        {
            if (selectedItem.id == 995) //������ ��� ��� ��� ���ŷ� ���
            {
                int mp_down = Random.Range(6 - 2, 6 + 2);
                if (!playercontroller.isRecovering && mp_down < playercontroller.currentMp)
                {
                    playercontroller.SpendMp(mp_down);
                } 
            }

            itemUsageManager.UseItem(selectedItem.itemName);
            TutorialEvents.OnWeaponUsed?.Invoke(selectedItem);
            animator.SetTrigger("Attack");
        }
    }

    void UseItem()
    {
        if (playercontroller.Player_Usage_cu_cool_down > 0f)
        {
            Debug.Log("������ ��� ��Ÿ�� ���Դϴ�.");
            return;
        }

        if (selectedWeaponIndex >= 0 && selectedWeaponIndex < weaponSlots.Length)
        {
            Item selectedItem = weaponSlots[selectedWeaponIndex];

            if (selectedItem != null)
            {
                // �ߺ� �������� ���


                if (selectedItem.id != 996)
                {
                    quickSlots[selectedSlotIndex] = null; 
                }
                


                // ������ ��� ó��
                TryUseItem(selectedItem);

                // ��ٿ� ����
                //playercontroller.Player_Usage_cu_cool_down = selectedItem.Usage_cool_down;
                //playercontroller.SetUseItemCooltime(selectedItem.Usage_cool_down);

                // UI ����
                UpdateQuickSlotUI();
            }
        }
    }

    public void PickUpItem()//�ݱ�
    {
        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, 1f, itemLayer);
        print("�ݱ� �ߵ�");
        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                print(itemObject);
                Item droppedItem = itemObject.itemData;
                Item slotItem = quickSlots[selectedSlotIndex];
                // ������ ����ִ� ���
                if (slotItem == null || string.IsNullOrEmpty(slotItem.itemName))
                {
                    print(slotItem);
                    TutorialEvents.OnItemPickedUp?.Invoke(droppedItem);
                    quickSlots[selectedSlotIndex] = droppedItem;
                    if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_pickup"));
                    Destroy(itemObject.gameObject);
                    UpdateQuickSlotUI();
                    GameEvents.CallPickupItem();
                }
                // ���Կ� �ٸ� �������� �ִ� ���
                else
                {
                    // ���� �ٸ� ���� �� �� ������ �ִ��� Ȯ��
                    bool placedInEmptySlot = false;
                    for (int i = 0; i < quickSlots.Length; i++)
                    {
                        if (quickSlots[i] == null || string.IsNullOrEmpty(quickSlots[i].itemName))
                        {
                            // �� ���� �߰� �� �� ���Կ� ������ �ֱ�
                            TutorialEvents.OnItemPickedUp?.Invoke(droppedItem);
                            quickSlots[i] = droppedItem;

                            if (SoundManager.Instance != null)
                                SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_pickup"));

                            Destroy(itemObject.gameObject);
                            UpdateQuickSlotUI();
                            GameEvents.CallPickupItem();

                            placedInEmptySlot = true;
                            break; // �� ���Կ��� �ְ� ����
                        }
                    }

                    // ��� ������ �� ���ִٸ� ���� ������� ���� ���԰� ��ü
                    if (!placedInEmptySlot)
                    {
                        DropItem();
                        quickSlots[selectedSlotIndex] = droppedItem;
                        Destroy(itemObject.gameObject);
                        UpdateQuickSlotUI();
                    }
                }

            }
        }
    }
    private void HandleSellAction()
    {
        // 1. �ٴ� ������ �ִ��� �˻�
        if (CheckCurrentSlotEmpty())
        {
            // �ٴ� ������ �Ǹ�
            RemoveItem();
            return;
        }
        // 2. ���� ���Կ� �������� �ִ��� Ȯ��
        SellCurrentSlotItem();
        return;
        
        // 3. �ƹ� �͵� ������ �ƹ� �ϵ� �� ��
    }

    // ���� ������ �Ǹ�
    private void SellCurrentSlotItem()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_drop"));
        Sale("one",0);
        UpdateQuickSlotUI();
        GameEvents.CallDropItem();
    }

    public void DropItem()
    {
        if (quickSlots[selectedSlotIndex] != null && !string.IsNullOrEmpty(quickSlots[selectedSlotIndex].itemName))
        {
            Item selectedItem = quickSlots[selectedSlotIndex];

            // �� ������ �������� ������� ���ο� ������ ����
            GameObject newItem = Instantiate(item_Prefab, dropPoint.position, Quaternion.identity);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_drop"));
            // ���� ������ �����ۿ� ItemObject ��ũ��Ʈ �߰� �� ������ ����
            ItemObject newItemComponent = newItem.GetComponent<ItemObject>();
            if (newItemComponent != null)
            {
                newItemComponent.itemData = selectedItem.Clone(); // ��ü ����
                newItemComponent.itemData = selectedItem; // ��ü ������ ������ ������
                newItemComponent.itemData.Drop_item = true; // *����Ʈ�� �� �ִ� ������ ���� ����*
            }

            SpriteRenderer spriteRenderer = newItem.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newItemComponent.itemData.InGameSprite;
            }

            // �����Կ��� �ش� ������ ����
            quickSlots[selectedSlotIndex] = null;
            UpdateQuickSlotUI();
            GameEvents.CallDropItem();
        }
    }
    public void Drop_All_Item()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] != null && !string.IsNullOrEmpty(quickSlots[i].itemName))
            {
                selectedSlotIndex = i;
                DropItem(); // ���� �޼��� ���
            }
        }
    }
    void RemoveItem() // ��� �Ǹ�
    {
        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, 1f, itemLayer);

        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();

            if (itemObject != null && itemObject.itemData != null)
            {
                Destroy(itemObject.gameObject);
                Sale("one", itemObject.itemData.Coin);
            }
        }
    }
    public void Sale(string ty, int add_coin) // "one" or "all"
    {
        int itemValue = 0;
        if (ty == "one")
        {
            if (quickSlots[selectedSlotIndex] == null)
            {
                itemValue = add_coin;
            }
            else
            {
                itemValue = quickSlots[selectedSlotIndex].Coin;
                quickSlots[selectedSlotIndex] = null;
            }
            GameManager.Instance.Add_Gold(itemValue);
            SpawnEffectParts(itemValue, "Coin");
            SoundManager.Instance?.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else if (ty == "all")
        {
            for (int i = 0; i < quickSlots.Length; i++)
            {
                if (quickSlots[i] != null && !string.IsNullOrEmpty(quickSlots[i].itemName))
                {
                    itemValue += quickSlots[i].Coin;
                    quickSlots[i] = null;
                }
            }
            GameManager.Instance?.Add_Gold(itemValue);
            SpawnEffectParts(itemValue * 2, "Coin");
            GameManager.Instance?.Add_Soul(itemValue * 2);
            SpawnEffectParts(itemValue, "Soul");
        }
    }
    private void SpawnEffectParts(int totalValue, string type)
    {
        int remainingValue = totalValue;

        while (remainingValue > 0)
        {
            int shardValue = Random.Range(7, 13);
            if (shardValue > remainingValue)
                shardValue = remainingValue;

            GameObject fx = Instantiate(Sale_Effect, transform.position, Quaternion.identity);
            fx.transform.SetParent(uiCanvas, false);
            MoneyEffect effect = fx.GetComponent<MoneyEffect>();
            effect.ty = type;

            remainingValue -= shardValue;
        }
    }

    public float GetTotalItemWeight()//��� �ִ� ��� �������� ����
    {
        float totalWeight = 0f;
        foreach (Item item in quickSlots)
        {
            if (item != null)
            {
                totalWeight += 0;//item.Weight;
            }
        }
        return totalWeight;
    }
   public void UpdateQuickSlotUI()
    {
        QuickSlotUI quickSlotUI = FindObjectOfType<QuickSlotUI>();
        if (quickSlotUI != null)
        {
            quickSlotUI.UpdateUI();
        }
    }

    //��� üũ �Լ�
    public int CheckEmptySlotsCount()
    {
        int emptyCount = 0;
        foreach(var quickSlot in quickSlots)
        {
            if(quickSlot == null)
                emptyCount++;
        }
        return emptyCount;
    }
}
