using UnityEngine;

public class Player_Item_Use : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4���� ������
    public int selectedSlotIndex = 0; // ���� ���õ� ����
    public Transform dropPoint; // ������ ��� ��ġ
    public LayerMask itemLayer; // ������ ���̾� ����
    public GameObject item_Prefab; // �������� ���� �Ҷ� ��� �� �� ������
    public float holdTime = 0f;
    private bool isHolding = false;
    private const float requiredHoldTime = 1f;
    private ItemUsageManager itemUsageManager;
    private PlayerController playercontroller;
    private float chargingTimer = 0f;
    private bool isCharging = false;
    private Item chargingItem = null;

    void Start()
    {
        itemUsageManager = GetComponent<ItemUsageManager>();
        playercontroller = GetComponent<PlayerController>();
        
    }
    void Update()
    {
        //print(quickSlots[selectedSlotIndex]);
        HandleSlotSelection(); // ���� ���� ó��
        float weight = GetTotalItemWeight();
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedSlotIndex >= 0 && selectedSlotIndex < quickSlots.Length)
            {
                Item selectedItem = quickSlots[selectedSlotIndex];
                if (selectedItem != null)
                {
                    if (selectedItem.Charging)
                    {
                        isCharging = true;
                        chargingTimer = 0f;
                        chargingItem = selectedItem;
                    }
                    else
                    {
                        UseItem(selectedItem.itemName); // ���
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0) && isCharging)
        {
            chargingTimer += Time.deltaTime;
            if (chargingTimer >= requiredHoldTime)
            {
                UseItem(chargingItem.itemName); // ��¡ �Ϸ� �� ���
                isCharging = false;
                chargingItem = null;
            }
        }
        else if (Input.GetMouseButtonUp(0) && isCharging)
        {
            // ���콺 ���� ��¡ ���
            isCharging = false;
            chargingTimer = 0f;
            chargingItem = null;
        }
        else if (Input.GetKeyDown(KeyCode.F) && !CheckCurrentSlotEmpty()) // ������
        {
            //DropItem();
            playercontroller.OnPickUpStart(false);
        }
        else if (Input.GetKey(KeyCode.E)) // �� ������ �Ǹ�, �ݱ�
        {
            Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, 1f, itemLayer);
            
            foreach (Collider2D collider in itemColliders)
            {
                ItemObject itemObject = collider.GetComponent<ItemObject>();
                if (itemObject.itemData.Sell_immediately)// ��� �Ǹ� ������ �����۸� ����
                {
                    if (!isHolding)
                    {
                        isHolding = true;
                        holdTime = 0f;
                    }
                    holdTime += Time.deltaTime;
                    if (holdTime >= requiredHoldTime)
                    {
                        RemoveItem();
                        isHolding = false; // �� �� ���� �� �ٽ� ���
                        holdTime = 0f;
                    }
                }
            }

        }
        else if (Input.GetKeyUp(KeyCode.E)) // Ű ����
        {
            if (holdTime <= 0.2f) // ª�� �����ٸ� �ݴ°ɷ� ����
            {
                if (playercontroller.isPickUpableItem)
                {
                    playercontroller.OnPickUpStart(true);
                }
            }
            isHolding = false; // �װ� �ƴϸ� �ʱ�ȭ
            holdTime = 0f;
        }
    }

    //���� ������ ������� �Ǵ�
    bool CheckCurrentSlotEmpty()
    {
        return quickSlots[selectedSlotIndex] == null;
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

        UpdateQuickSlotUI();
    }

    void TryUseItem(Item selectedItem)
    {
        if(selectedItem.spendSPAmount < playercontroller.currentSp)
        {
            playercontroller.SpendSp(selectedItem.spendSPAmount);
            itemUsageManager.UseItem(selectedItem.itemName);
        }
    }

    void UseItem(string itemName)
    {
        if (playercontroller.Player_Usage_cu_cool_down > 0f)
        {
            Debug.Log("������ ��� ��Ÿ�� ���Դϴ�.");
            return;
        }

        if (selectedSlotIndex >= 0 && selectedSlotIndex < quickSlots.Length)
        {
            Item selectedItem = quickSlots[selectedSlotIndex];

            if (selectedItem != null && selectedItem.isUsable)
            {
                // �ߺ� �������� ���
                if (selectedItem.Count_Check)
                {
                    if (selectedItem.Count > 0)
                    {
                        selectedItem.Count--;

                        // ����� 0�� �Ǹ� ���� ����
                        if (selectedItem.Count <= 0)
                        {
                            quickSlots[selectedSlotIndex] = null;
                        }
                    }
                    else
                    {
                        Debug.Log("������ ����� �����մϴ�.");
                        return;
                    }
                }

                // ������ ��� ó��
                TryUseItem(selectedItem);
                //itemUsageManager.UseItem(selectedItem.itemName);

                // ��ٿ� ����
                playercontroller.Player_Usage_cu_cool_down = selectedItem.Usage_cool_down;
                //switch (selectedItem.itemName) // ���� ���� ��� �Ҹ�
                //{
                //    case "ȯ��":
                //        playercontroller.SpendSp(10);
                //        break;
                //    case "�����":
                //        playercontroller.SpendSp(20);
                //        break;
                //    case "����":
                //        playercontroller.SpendSp(5);
                //        break;
                //    case "����":
                //        playercontroller.SpendSp(10);
                //        break;
                //    case "ȣ����":
                //        playercontroller.SpendSp(5);
                //        break;
                //}

                // UI ����
                UpdateQuickSlotUI();
            }
        }
    }

    public void PickUpItem()//�ݱ�
    {
        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, 1f, itemLayer);

        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                Item droppedItem = itemObject.itemData;
                Item slotItem = quickSlots[selectedSlotIndex];
                // ������ ����ִ� ���
                if (slotItem == null || string.IsNullOrEmpty(slotItem.itemName))
                {
                    quickSlots[selectedSlotIndex] = droppedItem;
                    Destroy(itemObject.gameObject);
                    UpdateQuickSlotUI();
                }
                // ���Կ� �̹� �������� �ְ� ���� �������̸�, ��� �ջ� ������ ���
                else if (slotItem.itemName == droppedItem.itemName && slotItem.Count_Check)
                {
                    slotItem.Count += droppedItem.Count;
                    Destroy(itemObject.gameObject);
                    UpdateQuickSlotUI();
                }
                // ���Կ� �ٸ� �������� �ִ� ��� - ���� ������ ��� �� ��ü
                else
                {
                    DropItem();
                    quickSlots[selectedSlotIndex] = droppedItem;
                    Destroy(itemObject.gameObject);
                    UpdateQuickSlotUI();
                }
            }
        }
    }


    public void DropItem()
    {
        if (quickSlots[selectedSlotIndex] != null && !string.IsNullOrEmpty(quickSlots[selectedSlotIndex].itemName))
        {
            Item selectedItem = quickSlots[selectedSlotIndex];

            // �� ������ �������� ������� ���ο� ������ ����
            GameObject newItem = Instantiate(item_Prefab, dropPoint.position, Quaternion.identity);

            // ���� ������ �����ۿ� ItemObject ��ũ��Ʈ �߰� �� ������ ����
            ItemObject newItemComponent = newItem.GetComponent<ItemObject>();
            if (newItemComponent != null)
            {
                newItemComponent.itemData = selectedItem.Clone(); // ��ü ����
                newItemComponent.itemData = selectedItem; // ��ü ������ ������ ������
                /*newItemComponent.itemData.Count_Check = selectedItem.Count_Check; // **���� �ݾ� ����**
                newItemComponent.itemData.Count = selectedItem.Count; // **���� ��� ����**
                newItemComponent.itemData.Coin = selectedItem.Coin; // **���� �ݾ� ����**
                newItemComponent.itemData.Weight = selectedItem.Weight; // **���� ���� ����***/
                newItemComponent.itemData.Drop_item = true; // *����Ʈ�� �� �ִ� ������ ���� ����*
            }

            SpriteRenderer spriteRenderer = newItem.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newItemComponent.itemData.InGameSprite;
            }

            Debug.Log($"���� ������: {newItemComponent.itemData.itemName}, �ݾ�: {newItemComponent.itemData.Coin}, ����: {newItemComponent.itemData.Weight}");

            // �����Կ��� �ش� ������ ����
            quickSlots[selectedSlotIndex] = null;
            UpdateQuickSlotUI();
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
                int itemValue = itemObject.itemData.Coin; // �������� ��ġ ��������
                //player.coin += itemValue; // �÷��̾� ���� ���� ���߿� �ż���� �и�
                GameManager.Instance.Add_Soul(itemValue);
                Destroy(itemObject.gameObject); // ������ ������Ʈ ����
            }
        }
    }
    public float GetTotalItemWeight()//��� �ִ� ��� �������� ����
    {
        float totalWeight = 0f;
        foreach (Item item in quickSlots)
        {
            if (item != null)
            {
                totalWeight += item.Weight;
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
