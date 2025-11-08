using UnityEngine;
using TMPro;


public class WeaponManager : MonoBehaviour
{
    ShopQuickSlot shopQuickSlot;

    [Header("Equipment Slots")]
    public GameObject[] equipmentSlot; // 장비 슬롯

    [Header("Selection")]
    public GameObject selectionIndicator; // 선택 표시 오브젝트 (이동하는 1개)
    public Transform[] slotPositions; // 슬롯 위치들 (6개: 장비 5개 + 다음버튼 1개)
    private int currentSelectedIndex = 0; // 0~5 (0~4: 장비, 5: 다음버튼)

    public ItemData[] weaponData; // 무기 데이터

    [Header("Equipped Weapons")]
    private int[] equippedSlots = new int[2] { -1, -1 }; // 장착된 장비 슬롯 인덱스 (0~4), -1은 빈칸
    private int currentEquipSlot = 0; // 현재 장착할 슬롯 (0 or 1)

    public bool canProcessInput = false; // 입력 처리 가능 플래그

    void Start()
    {
        shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
        UpdateSelectionUI();
    }

    void Update()
    {
        HandleEquipmentSelection();
    }
    void LateUpdate()
    {
        // LateUpdate에서 UI 업데이트
        UpdateSelectionUI();
    }
    void HandleEquipmentSelection()
    {
        // 입력 처리 불가능하면 무시
        if (!canProcessInput) return;

        // 좌우 이동
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedIndex--;
            if (currentSelectedIndex < 0) currentSelectedIndex = 5; // 0~5 순환
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSelectedIndex++;
            if (currentSelectedIndex > 5) currentSelectedIndex = 0; // 0~5 순환
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
        if (currentSelectedIndex >= 0 && currentSelectedIndex <= equipmentSlot.Length -1)
        {
            // 장비 장착
            EquipWeapon(currentSelectedIndex);
        }
        else if (currentSelectedIndex == equipmentSlot.Length)
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

    void ApplyEquipment(int equipSlot, int index)
    {
        if (index < 0 || index >= equipmentSlot.Length)
        {
            Debug.LogWarning($"잘못된 장비 인덱스: {index}");
            return;
        }

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
            Debug.LogWarning("모든 무기 슬롯이 가득 찼습니다.");
            return;
        }

        GameEvents.CallBuyWeapon();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }

        shopQuickSlot.weaponSlotsData[emptySlotIndex] = weaponData[index];
        GameManager.Instance.WeaponData[emptySlotIndex] = weaponData[index];

        Debug.Log($"{equipSlot}번 칸에 {weaponData[index].itemName} 장착 완료");
    }

    void RemoveEquipment(int equipSlot)
    {
        shopQuickSlot.weaponSlotsData[equipSlot] = null;
        GameManager.Instance.WeaponData[equipSlot] = null;
        Debug.Log($"{equipSlot}번 칸 장비 해제 완료");
    }

    GameObject GetEquipmentSlot(int slotIndex)
    {
        if (equipmentSlot.Length >= slotIndex)
        {
            return equipmentSlot[slotIndex];
        }
        else
        {
            return null;
        }
    }

    public int[] GetEquippedSlots()
    {
        return equippedSlots;
    }

    public bool IsSlotEquipped(int slotIndex)
    {
        return equippedSlots[0] == slotIndex || equippedSlots[1] == slotIndex;
    }
}