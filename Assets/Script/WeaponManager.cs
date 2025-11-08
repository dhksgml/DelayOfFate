using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("Equipment Slots")]
    public GameObject equipmentSlot1; // 장비 슬롯 1
    public GameObject equipmentSlot2; // 장비 슬롯 2
    public GameObject equipmentSlot3; // 장비 슬롯 3
    public GameObject equipmentSlot4; // 장비 슬롯 4
    public GameObject equipmentSlot5; // 장비 슬롯 5

    [Header("Selection")]
    public GameObject selectionIndicator; // 선택 표시 오브젝트 (이동하는 1개)
    public Transform[] slotPositions; // 슬롯 위치들 (6개: 장비 5개 + 다음버튼 1개)
    private int currentSelectedIndex = 0; // 0~5 (0~4: 장비, 5: 다음버튼)

    public ItemData[] weaponData; // 무기 데이터

    [Header("Equipped Weapons")]
    private int[] equippedSlots = new int[2] { -1, -1 }; // 장착된 장비 슬롯 인덱스 (0~4), -1은 빈칸
    private int currentEquipSlot = 0; // 현재 장착할 슬롯 (0 or 1)

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateSelectionUI();
    }

    void Update()
    {
        HandleEquipmentSelection();
    }

    /*public void BuyWeapon(int index) // 무기 구매
    {
        if (index < 0 || index >= 5) return;

        int price = weaponPrices[index];

        if (PassiveItemManager.Instance != null)
            price = 0;

        // 내부에서 바로 퀵슬롯 참조
        ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
        if (shopQuickSlot == null) return;

        // 빈 슬롯 찾기
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
            Debug.Log("퀵슬롯이 모두 찼습니다.");
            Shop shop = FindObjectOfType<Shop>();
            if (shop != null)
                shop.speech_bubble_on("무기충분");
            return;
        }
        GameManager.Instance.Sub_Gold(price);

        weaponSlots[index].text = "구매 완료";
        GameEvents.CallBuyWeapon();
        speech_bubble_on("구매");
        Button btn = weaponSlots[index].GetComponentInParent<Button>();
        if (btn != null) btn.interactable = false;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));

        shopQuickSlot.weaponSlotsData[emptySlotIndex] = weaponData[index];
        GameManager.Instance.WeaponData[emptySlotIndex] = shopQuickSlot.weaponSlotsData[emptySlotIndex];
        OnItemHover(emptySlotIndex, weaponData[index]);
    }*/
    // ============================================
    // TODO: 장비 선택 입력 처리
    // - 좌우 방향키: 선택 이동 (0~5)
    // - Z키: 현재 선택한 장비/버튼 확정
    // - X키: 장착 해제 (1번 슬롯부터)
    // ============================================
    void HandleEquipmentSelection()
    {
        // 좌우 이동
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedIndex--;
            if (currentSelectedIndex < 0) currentSelectedIndex = 5; // 0~5 순환
            UpdateSelectionUI();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSelectedIndex++;
            if (currentSelectedIndex > 5) currentSelectedIndex = 0; // 0~5 순환
            UpdateSelectionUI();
        }

        // Z키: 확정
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ConfirmSelection();
        }

        // X키: 장착 해제
        if (Input.GetKeyDown(KeyCode.X))
        {
            UnequipWeapon();
        }
    }

    // ============================================
    // TODO: 선택 UI 업데이트 (선택 표시 오브젝트 이동)
    // ============================================
    void UpdateSelectionUI()
    {
        if (selectionIndicator != null && slotPositions != null && slotPositions.Length > currentSelectedIndex)
        {
            selectionIndicator.transform.position = slotPositions[currentSelectedIndex].position;
        }
    }

    // ============================================
    // TODO: 선택 확정 처리
    // - 0~4: 장비 장착
    // - 5: 다음 버튼 (씬 전환 등)
    // ============================================
    void ConfirmSelection()
    {
        if (currentSelectedIndex >= 0 && currentSelectedIndex <= 4)
        {
            // 장비 장착
            EquipWeapon(currentSelectedIndex);
        }
        else if (currentSelectedIndex == 5)
        {
            // 다음 버튼
            OnNextButtonPressed();
        }
    }

    // ============================================
    // TODO: 장비 장착 메서드
    // - 이미 장착된 장비는 다시 장착 불가
    // - 0번 슬롯 → 1번 슬롯 순서로 장착
    // ============================================
    void EquipWeapon(int slotIndex)
    {
        // 이미 장착된 장비인지 확인
        if (equippedSlots[0] == slotIndex || equippedSlots[1] == slotIndex)
        {
            Debug.Log($"장비 슬롯 {slotIndex + 1}은 이미 장착되어 있습니다.");
            return;
        }

        // 현재 장착할 슬롯 결정
        if (equippedSlots[0] == -1)
        {
            currentEquipSlot = 0;
        }
        else if (equippedSlots[1] == -1)
        {
            currentEquipSlot = 1;
        }
        else
        {
            Debug.Log("모든 장비 슬롯이 이미 장착되어 있습니다.");
            return;
        }

        // 장비 장착
        equippedSlots[currentEquipSlot] = slotIndex;
        Debug.Log($"장비 슬롯 {slotIndex + 1}을(를) {currentEquipSlot}번 칸에 장착했습니다.");

        // TODO: 실제 장비 장착 로직 (무기 데이터 적용 등)
        ApplyEquipment(currentEquipSlot, slotIndex);
    }

    // ============================================
    // TODO: 장비 장착 해제 메서드
    // - 1번 슬롯부터 해제 (equippedSlots[1] → equippedSlots[0])
    // ============================================
    void UnequipWeapon()
    {
        // 1번 슬롯부터 해제
        if (equippedSlots[1] != -1)
        {
            Debug.Log($"장비 슬롯 {equippedSlots[1] + 1}을(를) 1번 칸에서 해제했습니다.");
            equippedSlots[1] = -1;

            // TODO: 실제 장비 해제 로직
            RemoveEquipment(1);
        }
        else if (equippedSlots[0] != -1)
        {
            Debug.Log($"장비 슬롯 {equippedSlots[0] + 1}을(를) 0번 칸에서 해제했습니다.");
            equippedSlots[0] = -1;

            // TODO: 실제 장비 해제 로직
            RemoveEquipment(0);
        }
        else
        {
            Debug.Log("장착된 장비가 없습니다.");
        }
    }

    void OnNextButtonPressed() //상점으로 이동
    {
        FindObjectOfType<Stage_Manager>().Shop_ch();
    }

    // ============================================
    // TODO: 실제 장비 적용 메서드
    // - equipSlot: 0번 또는 1번 칸
    // - slotIndex: 장비 슬롯 인덱스 (0~4)
    // ============================================
    void ApplyEquipment(int equipSlot, int slotIndex)
    {
        GameObject selectedEquipment = GetEquipmentSlot(slotIndex);
        if (selectedEquipment == null) return;

        // TODO: 실제 장비 데이터를 플레이어에게 적용
        // 예: PlayerWeaponManager.Instance.EquipWeapon(equipSlot, selectedEquipment.GetWeaponData());

        Debug.Log($"{equipSlot}번 칸에 {selectedEquipment.name} 장착 완료");
    }

    // ============================================
    // TODO: 실제 장비 제거 메서드
    // - equipSlot: 0번 또는 1번 칸
    // ============================================
    void RemoveEquipment(int equipSlot)
    {
        // TODO: 실제 장비 데이터를 플레이어에게서 제거
        // 예: PlayerWeaponManager.Instance.UnequipWeapon(equipSlot);

        Debug.Log($"{equipSlot}번 칸 장비 해제 완료");
    }

    // ============================================
    // Helper: 슬롯 인덱스로 Equipment_System 가져오기
    // ============================================
    GameObject GetEquipmentSlot(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0: return equipmentSlot1;
            case 1: return equipmentSlot2;
            case 2: return equipmentSlot3;
            case 3: return equipmentSlot4;
            case 4: return equipmentSlot5;
            default: return null;
        }
    }

    // ============================================
    // 외부 접근용: 현재 장착된 장비 확인
    // ============================================
    public int[] GetEquippedSlots()
    {
        return equippedSlots;
    }

    public bool IsSlotEquipped(int slotIndex)
    {
        return equippedSlots[0] == slotIndex || equippedSlots[1] == slotIndex;
    }
}