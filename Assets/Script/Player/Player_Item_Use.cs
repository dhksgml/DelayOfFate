using UnityEngine;

public class Player_Item_Use : MonoBehaviour
{
    public Item[] quickSlots = new Item[4]; // 4개의 퀵슬롯
    public int selectedSlotIndex = 0; // 현재 선택된 슬롯
    public Transform dropPoint; // 아이템 드롭 위치
    public LayerMask itemLayer; // 아이템 레이어 설정
    public GameObject item_Prefab; // 아이템을 생성 할때 사용 할 빈 프리팹
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
        HandleSlotSelection(); // 슬롯 변경 처리
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
                        UseItem(selectedItem.itemName); // 사용
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0) && isCharging)
        {
            chargingTimer += Time.deltaTime;
            if (chargingTimer >= requiredHoldTime)
            {
                UseItem(chargingItem.itemName); // 차징 완료 시 사용
                isCharging = false;
                chargingItem = null;
            }
        }
        else if (Input.GetMouseButtonUp(0) && isCharging)
        {
            // 마우스 떼면 차징 취소
            isCharging = false;
            chargingTimer = 0f;
            chargingItem = null;
        }
        else if (Input.GetKeyDown(KeyCode.F) && !CheckCurrentSlotEmpty()) // 버리기
        {
            //DropItem();
            playercontroller.OnPickUpStart(false);
        }
        else if (Input.GetKey(KeyCode.E)) // 꾹 눌러서 판매, 줍기
        {
            Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, 1f, itemLayer);
            
            foreach (Collider2D collider in itemColliders)
            {
                ItemObject itemObject = collider.GetComponent<ItemObject>();
                if (itemObject.itemData.Sell_immediately)// 즉시 판매 가능한 아이템만 가능
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
                        isHolding = false; // 한 번 실행 후 다시 대기
                        holdTime = 0f;
                    }
                }
            }

        }
        else if (Input.GetKeyUp(KeyCode.E)) // 키 떼면
        {
            if (holdTime <= 0.2f) // 짧게 눌렀다면 줍는걸로 인지
            {
                if (playercontroller.isPickUpableItem)
                {
                    playercontroller.OnPickUpStart(true);
                }
            }
            isHolding = false; // 그게 아니면 초기화
            holdTime = 0f;
        }
    }

    //현재 슬롯이 비었는지 판단
    bool CheckCurrentSlotEmpty()
    {
        return quickSlots[selectedSlotIndex] == null;
    }

    void HandleSlotSelection()
    {
        // 슬롯 선택 (1~4 키)
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlotIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlotIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlotIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlotIndex = 3;

        // 마우스 휠로 슬롯 변경
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) selectedSlotIndex = (selectedSlotIndex + 1) % 4;  // 0~3 범위로 돌아가도록
        if (scroll < 0f) selectedSlotIndex = (selectedSlotIndex + 3) % 4;  // 역방향 스크롤 (0~3 범위로 돌아가도록)

        // selectedSlotIndex가 0~3 범위 내로 유지되도록 보장
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
            Debug.Log("아이템 사용 쿨타임 중입니다.");
            return;
        }

        if (selectedSlotIndex >= 0 && selectedSlotIndex < quickSlots.Length)
        {
            Item selectedItem = quickSlots[selectedSlotIndex];

            if (selectedItem != null && selectedItem.isUsable)
            {
                // 중복 아이템일 경우
                if (selectedItem.Count_Check)
                {
                    if (selectedItem.Count > 0)
                    {
                        selectedItem.Count--;

                        // 곗수가 0이 되면 슬롯 비우기
                        if (selectedItem.Count <= 0)
                        {
                            quickSlots[selectedSlotIndex] = null;
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
                //itemUsageManager.UseItem(selectedItem.itemName);

                // 쿨다운 적용
                playercontroller.Player_Usage_cu_cool_down = selectedItem.Usage_cool_down;
                //switch (selectedItem.itemName) // 무기 사용시 기력 소모
                //{
                //    case "환도":
                //        playercontroller.SpendSp(10);
                //        break;
                //    case "방망이":
                //        playercontroller.SpendSp(20);
                //        break;
                //    case "부적":
                //        playercontroller.SpendSp(5);
                //        break;
                //    case "족자":
                //        playercontroller.SpendSp(10);
                //        break;
                //    case "호리병":
                //        playercontroller.SpendSp(5);
                //        break;
                //}

                // UI 갱신
                UpdateQuickSlotUI();
            }
        }
    }

    public void PickUpItem()//줍기
    {
        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, 1f, itemLayer);

        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                Item droppedItem = itemObject.itemData;
                Item slotItem = quickSlots[selectedSlotIndex];
                // 슬롯이 비어있는 경우
                if (slotItem == null || string.IsNullOrEmpty(slotItem.itemName))
                {
                    quickSlots[selectedSlotIndex] = droppedItem;
                    Destroy(itemObject.gameObject);
                    UpdateQuickSlotUI();
                }
                // 슬롯에 이미 아이템이 있고 같은 아이템이며, 곗수 합산 가능한 경우
                else if (slotItem.itemName == droppedItem.itemName && slotItem.Count_Check)
                {
                    slotItem.Count += droppedItem.Count;
                    Destroy(itemObject.gameObject);
                    UpdateQuickSlotUI();
                }
                // 슬롯에 다른 아이템이 있는 경우 - 기존 아이템 드롭 후 교체
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

            // 빈 아이템 프리팹을 기반으로 새로운 아이템 생성
            GameObject newItem = Instantiate(item_Prefab, dropPoint.position, Quaternion.identity);

            // 새로 생성된 아이템에 ItemObject 스크립트 추가 후 데이터 복사
            ItemObject newItemComponent = newItem.GetComponent<ItemObject>();
            if (newItemComponent != null)
            {
                newItemComponent.itemData = selectedItem.Clone(); // 객체 복사
                newItemComponent.itemData = selectedItem; // 객체 데이터 복사후 떨구기
                /*newItemComponent.itemData.Count_Check = selectedItem.Count_Check; // **기존 금액 유지**
                newItemComponent.itemData.Count = selectedItem.Count; // **기존 곗수 유지**
                newItemComponent.itemData.Coin = selectedItem.Coin; // **기존 금액 유지**
                newItemComponent.itemData.Weight = selectedItem.Weight; // **기존 무게 유지***/
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
                int itemValue = itemObject.itemData.Coin; // 아이템의 가치 가져오기
                //player.coin += itemValue; // 플레이어 코인 증가 나중에 매서드로 분리
                GameManager.Instance.Add_Soul(itemValue);
                Destroy(itemObject.gameObject); // 아이템 오브젝트 삭제
            }
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
