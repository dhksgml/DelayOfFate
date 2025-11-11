using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    ShopQuickSlot shopQuickSlot;

    [Header("Equipment Slots")]
    public GameObject[] equipmentSlot; // 장비 슬롯

    [Header("Selection")]
    public GameObject selectionIndicator; // 선택 표시 오브젝트 (이동하는 1개)
    public Transform[] slotPositions; // 슬롯 위치들
    private int currentSelectedIndex = 0;

    public ItemData[] weaponData; // 무기 데이터

    [Header("Equipped Weapons")]
    private int[] equippedSlots = new int[2] { -1, -1 }; // 장착된 장비 슬롯 인덱스, -1은 빈칸
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
        UpdateSelectionUI();
    }

    void HandleEquipmentSelection()
    {
        PassiveItemUI passiveUI = FindObjectOfType<PassiveItemUI>();
        PassiveItemManager passiveItemManager = FindObjectOfType<PassiveItemManager>();

        if (!canProcessInput) return;

        int previousIndex = currentSelectedIndex;

        // 현재 줄과 해당 줄의 시작/끝 인덱스 계산
        int currentRow = GetRowFromIndex(currentSelectedIndex);
        int rowStart = GetRowStartIndex(currentRow);
        int rowEnd = GetRowEndIndex(currentRow);

        // 위로 이동
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentRow == 0) // 1줄에서 위로
            {
                // 3줄로 이동 (같은 열 유지, 3줄이 더 길면 끝으로)
                int columnInRow = currentSelectedIndex - rowStart;
                currentSelectedIndex = GetRowStartIndex(2) + columnInRow;
                if (currentSelectedIndex > GetRowEndIndex(2)) currentSelectedIndex = GetRowEndIndex(2);
            }
            else if (currentRow == 1) // 2줄에서 위로
            {
                // 1줄로 이동
                int columnInRow = currentSelectedIndex - rowStart;
                currentSelectedIndex = GetRowStartIndex(0) + columnInRow;
            }
            else if (currentRow == 2) // 3줄에서 위로
            {
                // 2줄로 이동 (3줄이 1개라 2줄 끝으로)
                int columnInRow = currentSelectedIndex - rowStart;
                currentSelectedIndex = GetRowStartIndex(1) + columnInRow;
                if (currentSelectedIndex > GetRowEndIndex(1)) currentSelectedIndex = GetRowEndIndex(1);
            }
            else if (currentRow == 3) // 상점 버튼에서 위로
            {
                // 3줄로 이동
                currentSelectedIndex = GetRowStartIndex(2);
            }
        }

        // 아래로 이동
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentRow == 0) // 1줄에서 아래로
            {
                // 2줄로 이동
                int columnInRow = currentSelectedIndex - rowStart;
                currentSelectedIndex = GetRowStartIndex(1) + columnInRow;
            }
            else if (currentRow == 1) // 2줄에서 아래로
            {
                // 3줄로 이동
                int columnInRow = currentSelectedIndex - rowStart;
                currentSelectedIndex = GetRowStartIndex(2) + columnInRow;
                if (currentSelectedIndex > GetRowEndIndex(2)) currentSelectedIndex = GetRowEndIndex(2);
            }
            else if (currentRow == 2) // 3줄에서 아래로
            {
                // 상점 버튼으로 이동
                currentSelectedIndex = equipmentSlot.Length;
            }
            else if (currentRow == 3) // 상점 버튼에서 아래로
            {
                // 1줄로 이동
                currentSelectedIndex = 0;
            }
        }

        // 왼쪽 이동
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentRow == 3) // 상점 버튼에서는 이동 불가
            {
                return;
            }

            currentSelectedIndex--;
            if (currentSelectedIndex < rowStart) currentSelectedIndex = rowEnd;
        }

        // 오른쪽 이동
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentRow == 3) // 상점 버튼에서는 이동 불가
            {
                return;
            }

            currentSelectedIndex++;
            if (currentSelectedIndex > rowEnd) currentSelectedIndex = rowStart;
        }

        // 설명 업데이트
        if (currentSelectedIndex < equipmentSlot.Length)
        {
            passiveUI.Show(passiveItemManager.GetPassiveName(8, currentSelectedIndex + 1),
                passiveItemManager.GetPassiveDescription(8, currentSelectedIndex + 1),
                passiveItemManager.GetPassiveEmdrmq(8, currentSelectedIndex + 1));
        }
        else if (currentSelectedIndex == equipmentSlot.Length)
        {
            passiveUI.Show(passiveItemManager.GetPassiveName(10, 1),
                passiveItemManager.GetPassiveDescription(10, 1),
                passiveItemManager.GetPassiveEmdrmq(10, 1));
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

    // 인덱스로 현재 줄 구하기
    // 0~1: 0줄, 2~3: 1줄, 4: 2줄, equipmentSlot.Length: 3줄(상점버튼)
    int GetRowFromIndex(int index)
    {
        if (index >= 0 && index <= 1) return 0; // 1줄
        if (index >= 2 && index <= 3) return 1; // 2줄
        if (index == 4) return 2; // 3줄
        if (index == equipmentSlot.Length) return 3; // 상점버튼
        return 0;
    }

    // 줄의 시작 인덱스
    int GetRowStartIndex(int row)
    {
        switch (row)
        {
            case 0: return 0;  // 1줄 시작
            case 1: return 2;  // 2줄 시작
            case 2: return 4;  // 3줄 시작
            case 3: return equipmentSlot.Length; // 상점버튼
            default: return 0;
        }
    }

    // 줄의 끝 인덱스
    int GetRowEndIndex(int row)
    {
        switch (row)
        {
            case 0: return 1;  // 1줄 끝 (2개)
            case 1: return 3;  // 2줄 끝 (2개)
            case 2: return 4;  // 3줄 끝 (1개)
            case 3: return equipmentSlot.Length; // 상점버튼
            default: return 0;
        }
    }

    void UpdateSelectionUI()
    {
        if (selectionIndicator != null && slotPositions != null && slotPositions.Length > currentSelectedIndex)
        {
            selectionIndicator.transform.position = slotPositions[currentSelectedIndex].position;
        }
    }

    void ConfirmSelection()
    {
        if (currentSelectedIndex >= 0 && currentSelectedIndex <= equipmentSlot.Length - 1)
        {
            EquipWeapon(currentSelectedIndex);
        }
        else if (currentSelectedIndex == equipmentSlot.Length)
        {
            OnNextButtonPressed();
        }
    }

    void EquipWeapon(int slotIndex)
    {
        if (equippedSlots[0] == slotIndex || equippedSlots[1] == slotIndex)
        {
            Debug.Log($"장비 슬롯 {slotIndex + 1}은 이미 장착되어 있습니다.");
            return;
        }

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
            return;
        }

        equippedSlots[currentEquipSlot] = slotIndex;

        ApplyEquipment(currentEquipSlot, slotIndex);
    }

    void UnequipWeapon()
    {
        if (equippedSlots[1] != -1)
        {
            equippedSlots[1] = -1;
            RemoveEquipment(1);
        }
        else if (equippedSlots[0] != -1)
        {
            equippedSlots[0] = -1;
            RemoveEquipment(0);
        }
    }

    void OnNextButtonPressed()
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
