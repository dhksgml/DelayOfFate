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
    public Image rewardIconImages;

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
        KillEnemies,
        InteractPlaces,
        RecoverItems,
        LightAllAreas,
        SellItems,
        TimeLimit,
        OneWeaponOnly
    }

    // 보상 타입
    public enum RewardType
    {
        Soul,
        Money,
        Passive
    }

    [System.Serializable]
    public class MissionData
    {
        public MissionGrade grade;
        public MissionType type;
        public int targetCount;
        public RewardType rewardType;
        public int rewardValue;
        public string passiveRewardId;
    }

    private MissionData currentMission;

    // 일차별 미션 등급 확률 테이블
    private Dictionary<int, Dictionary<int, float>> dayMissionGradeProbabilities = new Dictionary<int, Dictionary<int, float>>()
    {
        { 1, new Dictionary<int, float> { {0, 80f}, {1, 15f}, {2, 5f} } },
        { 2, new Dictionary<int, float> { {0, 55f}, {1, 35f}, {2, 10f} } },
        { 3, new Dictionary<int, float> { {0, 45f}, {1, 45f}, {2, 10f} } },
        { 4, new Dictionary<int, float> { {0, 35f}, {1, 40f}, {2, 25f} } },
        { 5, new Dictionary<int, float> { {0, 15f}, {1, 40f}, {2, 45f} } },
        { 6, new Dictionary<int, float> { {0, 10f}, {1, 30f}, {2, 60f} } },
        { 7, new Dictionary<int, float> { {0, 5f}, {1, 20f}, {2, 75f} } }
    };

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

    // 현재 일차에 맞는 미션 등급 결정
    MissionGrade GetMissionGradeByCurrentDay()
    {
        int currentDay = GameManager.Instance.Day;
        if (currentDay > 7) currentDay = 7;

        if (!dayMissionGradeProbabilities.ContainsKey(currentDay))
        {
            currentDay = 1;
        }

        Dictionary<int, float> probabilities = dayMissionGradeProbabilities[currentDay];
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var kvp in probabilities)
        {
            cumulative += kvp.Value;
            if (roll < cumulative)
            {
                return (MissionGrade)kvp.Key;
            }
        }

        return MissionGrade.Low;
    }

    public void GenerateRandomMission()
    {
        currentMission = new MissionData();

        // 일차별 확률로 미션 등급 결정
        currentMission.grade = GetMissionGradeByCurrentDay();

        // 미션 타입 선택
        List<MissionType> availableMissions = new List<MissionType>
        {
            MissionType.KillEnemies,
            MissionType.InteractPlaces,
            MissionType.RecoverItems,
            MissionType.SellItems,
            MissionType.TimeLimit
        };

        if (currentMission.grade != MissionGrade.Low)
        {
            availableMissions.Add(MissionType.LightAllAreas);
            availableMissions.Add(MissionType.OneWeaponOnly);
        }

        currentMission.type = availableMissions[Random.Range(0, availableMissions.Count)];

        SetMissionTarget();
        SetMissionReward();
        UpdateMissionUI();
    }

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

    void SetMissionReward()
    {
        // 보상 타입 결정 (혼령강화 70%, 혼 15%, 냥 15%)
        float rewardTypeRoll = Random.Range(0f, 100f);

        if (rewardTypeRoll < 70f) // 혼령강화
        {
            currentMission.rewardType = RewardType.Passive;

            // 미션 등급에 따른 혼령강화 등급 결정 (50% 50%)
            int passiveRating = 1;
            float passiveRoll = Random.Range(0f, 100f);

            switch (currentMission.grade)
            {
                case MissionGrade.Low: // 하급 50%, 중급 50%
                    passiveRating = passiveRoll < 50f ? 1 : 2;
                    break;
                case MissionGrade.Mid: // 중급 50%, 상급 50%
                    passiveRating = passiveRoll < 50f ? 2 : 3;
                    break;
                case MissionGrade.High: // 상급 50%, 최상급 50%
                    passiveRating = passiveRoll < 50f ? 3 : 4;
                    break;
            }

            // 혼령강화 선택 (예약하지 않음!)
            string passiveId = null;
            int attempts = 0;
            while (string.IsNullOrEmpty(passiveId) && attempts < 100)
            {
                // GetRandomPassiveByGradeWithoutReserve 사용 (예약 없이 선택만)
                passiveId = PassiveItemManager.Instance.GetRandomPassiveByGradeWithoutReserve(passiveRating);
                attempts++;
            }

            currentMission.passiveRewardId = passiveId;
            currentMission.rewardValue = 0;
        }
        else if (rewardTypeRoll < 85f) // 혼 15%
        {
            currentMission.rewardType = RewardType.Soul;
            currentMission.rewardValue = currentMission.grade == MissionGrade.Low ? 350 :
                                         currentMission.grade == MissionGrade.Mid ? 500 : 850;
            currentMission.passiveRewardId = "";
        }
        else // 냥 15%
        {
            currentMission.rewardType = RewardType.Money;
            currentMission.rewardValue = currentMission.grade == MissionGrade.Low ? 150 :
                                         currentMission.grade == MissionGrade.Mid ? 215 : 300;
            currentMission.passiveRewardId = "";
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
        else if (currentMission.rewardType == RewardType.Passive)
        {
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

    void UpdateMissionUI()
    {
        string gradeColor = currentMission.grade == MissionGrade.Low ? "#00FF00" :
                           currentMission.grade == MissionGrade.Mid ? "#FFA500" : "#FF0000";
        string gradeText = currentMission.grade == MissionGrade.Low ? "하급" :
                          currentMission.grade == MissionGrade.Mid ? "중급" : "상급";

        missionGradeText.text = $"<color={gradeColor}>[{gradeText}]</color>";

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

        missionNameText.text = "[" + missionName + "]";
        missionDescText.text = missionDesc;
        missionTargetText.text = $"[목표]\n{targetText}";

        UpdateRewardIcon();
    }

    public MissionData GetCurrentMission()
    {
        return currentMission;
    }
}