using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Mission Slots")]
    public Mission_System missionSlot1;
    public Mission_System missionSlot2;
    public Mission_System missionSlot3;

    [Header("Selection")]
    public GameObject selectionIndicator; // 선택 표시 오브젝트 (이동하는 1개)
    public Transform[] missionPositions; // 미션 버튼 위치들 (3개)
    private int currentSelectedIndex = 0; // 0, 1, 2

    [Header("InGame Mission UI")]
    public TMP_Text inGameMissionText; // 인게임 미션 표시 텍스트
    public GameObject missionUIPanel; // 미션 UI 패널 (on/off 용)

    // 현재 진행중인 미션
    private Mission_System.MissionData activeMission;
    private string selectedPassiveId = ""; // 미션에서 선택한 혼령강화 ID
    private bool isMissionActive = false; // 미션 진행 중 (인게임에서)
    private bool isMissionSelected = false; // 미션 선택됨 (상점에서)
    private bool isMissionCompleted = false;

    public bool Mission_ok = false; //미션을 수락 가능한 상태인지 체크

    // 미션 진행도 추적
    private int killCount = 0;
    private int interactCount = 0;
    private int recoverCount = 0;
    private int lightedAreaCount = 0;
    private int totalAreaCount = 0;
    private int sellCount = 0;
    private float missionStartTime = 0f;
    private int weaponsUsedCount = 0;
    private string lastUsedWeapon = "";

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
    
    public void Mission_start() // 시작 신호
    {
        if (Mission_ok)
        {
            // 미션 슬롯 초기화
            if (missionSlot1 != null) missionSlot1.GenerateRandomMission();
            if (missionSlot2 != null) missionSlot2.GenerateRandomMission();
            if (missionSlot3 != null) missionSlot3.GenerateRandomMission();
            UpdateSelectionUI();
            UpdateMissionInfo(); // 설명 업데이트
        }
    }

    void Update()
    {
        if (Mission_ok)
        {
            // 상점 씬에서만 미션 선택 가능
            if (IsInShopScene())
            {
                HandleMissionSelection();
            }
        }

        // 항상 미션 UI 업데이트 (상점, 인게임 모두)
        UpdateInGameMissionUI();

        // 미션 진행 중 시간 체크
        if (isMissionActive && activeMission != null)
        {
            CheckTimeLimitMission();
        }
    }
    // 상점 씬인지 확인
    bool IsInShopScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return currentScene == "Stage_Scene";
    }

    // 인게임 씬인지 확인
    bool IsInGameScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return currentScene == "InGame_Scenes";
    }

    // 미션 선택 입력 처리
    void HandleMissionSelection()
    {
        // 좌우 이동
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedIndex--;
            if (currentSelectedIndex < 0) currentSelectedIndex = 2;
            UpdateSelectionUI();
            UpdateMissionInfo(); // 설명 업데이트
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSelectedIndex++;
            if (currentSelectedIndex > 2) currentSelectedIndex = 0;
            UpdateSelectionUI();
            UpdateMissionInfo(); // 설명 업데이트
        }

        // Z, X, C 키로 각 미션 직접 선택
        if ((Input.GetKeyDown(KeyCode.Z)/* || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)*/) && !isMissionActive)
        {
            SelectMission(currentSelectedIndex);
        }
    }
    void UpdateMissionInfo()
    {
        Mission_System selectedSlot = null;
        switch (currentSelectedIndex)
        {
            case 0: selectedSlot = missionSlot1; break;
            case 1: selectedSlot = missionSlot2; break;
            case 2: selectedSlot = missionSlot3; break;
        }
        if (selectedSlot == null) return;

        var missionData = selectedSlot.GetCurrentMission();
        PassiveItemUI passiveUI = FindObjectOfType<PassiveItemUI>();
        if (passiveUI == null) return;

        switch (missionData.rewardType)
        {
            case Mission_System.RewardType.Soul:
                passiveUI.Show("혼", "대부분 악귀를 처치해 얻음\n딸의 약값으로 자주 거래됨", 7);
                break;

            case Mission_System.RewardType.Money:
                passiveUI.Show("냥", "귀한 물건을 판매해 얻음\n상점에서 혼령강화을 구매할 때 자주 거래됨", 7);
                break;

            case Mission_System.RewardType.Passive:
                if (!string.IsNullOrEmpty(missionData.passiveRewardId))
                {
                    var item = PassiveItemManager.Instance.passiveItems.Find(i => i.id == missionData.passiveRewardId);
                    if (item != null)
                    {
                        passiveUI.Show(item.itemName, item.description, item.rating);
                    }
                }
                break;
        }
    }

    // 선택 UI 업데이트 (선택 표시 오브젝트 이동)
    void UpdateSelectionUI()
    {
        if (selectionIndicator != null && missionPositions != null && missionPositions.Length > currentSelectedIndex)
        {
            // 선택 표시 오브젝트를 현재 선택된 미션 위치로 이동
            selectionIndicator.transform.position = missionPositions[currentSelectedIndex].position;
        }
    }

    public void SelectMission(int slotIndex)
    {
        Mission_System selectedSlot = null;

        switch (slotIndex)
        {
            case 0: selectedSlot = missionSlot1; break;
            case 1: selectedSlot = missionSlot2; break;
            case 2: selectedSlot = missionSlot3; break;
        }

        if (selectedSlot == null) return;

        activeMission = selectedSlot.GetCurrentMission();
        isMissionSelected = true;
        isMissionActive = false;
        isMissionCompleted = false;

        // 혼령강화 보상이면 이제 예약 (미션 선택 시점에!)
        if (activeMission.rewardType == Mission_System.RewardType.Passive)
        {
            selectedPassiveId = activeMission.passiveRewardId;

            // 이제 예약 목록에 추가
            if (!string.IsNullOrEmpty(selectedPassiveId))
            {
                PassiveItemManager.Instance.reservedPassiveIds.Add(selectedPassiveId);
                Debug.Log($"미션 보상 예약: {selectedPassiveId}");
            }
        }

        FindObjectOfType<Stage_Manager>().Weapon_ch();
        Mission_ok = false;
        Debug.Log($"미션 선택됨: {activeMission.type}, 목표: {activeMission.targetCount}");
    }
    // 미션 진행도 초기화
    void ResetMissionProgress()
    {
        killCount = 0;
        interactCount = 0;
        recoverCount = 0;
        lightedAreaCount = 0;
        totalAreaCount = 0;
        sellCount = 0;
        weaponsUsedCount = 0;
        lastUsedWeapon = "";
    }

    // === 게임 중 호출될 메서드들 ===

    // 악귀 처치 시 호출
    public void OnEnemyKilled()
    {
        if (!isMissionActive || isMissionCompleted) return;

        killCount++;

        CheckMissionCompletion();
    }

    // 장소 상호작용 시 호출
    public void OnPlaceInteracted()
    {
        if (!isMissionActive || isMissionCompleted) return;

        interactCount++;
        CheckMissionCompletion();
    }

    // 물건 회수하고 탈출 시 호출
    public void OnItemRecovered()
    {
        if (!isMissionActive || isMissionCompleted) return;

        recoverCount++;
        CheckMissionCompletion();
    }

    // 지역 밝힘 시 호출
    public void OnAreaLighted()//어캐 하지 이거
    {
        if (!isMissionActive || isMissionCompleted) return;

        lightedAreaCount++;
        CheckMissionCompletion();
    }

    // 전체 지역 수 설정 (맵 로드 시 호출)
    public void SetTotalAreaCount(int count)
    {
        totalAreaCount = count;
    }

    // 물건 즉시판매 시 호출
    public void OnItemSold()
    {
        if (!isMissionActive || isMissionCompleted) return;

        sellCount++;
        CheckMissionCompletion();
    }

    // 무기 사용 시 호출
    public void OnWeaponUsed(string weaponName)
    {
        if (!isMissionActive || isMissionCompleted) return;

        if (activeMission.type == Mission_System.MissionType.OneWeaponOnly)
        {
            if (string.IsNullOrEmpty(lastUsedWeapon))
            {
                lastUsedWeapon = weaponName;
                weaponsUsedCount = 1;
            }
            else if (lastUsedWeapon != weaponName)
            {
                weaponsUsedCount++;
                // 2개 이상 사용하면 미션 실패
                if (weaponsUsedCount > 1)
                {
                    Debug.Log("무기 1개만 사용 미션 실패!");
                    FailMission();
                }
            }
        }
    }

    // 시간 제한 미션 체크
    void CheckTimeLimitMission()
    {
        if (activeMission.type == Mission_System.MissionType.TimeLimit)
        {
            float elapsedMinutes = (Time.time - missionStartTime) / 60f;

            // 시간 초과 시 미션 실패
            if (elapsedMinutes > activeMission.targetCount)
            {
                Debug.Log("시간 제한 초과! 미션 실패!");
                FailMission();
            }
        }
    }

    // 미션 완료 체크
    void CheckMissionCompletion()
    {
        if (!isMissionActive || isMissionCompleted) return;

        bool isCompleted = false;

        switch (activeMission.type)
        {
            case Mission_System.MissionType.KillEnemies:
                isCompleted = killCount >= activeMission.targetCount;
                break;

            case Mission_System.MissionType.InteractPlaces:
                isCompleted = interactCount >= activeMission.targetCount;
                break;

            case Mission_System.MissionType.RecoverItems:
                isCompleted = recoverCount >= activeMission.targetCount;
                break;

            case Mission_System.MissionType.LightAllAreas:
                isCompleted = lightedAreaCount >= totalAreaCount && totalAreaCount > 0;
                break;

            case Mission_System.MissionType.SellItems:
                isCompleted = sellCount >= activeMission.targetCount;
                break;

            case Mission_System.MissionType.TimeLimit:
                // 탈출 시 별도로 CompleteMission() 호출 필요
                break;

            case Mission_System.MissionType.OneWeaponOnly:
                // 탈출 시 weaponsUsedCount가 1이면 성공
                break;
        }

        if (isCompleted)
        {
            CompleteMission();
        }
    }

    // 미션 완료
    void CompleteMission()
    {
        if (isMissionCompleted) return;

        isMissionCompleted = true;
        Debug.Log("미션 완료!");

        // 상점 씬이면 즉시 보상 지급
        if (IsInShopScene())
        {
            GiveReward();
        }
    }

    // 미션 실패 시
    void FailMission()
    {
        // 혼령강화 예약 취소
        if (!string.IsNullOrEmpty(selectedPassiveId))
        {
            PassiveItemManager.Instance?.CancelPassiveReservation(selectedPassiveId);
            selectedPassiveId = "";
        }

        isMissionActive = false;
        isMissionSelected = false;
        isMissionCompleted = false;
        activeMission = null;
        ResetMissionProgress();
    }

    // 탈출 시 호출 (TimeLimit, OneWeaponOnly 미션 체크)
    public void OnPlayerEscaped()
    {
        if (!isMissionActive || isMissionCompleted) return;

        if (activeMission.type == Mission_System.MissionType.TimeLimit)
        {
            float elapsedMinutes = (Time.time - missionStartTime) / 60f;
            if (elapsedMinutes <= activeMission.targetCount)
            {
                CompleteMission();
            }
            else
            {
                FailMission();
            }
        }
        else if (activeMission.type == Mission_System.MissionType.OneWeaponOnly)
        {
            if (weaponsUsedCount <= 1)
            {
                CompleteMission();
            }
            else
            {
                FailMission();
            }
        }
    }

    // 상점 진입 시 호출 (보상 지급 체크)
    public void OnEnterShop()
    {
        if (isMissionCompleted && !IsInShopScene())
        {
            // 씬 전환 후 상점에서 보상 지급
            Invoke(nameof(GiveReward), 0.5f);
        }
    }

    // 보상 지급
    void GiveReward()
    {
        if (activeMission == null) return;

        switch (activeMission.rewardType)
        {
            case Mission_System.RewardType.Soul:
                GameManager.Instance?.Add_Soul(activeMission.rewardValue);
                Debug.Log($"보상 지급: 혼 {activeMission.rewardValue}");
                break;

            case Mission_System.RewardType.Money:
                GameManager.Instance?.Add_Gold(activeMission.rewardValue);
                Debug.Log($"보상 지급: 냥 {activeMission.rewardValue}");
                break;

            case Mission_System.RewardType.Passive:
                if (!string.IsNullOrEmpty(selectedPassiveId))
                {
                    PassiveItemManager.Instance?.ConfirmPassivePurchase(selectedPassiveId);
                    Debug.Log($"보상 지급: 혼령강화 {selectedPassiveId}");
                    selectedPassiveId = "";
                }
                break;
        }

        isMissionActive = false;
        isMissionSelected = false;
        isMissionCompleted = false;
        activeMission = null;
    }

    // === 외부 접근용 메서드 ===

    // 현재 미션 정보 가져오기
    public Mission_System.MissionData GetActiveMission()
    {
        return activeMission;
    }

    // 미션 진행중인지 확인
    public bool IsMissionActive()
    {
        return isMissionActive;
    }

    // 미션 완료 여부
    public bool IsMissionCompleted()
    {
        return isMissionCompleted;
    }

    // 현재 진행도 가져오기
    public string GetMissionProgress()
    {
        if (!isMissionActive || activeMission == null) return "미션 없음";

        switch (activeMission.type)
        {
            case Mission_System.MissionType.KillEnemies:
                return $"악귀 처치: {killCount}/{activeMission.targetCount}";
            case Mission_System.MissionType.InteractPlaces:
                return $"상호작용: {interactCount}/{activeMission.targetCount}";
            case Mission_System.MissionType.RecoverItems:
                return $"회수: {recoverCount}/{activeMission.targetCount}";
            case Mission_System.MissionType.LightAllAreas:
                return $"밝힘: {lightedAreaCount}/{totalAreaCount}";
            case Mission_System.MissionType.SellItems:
                return $"판매: {sellCount}/{activeMission.targetCount}";
            case Mission_System.MissionType.TimeLimit:
                float elapsed = (Time.time - missionStartTime) / 60f;
                return $"경과 시간: {elapsed:F1}/{activeMission.targetCount}분";
            case Mission_System.MissionType.OneWeaponOnly:
                return $"사용 무기: {weaponsUsedCount}/1";
            default:
                return "";
        }
    }

    // === 인게임 미션 UI ===

    // 인게임 미션 UI 업데이트
    void UpdateInGameMissionUI()
    {
        // activeMission이 null이면 UI 숨김
        if (activeMission == null)
        {
            HideMissionUI();
            return;
        }

        // UI 표시
        ShowMissionUI();

        // 미션 텍스트 구성
        string missionText = "[임무]\n";
        string progressText = "";

        switch (activeMission.type)
        {
            case Mission_System.MissionType.KillEnemies:
                progressText = $"악귀 ({killCount}/{activeMission.targetCount})마리 처치";
                break;

            case Mission_System.MissionType.InteractPlaces:
                progressText = $"장소 ({interactCount}/{activeMission.targetCount})회 상호작용";
                break;

            case Mission_System.MissionType.RecoverItems:
                progressText = $"물건 ({recoverCount}/{activeMission.targetCount})개 회수";
                break;

            case Mission_System.MissionType.LightAllAreas:
                progressText = $"지역 ({lightedAreaCount}/{totalAreaCount})곳 밝히기";
                break;

            case Mission_System.MissionType.SellItems:
                progressText = $"물건 ({sellCount}/{activeMission.targetCount})회 판매";
                break;

            case Mission_System.MissionType.TimeLimit:
                float elapsed = (Time.time - missionStartTime) / 60f;
                float remaining = activeMission.targetCount - elapsed;
                if (remaining < 0) remaining = 0;
                progressText = $"{remaining:F1}각 안에 탈출";
                break;

            case Mission_System.MissionType.OneWeaponOnly:
                progressText = weaponsUsedCount <= 1 ? "무기 1개만 사용 중" : "무기 제한 실패!";
                break;
        }

        missionText += progressText;

        // 완료 상태 표시
        if (isMissionCompleted)
        {
            missionText += "\n<color=#00FF00>(완료!)</color>";
        }

        // 텍스트 업데이트
        if (inGameMissionText != null)
        {
            inGameMissionText.text = missionText;
        }
    }

    // 미션 UI 표시
    void ShowMissionUI()
    {
        if (missionUIPanel != null && !missionUIPanel.activeSelf)
        {
            missionUIPanel.SetActive(true);
        }
    }

    // 미션 UI 숨김
    void HideMissionUI()
    {
        if (missionUIPanel != null && missionUIPanel.activeSelf)
        {
            missionUIPanel.SetActive(false);
        }
    }
}