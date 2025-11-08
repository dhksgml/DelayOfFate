using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Mission_System : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text missionNameText;
    public TMP_Text missionDescText;
    public TMP_Text missionGradeText;
    public TMP_Text missionTargetText;
    public TMP_Text rewardText;

    [Header("UI References")]
    public Image rewardIconImages; // 보상 아이콘을 표시할 Image 컴포넌트들

    private PassiveItemManager passiveItemManager;
    

    public Sprite soul_icon;
    public Sprite coin_icon;

    // 미션 등급
    public enum MissionGrade
    {
        Low,    // 하급
        Mid,    // 중급
        High    // 상급
    }

    // 미션 타입
    public enum MissionType
    {
        KillEnemies,        // 악귀 N마리 처치
        InteractPlaces,     // 장소 상호작용 N회
        RecoverItems,       // 잃어버린 N개 물건 회수
        LightAllAreas,      // 모든 지역 밝히기
        SellItems,          // 물건 즉시 N회 판매
        TimeLimit,          // N시간 안에 탈출
        OneWeaponOnly       // 무기 1개만 사용하기
    }

    // 보상 타입
    public enum RewardType
    {
        Soul,           // 혼
        Money,          // 냥
        PassiveLow,     // 혼령강화 하급
        PassiveMid,     // 혼령강화 중급
        PassiveHigh,    // 혼령강화 상급
        PassiveMax      // 혼령강화 최상급
    }


    // 현재 미션 정보
    [System.Serializable]
    public class MissionData
    {
        public MissionGrade grade;
        public MissionType type;
        public int targetCount;
        public string weaponType;
        public RewardType rewardType;
        public int rewardValue;
        public string passiveRewardId; // 혼령강화 보상 ID 추가
    }

    private MissionData currentMission;

    void Start()
    {
        passiveItemManager = FindObjectOfType<PassiveItemManager>();
        
        GenerateRandomMission();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateRandomMission();
        }
    }

    // 랜덤 미션 생성
    public void GenerateRandomMission()
    {
        currentMission = new MissionData();

        // 랜덤 등급 결정
        currentMission.grade = (MissionGrade)Random.Range(0, 3);

        // 등급에 따라 가능한 미션 타입 필터링
        List<MissionType> availableMissions = new List<MissionType>
        {
            MissionType.KillEnemies,
            MissionType.InteractPlaces,
            MissionType.RecoverItems,
            MissionType.SellItems,
            MissionType.TimeLimit
        };

        // 중급, 상급만 가능한 미션 추가
        if (currentMission.grade != MissionGrade.Low)
        {
            availableMissions.Add(MissionType.LightAllAreas);
            availableMissions.Add(MissionType.OneWeaponOnly);
        }

        // 랜덤 미션 타입 선택
        currentMission.type = availableMissions[Random.Range(0, availableMissions.Count)];

        // 미션 목표 수치 설정
        SetMissionTarget();

        // 보상 결정
        SetMissionReward();

        // UI 업데이트
        UpdateMissionUI();
    }

    // 미션 목표 수치 설정
    void SetMissionTarget()
    {
        switch (currentMission.type)
        {
            case MissionType.KillEnemies:
                currentMission.targetCount = currentMission.grade == MissionGrade.Low ? 10 :
                                            currentMission.grade == MissionGrade.Mid ? 15 : 20;
                break;

            case MissionType.InteractPlaces:
                currentMission.targetCount = currentMission.grade == MissionGrade.Low ? 2 :
                                            currentMission.grade == MissionGrade.Mid ? 3 : 4;
                break;

            case MissionType.RecoverItems:
                currentMission.targetCount = currentMission.grade == MissionGrade.Low ? 1 :
                                            currentMission.grade == MissionGrade.Mid ? 2 : 3;
                break;

            case MissionType.LightAllAreas:
                currentMission.targetCount = 1;
                break;

            case MissionType.SellItems:
                currentMission.targetCount = currentMission.grade == MissionGrade.Low ? 4 :
                                            currentMission.grade == MissionGrade.Mid ? 7 : 10;
                break;

            case MissionType.TimeLimit:
                currentMission.targetCount = currentMission.grade == MissionGrade.Low ? 16 :
                                            currentMission.grade == MissionGrade.Mid ? 14 : 12;
                break;

            case MissionType.OneWeaponOnly:
                currentMission.targetCount = 1;
                break;
        }
    }

    // 보상 결정 (확률 기반)
    void SetMissionReward()
    {
        float roll = Random.Range(0f, 100f);

        switch (currentMission.grade)
        {
            case MissionGrade.Low:
                if (roll < 15f)
                {
                    currentMission.rewardType = RewardType.Soul;
                    currentMission.rewardValue = 350;
                }
                else if (roll < 30f)
                {
                    currentMission.rewardType = RewardType.Money;
                    currentMission.rewardValue = 150;
                }
                else if (roll < 80f)
                {
                    currentMission.rewardType = RewardType.PassiveLow;
                    currentMission.passiveRewardId = PassiveItemManager.Instance.GetRandomPassiveForMission(0);// 혼령강화 ID 받기
                }
                else
                {
                    currentMission.rewardType = RewardType.PassiveMid;
                    currentMission.passiveRewardId = PassiveItemManager.Instance.GetRandomPassiveForMission(0);
                }
                break;

            case MissionGrade.Mid:
                if (roll < 15f)
                {
                    currentMission.rewardType = RewardType.Soul;
                    currentMission.rewardValue = 500;
                }
                else if (roll < 30f)
                {
                    currentMission.rewardType = RewardType.Money;
                    currentMission.rewardValue = 215;
                }
                else if (roll < 80f)
                {
                    currentMission.rewardType = RewardType.PassiveMid;
                    currentMission.passiveRewardId = PassiveItemManager.Instance.GetRandomPassiveForMission(0);// 혼령강화 ID 받기
                }
                else
                {
                    currentMission.rewardType = RewardType.PassiveHigh;
                    currentMission.passiveRewardId = PassiveItemManager.Instance.GetRandomPassiveForMission(0);
                }
                break;

            case MissionGrade.High:
                if (roll < 15f)
                {
                    currentMission.rewardType = RewardType.Soul;
                    currentMission.rewardValue = 850;
                }
                else if (roll < 30f)
                {
                    currentMission.rewardType = RewardType.Money;
                    currentMission.rewardValue = 300;
                }
                else if (roll < 80f)
                {
                    currentMission.rewardType = RewardType.PassiveHigh;
                    currentMission.passiveRewardId = PassiveItemManager.Instance.GetRandomPassiveForMission(0);// 혼령강화 ID 받기
                }
                else
                {
                    currentMission.rewardType = RewardType.PassiveMax;
                    currentMission.passiveRewardId = PassiveItemManager.Instance.GetRandomPassiveForMission(0);// 혼령강화 ID 받기
                }
                break;
        }
    }
    void UpdateRewardIcon()
    {
        string rewardStr = "";

        if (currentMission.rewardType == RewardType.Money)
        {
            rewardIconImages.sprite = coin_icon;
            rewardStr = $"{currentMission.rewardValue}<sprite=9>";
            rewardIconImages.gameObject.SetActive(true);
        }
        else if (currentMission.rewardType == RewardType.Soul)
        {
            rewardIconImages.sprite = soul_icon;
            rewardStr = $"{currentMission.rewardValue}<sprite=8>";
            rewardIconImages.gameObject.SetActive(true);
        }
        else
        {
            // passiveRewardId null 체크
            if (string.IsNullOrEmpty(currentMission.passiveRewardId))
            {
                Debug.LogError("passiveRewardId가 null입니다!");
                rewardIconImages.gameObject.SetActive(false);
                rewardStr = "보상 오류";
                rewardText.text = $"[{rewardStr}]";
                return;
            }

            string[] parts = currentMission.passiveRewardId.Split('_');
            if (parts.Length < 4)
            {
                Debug.LogError($"잘못된 passiveRewardId: {currentMission.passiveRewardId}");
                rewardIconImages.gameObject.SetActive(false);
                rewardStr = "보상 오류";
                rewardText.text = $"[{rewardStr}]";
                return;
            }

            int group = int.Parse(parts[2]);
            int number = int.Parse(parts[3]);

            // passiveItemManager null 체크 추가
            if (passiveItemManager == null)
            {
                passiveItemManager = FindObjectOfType<PassiveItemManager>();
                if (passiveItemManager == null)
                {
                    Debug.LogError("PassiveItemManager를 찾을 수 없습니다!");
                    rewardIconImages.gameObject.SetActive(false);
                    rewardStr = "보상 오류";
                    rewardText.text = $"[{rewardStr}]";
                    return;
                }
            }

            Sprite icon = passiveItemManager.GetIcon(group, number);

            if (icon != null)
            {
                rewardIconImages.sprite = icon;
                rewardIconImages.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"아이콘을 찾을 수 없음: group={group}, number={number}");
                rewardIconImages.gameObject.SetActive(false);
            }

            rewardStr = passiveItemManager.GetPassiveName(group, number);
        }

        rewardText.text = $"[{rewardStr}]";
    }
    // UI 업데이트
    void UpdateMissionUI()
    {
        // 등급 표시
        string gradeColor = currentMission.grade == MissionGrade.Low ? "#00FF00" :
                           currentMission.grade == MissionGrade.Mid ? "#FFA500" : "#FF0000";
        string gradeText = currentMission.grade == MissionGrade.Low ? "하급" :
                          currentMission.grade == MissionGrade.Mid ? "중급" : "상급";

        missionGradeText.text = $"<color={gradeColor}>[{gradeText}]</color>";

        // 미션명과 설명
        string missionName = "";
        string missionDesc = "";
        string targetText = "";

        switch (currentMission.type)
        {
            case MissionType.KillEnemies:
                missionName = "악귀 토벌";
                missionDesc = "정해진 수의 악귀를 처치하세요.";
                targetText = $"악귀 {currentMission.targetCount}마리 처치";
                break;

            case MissionType.InteractPlaces:
                missionName = "장소 조사";
                missionDesc = "특정 장소들과 상호작용하세요.";
                targetText = $"장소 {currentMission.targetCount}회 상호작용";
                break;

            case MissionType.RecoverItems:
                missionName = "유실물 회수";
                missionDesc = "잃어버린 물건들을 회수하세요.";
                targetText = $"물건 {currentMission.targetCount}개 회수";
                break;

            case MissionType.LightAllAreas:
                missionName = "구역 조명";
                missionDesc = "모든 지역을 밝히세요.";
                targetText = "모든 지역 밝히기";
                break;

            case MissionType.SellItems:
                missionName = "긴급 처분";
                missionDesc = "현장에서 물건을 즉시 판매하세요.";
                targetText = $"물건 {currentMission.targetCount}회 판매";
                break;

            case MissionType.TimeLimit:
                missionName = "신속 작전";
                missionDesc = "제한 시간 내에 탈출하세요.";
                targetText = $"{currentMission.targetCount}각 안에 탈출";
                break;

            case MissionType.OneWeaponOnly:
                missionName = "제한 무장";
                missionDesc = "무기 1개만 사용하여 임무를 완수하세요.";
                targetText = "무기 1개만 사용";
                break;
        }

        missionNameText.text = "["+missionName+"]";
        missionDescText.text = missionDesc;
        missionTargetText.text = $"[목표]\n{targetText}";


        // 보상 아이콘 업데이트 추가
        UpdateRewardIcon();

    }


    // 외부에서 현재 미션 정보 가져오기
    public MissionData GetCurrentMission()
    {
        return currentMission;
    }

    // 미션 완료 체크 (게임 중 호출)
    public bool CheckMissionComplete(MissionType type, int currentProgress, string weaponUsed = "")
    {
        if (currentMission.type != type) return false;

        return currentProgress >= currentMission.targetCount;
    }

    // 미션 완료 시 보상 지급 (상점에서 호출)
    public void GiveMissionReward()
    {
        switch (currentMission.rewardType)
        {
            case RewardType.Soul:
                GameManager.Instance?.Add_Soul(currentMission.rewardValue);
                Debug.Log($"혼 {currentMission.rewardValue}개 획득!");
                break;

            case RewardType.Money:
                GameManager.Instance?.Add_Gold(currentMission.rewardValue);
                Debug.Log($"냥 {currentMission.rewardValue}전 획득!");
                break;

            case RewardType.PassiveLow:
            case RewardType.PassiveMid:
            case RewardType.PassiveHigh:
            case RewardType.PassiveMax:
                // PassiveItemManager를 통해 랜덤 혼령강화 지급
                Debug.Log($"{currentMission.rewardType} 혼령강화 획득!");
                // TODO: 실제 혼령강화 지급 로직 구현
                break;
        }
    }
}