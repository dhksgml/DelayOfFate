using UnityEngine;

public class Player_Item_Use : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4개의 퀵슬롯
    public int selectedSlotIndex = 0; // 현재 선택된 슬롯
    public Item[] weaponSlots = new Item[2];
    public int selectedWeaponIndex = 0;
    public Transform dropPoint; // 아이템 드롭 위치
    public LayerMask itemLayer; // 아이템 레이어 설정
    public GameObject item_Prefab; // 아이템을 생성 할때 사용 할 빈 프리팹
    private const float requiredHoldTime = 1f;
    private ItemUsageManager itemUsageManager;
    private PlayerController playercontroller;
    private Animator animator;
    private Item chargingItem = null;
    private Transform uiCanvas; // 이펙트가 생성될 위치
    public GameObject Sale_Effect; // 이펙트 오브젝트

    // 소면귀용
    public bool isItemTouch = false;

    //public static bool isAnyItemBeingSold = false; // 전역 중복 방지 플래그
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
        HandleSlotSelection(); // 슬롯 변경 처리
        float weight = GetTotalItemWeight();
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedWeaponIndex >= 0 && selectedWeaponIndex < weaponSlots.Length)
            {
                Item selectedItem = weaponSlots[selectedWeaponIndex];
                if (selectedItem != null)
                {
                    if (selectedItem.Charging)
                    {
                        chargingItem = selectedItem;
                    }
                    else
                    {
                        UseItem(); // 사용                   
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.F)) // 버리기
        {
            //DropItem();
            TutorialEvents.OnItemDropped?.Invoke(quickSlots[selectedSlotIndex]);
            playercontroller.OnPickUpStart(false);
            HandleSellAction();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !playercontroller.isRecovering) // 줍기
        {
            //if (!isAnyItemBeingSold)
            //{
                // 주변에서 즉시 판매 가능한 아이템 중 첫 번째 하나만 찾기
                Collider2D nearestItemCollider = Physics2D.OverlapCircle(transform.position, 1f, itemLayer);
                if (nearestItemCollider != null)
                {
                    ItemObject itemObject = nearestItemCollider.GetComponent<ItemObject>();
                    if (itemObject != null && itemObject.itemData.Sell_immediately)
                    {
                        PickUpItem();
                        //isAnyItemBeingSold = true;
                        isItemTouch = true;
                        currentSellingItem = itemObject; // 새 변수, 현재 판매 중인 아이템 저장
                    }
                }
            //}
        }
    }

    //현재 슬롯이 비었는지 판단
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

        // 슬롯 선택 (1~4 키)
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlotIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlotIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlotIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlotIndex = 3;

        // 마우스 휠로 슬롯 변경
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) selectedSlotIndex = (selectedSlotIndex + 3) % 4;  // 역방향 스크롤 (0~3 범위로 돌아가도록)
        if (scroll < 0f) selectedSlotIndex = (selectedSlotIndex + 1) % 4;  // 0~3 범위로 돌아가도록

        // selectedSlotIndex가 0~3 범위 내로 유지되도록 보장
        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, 3);

        UpdateQuickSlotUI();
    }

    void TryUseItem(Item selectedItem)
    {
        if(!playercontroller.isRecovering)
        {
            if (selectedItem.id == 995) //족자의 경우 기력 대신 정신력 사용
            {
                if (!playercontroller.isRecovering && selectedItem.spendSPAmount < playercontroller.currentMp)
                {
                    playercontroller.SpendMp(Random.Range(selectedItem.spendSPAmount - 2, selectedItem.spendSPAmount + 2));
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
            Debug.Log("아이템 사용 쿨타임 중입니다.");
            return;
        }

        if (selectedWeaponIndex >= 0 && selectedWeaponIndex < weaponSlots.Length)
        {
            Item selectedItem = weaponSlots[selectedWeaponIndex];

            if (selectedItem != null && selectedItem.isUsable)
            {
                // 중복 아이템일 경우
                if (selectedItem.Count_Check)
                {
                    if (selectedItem.Count > 0)
                    {
                        if (selectedItem.id != 996)
                        {
                            selectedItem.Count--;

                            // 곗수가 0이 되면 슬롯 비우기
                            if (selectedItem.Count <= 0)
                            {
                                weaponSlots[selectedWeaponIndex] = null;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("아이템 곗수가 부족합니다.");
                        return;
                    }
                }

                // 아이템 사용 처리
                TryUseItem(selectedItem);

                // 쿨다운 적용
                playercontroller.Player_Usage_cu_cool_down = selectedItem.Usage_cool_down;
                playercontroller.SetUseItemCooltime(selectedItem.Usage_cool_down);

                // UI 갱신
                UpdateQuickSlotUI();
            }
        }
    }

    public void PickUpItem()//줍기
    {
        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, 1f, itemLayer);
        print("줍기 발동");
        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                print(itemObject);
                Item droppedItem = itemObject.itemData;
                Item slotItem = quickSlots[selectedSlotIndex];
                // 슬롯이 비어있는 경우
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
                // 슬롯에 다른 아이템이 있는 경우
                else
                {
                    // 먼저 다른 슬롯 중 빈 슬롯이 있는지 확인
                    bool placedInEmptySlot = false;
                    for (int i = 0; i < quickSlots.Length; i++)
                    {
                        if (quickSlots[i] == null || string.IsNullOrEmpty(quickSlots[i].itemName))
                        {
                            // 빈 슬롯 발견 → 그 슬롯에 아이템 넣기
                            TutorialEvents.OnItemPickedUp?.Invoke(droppedItem);
                            quickSlots[i] = droppedItem;

                            if (SoundManager.Instance != null)
                                SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_pickup"));

                            Destroy(itemObject.gameObject);
                            UpdateQuickSlotUI();
                            GameEvents.CallPickupItem();

                            placedInEmptySlot = true;
                            break; // 한 슬롯에만 넣고 종료
                        }
                    }

                    // 모든 슬롯이 꽉 차있다면 기존 로직대로 현재 슬롯과 교체
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
        // 1. 바닥 아이템 있는지 검사
        if (CheckCurrentSlotEmpty())
        {
            // 바닥 아이템 판매
            RemoveItem();
            return;
        }
        // 2. 현재 슬롯에 아이템이 있는지 확인
        SellCurrentSlotItem();
        return;
        
        // 3. 아무 것도 없으면 아무 일도 안 함
    }

    // 슬롯 아이템 판매
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

            // 빈 아이템 프리팹을 기반으로 새로운 아이템 생성
            GameObject newItem = Instantiate(item_Prefab, dropPoint.position, Quaternion.identity);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_drop"));
            // 새로 생성된 아이템에 ItemObject 스크립트 추가 후 데이터 복사
            ItemObject newItemComponent = newItem.GetComponent<ItemObject>();
            if (newItemComponent != null)
            {
                newItemComponent.itemData = selectedItem.Clone(); // 객체 복사
                newItemComponent.itemData = selectedItem; // 객체 데이터 복사후 떨구기
                newItemComponent.itemData.Drop_item = true; // *떨어트린 적 있는 아이템 으로 변경*
            }

            SpriteRenderer spriteRenderer = newItem.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newItemComponent.itemData.InGameSprite;
            }

            Debug.Log($"버린 아이템: {newItemComponent.itemData.itemName}, 금액: {newItemComponent.itemData.Coin}, 무게: {newItemComponent.itemData.Weight}");
            // 퀵슬롯에서 해당 아이템 제거
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
                DropItem(); // 기존 메서드 사용
            }
        }
    }
    void RemoveItem() // 즉시 판매
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

    public float GetTotalItemWeight()//들고 있는 모든 아이템의 무게
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

    //빈손 체크 함수
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
