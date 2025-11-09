using System.Collections.Generic;
using UnityEngine;

public class PassiveItemManager : MonoBehaviour
{
    public static PassiveItemManager Instance { get; private set; }

    public List<PassiveItemData> passiveItems;
    public Sprite[] Passive_Item_Icon_1;
    public Sprite[] Passive_Item_Icon_2;
    public Sprite[] Passive_Item_Icon_3;
    public Sprite[] Passive_Item_Icon_4;
    public Sprite[] Passive_Item_Icon_5;
    public Sprite[] Passive_Item_Icon_6;
    public Sprite[] Passive_Item_Icon_7;

    private List<IPassiveEffect> activeEffects = new();
    private int passive_6_1_count = 0;
    //private float lastBonusSpeed_5_2 = 0f;
    //private float lastBonusDamage = 0f;

    private Dictionary<string, float> passiveSpeedBonuses = new Dictionary<string, float>();
    private Dictionary<string, float> passiveDamageBonuses = new Dictionary<string, float>();

    public List<string> reservedPassiveIds = new List<string>(); // 미션/상점에서 예약된 ID

    void Awake()
    {
        PlayerPrefs.DeleteAll();

        // 현재 씬에 자신과 같은 타입의 오브젝트가 2개 이상 있는 경우 즉시 삭제
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        passiveItems = new List<PassiveItemData>();

        // 종류별 2개씩 총 7종류
        for (int g = 1; g <= 7; g++)
        {
            for (int n = 1; n <= 2; n++)
            {
                AddPassiveItem(g, n);
            }
        }
        AddPassiveItem(1, 3);
        AddPassiveItem(1, 4);
        // 3번째 효과 Soul_Add_2_3, Soul_Add_4_3, Soul_Add_6_3
        for (int n = 2; n <= 6; n += 2)
        {
            AddPassiveItem(n, 3);
        }
        // Soul_Add_8_1 ~ Soul_Add_8_5
        for (int n = 1; n <= 5; n++)
        {
            AddPassiveItem(8, n);
        }
        // Soul_Add_9_1 ~ Soul_Add_9_2
        for (int n = 1; n <= 3; n++)
        {
            AddPassiveItem(9, n);
        }
        // Soul_Add_10_1 ~ Soul_Add_10_8
        for (int n = 1; n <= 8; n++)
        {
            AddPassiveItem(10, n);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnNextDay += HandleNextDay;
        GameEvents.OnSaleItemImmediately += HandleSaleItemImmediately;
        GameEvents.OnPickupItem += HandlePickupItem;
        GameEvents.OnDropItem += HandleDropItem;
        GameEvents.OnTimeAngleUnit18 += HadleTimeAngleUnit18;
    }

    private void OnDisable()
    {
        GameEvents.OnNextDay -= HandleNextDay;
        GameEvents.OnSaleItemImmediately -= HandleSaleItemImmediately;
        GameEvents.OnPickupItem -= HandlePickupItem;
        GameEvents.OnDropItem -= HandleDropItem;
        GameEvents.OnTimeAngleUnit18 -= HadleTimeAngleUnit18;
    }

    void Start()
    {
        // 저장된 값 불러오기
        foreach (var item in passiveItems)
        {
            if (PlayerPrefs.GetInt(item.id, 0) == 1)
            {
                item.isPurchased = true;
                ApplyPassiveEffect(item.id);
            }
        }
    }
    private void AddPassiveItem(int g, int n)
    {
        string id = $"Soul_Add_{g}_{n}";
        string name = GetPassiveName(g, n);
        string desc = GetPassiveDescription(g, n);
        int emdrmq = GetPassiveEmdrmq(g, n);

        bool purchased = PlayerPrefs.GetInt(id, 0) == 1;

        passiveItems.Add(new PassiveItemData
        {
            id = id,
            itemName = name,
            description = desc,
            isPurchased = purchased,
            rating = emdrmq
        });

        if (purchased)
            ApplyPassiveEffect(id);
    }

    public string GetPassiveName(int group, int number)
    {
        switch ($"{group}_{number}")
        {
            case "1_1": return "천하장사";
            case "1_2": return "정정당당";
            case "1_3": return "속전속결";
            case "1_4": return "기고만장";
            case "1_5": return "백전백승";
            case "1_6": return "불철주야";

            case "2_1": return "문전박대";
            case "2_2": return "백발백중";
            case "2_3": return "쾌도난마";
            case "2_4": return "일망타진";
            case "2_5": return "현호세세";

            case "3_1": return "금의환향";
            case "3_2": return "다다익선";
            case "3_3": return "일확천금";

            case "4_1": return "금강불괴";
            case "4_2": return "외강내유";
            case "4_3": return "외유내강";

            case "5_1": return "가담항설";
            case "5_2": return "취사선택";
            case "5_3": return "경화수월";
            case "5_4": return "어부지리";

            case "6_1": return "등용문";
            case "6_2": return "승승장구";
            case "6_3": return "선견지명";
            case "6_4": return "일취월장";
            case "6_5": return "독불장군";

            case "7_1": return "구사일생";
            case "7_2": return "궁여지책";
            case "7_3": return "무아지경";
            case "7_4": return "배수진";
            //패시브 아님
            case "8_1": return "환도";
            case "8_2": return "방망이";
            case "8_3": return "부적";
            case "8_4": return "호리병";
            case "8_5": return "족자";
            case "9_1": return "";
            case "9_2": return "";
            case "10_1": return "상점";//상점
            case "10_2": return "장비";//이전
            case "10_3": return "재입고";//리롤
            case "10_4": return "전투";//전투로
            case "10_5": return "냥교환";
            case "10_6": return "혼교환";
            // ...
            default: return " ";
        }
    }

    #region Bonus Speed
    public float GetTotalBonusSpeed()
    {
        float totalBonus = 0f;
        foreach (var kvp in passiveSpeedBonuses)
        {
            string passiveId = kvp.Key;
            float bonus = kvp.Value;

            // 플레이어가 해당 패시브를 가지고 있을 때만 더하기
            if (HasEffect(passiveId))
            {
                totalBonus += bonus;
            }
        }


        return totalBonus;
    }

    public void SetPassiveSpeedBonus(string passiveId, float value)
    {
        passiveSpeedBonuses[passiveId] = value;
    }

    public void RemovePassiveSpeedBonus(string passiveId)
    {
        if (passiveSpeedBonuses.ContainsKey(passiveId))
            passiveSpeedBonuses.Remove(passiveId);
    }
    #endregion

    #region Bonus Damage
    public float GetTotalBonusDamage()
    {
        float total = 0f;
        foreach (var kvp in passiveDamageBonuses)
        {
            if (HasEffect(kvp.Key))
                total += kvp.Value;
        }
        return total;
    }

    public void SetPassiveDamageBonus(string passiveId, float value)
    {
        passiveDamageBonuses[passiveId] = value;
    }

    public void RemovePassiveDamageBonus(string passiveId)
    {
        passiveDamageBonuses.Remove(passiveId);
    }

    #endregion

    public string GetPassiveDescription(int group, int number)
    {
        //16글자 마다 줄 바꿈이 됨
        switch ($"{group}_{number}")
        {
            //<sprite=8> 
            //<sprite=9> 
            case "1_1": return "소지한 물건당 피해 1할 증가";
            case "1_2": return "악귀에게 주는 피해가 10할 증가\n<color=red>악귀의 약점을 공격 할 수 없음</color>";
            case "1_3": return "최대체력인 악귀에게 피해량 3할 증가";
            case "1_4": return "최대체력 일때 피해량 3할 증가";
            case "1_5": return "오늘 처치한 악귀의 수 만큼 피해량 1 증가\n(최대 10)";
            case "1_6": return "12각 이상시 피해량 5할 증가";

            case "2_1": return "방망이의 피해량이 5할 증가";
            case "2_2": return "<color=red>부적의 피해량 2 감소</color>\n부적을 3개씩 던짐";
            case "2_3": return "환도의 공격속도 5할 증가\n환도의 피해량이 5증가";
            case "2_4": return "족자로 동시에 3 이상을 대상으로 공격에 성공했다면\n소모한 정신력을 회복함";
            case "2_5": return "호리병으로 적을 처치하면 혼을 2배로 획득\n재사용 시간이 5할 줄어듬";

            case "3_1": return "약값 지불 후 보유한 <sprite=8>의\n3할 만큼 획득";
            case "3_2": return "보유한 200 <sprite=9> 당 이동속도 1할 증가\n(최대 3할)";
            case "3_3": return "물건 즉시판매시 1할의 확률로 1000 냥 획득";

            case "4_1": return "악귀로 받는 체력피해 5할 감소";
            case "4_2": return "체력 75 증가\n<color=red>정신 25 감소</color>";
            case "4_3": return "정신 75 증가\n<color=red>체력 25 감소</color>";

            case "5_1": return "가장 가까운 물건의\n위치를 파악함";
            case "5_2": return "비어 있는 손 만큼\n이동속도 1할 증가";
            case "5_3": return "호롱불이 꺼지지 않음";
            case "5_4": return "악귀로 부터 혼을 얻을 때\n10할 추가 획득";

            case "6_1": return "4일차 이후라면 이동속도, 피해량 3할 상승";
            case "6_2": return "악귀 처치시 체력 5회복, 정신 3회복";
            case "6_3": return "하루가 지날때 들고 있던 물건의\n가치가 10할 증가";
            case "6_4": return "보유한 300 혼 당 피해량 1할 증가\n(최대 7할)";
            case "6_5": return "오늘 상호작용한 장소 수만큼 피해량 1할 증가\n(최대 4할)";

            case "7_1": return "체력이 3할 이하시 이동속도 3할 증가";
            case "7_2": return "18각 이상시\n달리기 속도가 5할 증가";
            case "7_3": return "체력 5할 이하면 피해량 5할 증가";
            case "7_4": return "체력 150 증가\n<color=red>체력 5할 이상시 체력회복불가</color>";
            //패시브 아님
            case "8_1": return "전방을 공격해 10~14의 피해를 입힘\n약점의 경우 즉사시킴";
            case "8_2": return "전방을 공격해 20~30의 피해를 입힘\n약점의 경우 즉사시킴";
            case "8_3": return "전방에 부적을 던져 6~8의 피해를 입힘\n약점의 경우 최대체력 5할의 피해를 입힘";
            case "8_4": return "전방의 적에게 100의 피해를 입힘\n재사용 시간 1각\n약점의 경우 재사용 시간이 초기화됨";
            case "8_5": return "화면의 모든 악귀에게\n최대체력 5할의 피해를 입힘\n약점의 경우 즉사시킴\n정신력 8~12 소모";
            case "9_1": return "빛이 더 강해짐";
            case "9_2": return "달려도 빛이 꺼지지 않음";
            case "10_1": return "혼령강화를 구매하러 감";//상점
            case "10_2": return "장비를 변경함";//이전
            case "10_3": return "혼령강화의 목록을\n새로운 품목으로 교체함";//리롤
            case "10_4": return "전투로부터 돈을 벌러 감";//전투로
            case "10_5": return "100 <sprite=8>을 50 <sprite=9>으로 교환함";
            case "10_6": return "100 <sprite=9>을 50 <sprite=8>으로 교환함";
            // ...
            default: return "설명이 없습니다.";
        }
    }
    public int GetPassiveEmdrmq(int group, int number) //아이템의 등급
    {
        switch ($"{group}_{number}")
        {
            case "1_1": return 2;
            case "1_2": return 3;
            case "1_3": return 1;
            case "1_4": return 1;
            case "1_5": return 2;
            case "1_6": return 2;

            case "2_1": return 1;
            case "2_2": return 1;
            case "2_3": return 1;
            case "2_4": return 1;
            case "2_5": return 1;

            case "3_1": return 3;
            case "3_2": return 2;
            case "3_3": return 1;

            case "4_1": return 4;
            case "4_2": return 1;
            case "4_3": return 1;

            case "5_1": return 1;
            case "5_2": return 2;
            case "5_3": return 1;
            case "5_4": return 2;

            case "6_1": return 2;
            case "6_2": return 2;
            case "6_3": return 3;
            case "6_4": return 3;
            case "6_5": return 1;

            case "7_1": return 2;
            case "7_2": return 1;
            case "7_3": return 3;
            case "7_4": return 2;

            //패시브 아님
            case "8_1": return 5;
            case "8_2": return 5;
            case "8_3": return 5;
            case "8_4": return 5;
            case "8_5": return 5;
            case "9_1": return 6;
            case "9_2": return 6;
            // ...
            default: return 0;
        }
    }
    public void PurchaseItem(string itemId)//구매 후 데이터 저장
    {
        PassiveItemData item = passiveItems.Find(i => i.id == itemId);
        if (item != null && !item.isPurchased)
        {
            item.isPurchased = true;
            PlayerPrefs.SetInt(item.id, 1);
            ApplyPassiveEffect(itemId);
        }
    }
    public bool IsPurchased(string id)
    {
        var item = passiveItems.Find(i => i.id == id);
        return item != null && item.isPurchased;
    }
    public void ResetPassiveItem()
    {
        PlayerPrefs.DeleteAll();
        foreach (var item in passiveItems)
        {
            if (item.isPurchased)
                RemovePassiveEffect(item.id);
            item.isPurchased = false;
        }
    }
    public Sprite GetIcon(int group, int number)
    {
        Sprite[] groupArray = null;
        switch (group)
        {
            case 1: groupArray = Passive_Item_Icon_1; break;
            case 2: groupArray = Passive_Item_Icon_2; break;
            case 3: groupArray = Passive_Item_Icon_3; break;
            case 4: groupArray = Passive_Item_Icon_4; break;
            case 5: groupArray = Passive_Item_Icon_5; break;
            case 6: groupArray = Passive_Item_Icon_6; break;
            case 7: groupArray = Passive_Item_Icon_7; break;
        }

        if (groupArray == null || number < 1 || number > groupArray.Length)
        {
            Debug.LogWarning($"GetIcon 실패: group={group}, number={number}");
            return null;
        }

        return groupArray[number - 1];
    }

    public bool HasEffect(string id)//값 불러오기
    {
        var item = passiveItems.Find(i => i.id == id);
        return item != null && item.isPurchased;
    }
    void ApplyPassiveEffect(string itemId)
    {
        switch (itemId)
        {
            case "Soul_Add_1_1":// 
                break;
            case "Soul_Add_1_2":// 
                DoPassive_1_2();
                break;
            case "Soul_Add_1_3":// 
                break;
            case "Soul_Add_1_4":// 
                break;
            case "Soul_Add_1_5":// 
                break;
            case "Soul_Add_1_6":// 
                break;

            case "Soul_Add_2_1":// 
                break;
            case "Soul_Add_2_2":// 
                break;
            case "Soul_Add_2_3":// 
                break;
            case "Soul_Add_2_4":// 
                break;
            case "Soul_Add_2_5":// 
                break;

            case "Soul_Add_3_1":// 
                DoPassive_3_1();
                break;
            case "Soul_Add_3_2":// 
                break;
            case "Soul_Add_3_3":// 
                break;

            case "Soul_Add_4_1":// 
                DoPassive_4_1();
                break;
            case "Soul_Add_4_2":// 
                DoPassive_4_2();
                break;
            case "Soul_Add_4_3":// 
                DoPassive_4_3();
                break;

            case "Soul_Add_5_1":// 
                DoPassive_5_1();
                break;
            case "Soul_Add_5_2":// 
                DoPassive_5_2();
                break;
            case "Soul_Add_5_3":// 
                break;
            case "Soul_Add_5_4":// 
                break;

            case "Soul_Add_6_1":// 
                break;
            case "Soul_Add_6_2":// 
                DoPassive_6_2();
                break;
            case "Soul_Add_6_3":// 
                                //DoPassive_6_3();
                break;
            case "Soul_Add_6_4":// 
                break;
            case "Soul_Add_6_5":// 
                break;

            case "Soul_Add_7_1":// 
                DoPassive_7_1();
                break;
            case "Soul_Add_7_2":// 
                DoPassive_7_2();
                break;
            case "Soul_Add_7_3":// 
                break;
            case "Soul_Add_7_4":// 
                break;
        }
    }
    void RemovePassiveEffect(string itemId)
    {
        switch (itemId)
        {
            case "Soul_Add_1_1":// 
                break;
            case "Soul_Add_1_2":// 
                RemovePassive_1_2();
                break;
            case "Soul_Add_1_3":// 
                break;
            case "Soul_Add_1_4":// 
                break;
            case "Soul_Add_1_5":// 
                break;
            case "Soul_Add_1_6":// 
                break;

            case "Soul_Add_2_1":// 
                break;
            case "Soul_Add_2_2":// 
                break;
            case "Soul_Add_2_3":// 
                break;
            case "Soul_Add_2_4":// 
                break;
            case "Soul_Add_2_5":// 
                break;

            case "Soul_Add_3_1":// 
                //RemovePassive_3_1();
                break;
            case "Soul_Add_3_2":// 
                break;
            case "Soul_Add_3_3":// 
                break;

            case "Soul_Add_4_1":// 
                RemovePassive_4_1();
                break;
            case "Soul_Add_4_2":// 
                RemovePassive_4_2();
                break;
            case "Soul_Add_4_3":// 
                //RemovePassive_4_3();
                break;

            case "Soul_Add_5_1":// 
                RemovePassive_5_1();
                break;
            case "Soul_Add_5_2":// 
                RemovePassive_5_2();
                break;
            case "Soul_Add_5_3":// 
                break;
            case "Soul_Add_5_4":// 
                break;

            case "Soul_Add_6_1":// 
                break;
            case "Soul_Add_6_2":// 
                RemovePassive_6_2();
                break;
            case "Soul_Add_6_3":// 
                                //DoPassive_6_3();
                break;
            case "Soul_Add_6_4":// 
                break;
            case "Soul_Add_6_5":// 
                break;

            case "Soul_Add_7_1":// 
                RemovePassive_7_1();
                break;
            case "Soul_Add_7_2":// 
                RemovePassive_7_2();
                break;
            case "Soul_Add_7_3":// 
                break;
            case "Soul_Add_7_4":// 
                break;
        }
    }
    #region 영혼강화

    private void TryApplyEffect(IPassiveEffect effect)
    {
        if (effect == null) return;
        effect.ApplyEffect();
    }

    private void TryRemoveEffect(IPassiveEffect effect)
    {
        if (effect == null) return;
        effect.RemoveEffect();
    }
    public string GetRandomPassiveByGrade(int grade)
    {
        // 등급별 후보 리스트
        List<string> candidates = new List<string>();
        
        foreach (var item in passiveItems)
        {
            // 조건: 구매 안 됨 + 예약 안 됨 + 등급 일치
            if (!item.isPurchased &&
                !reservedPassiveIds.Contains(item.id) &&
                item.rating == grade)
            {
                candidates.Add(item.id);
            }
        }
        // 후보가 없으면 null 반환
        if (candidates.Count == 0)
        {
            Debug.LogWarning($"등급 {grade}의 구매 가능한 혼령강화가 없습니다!");
            return null;
        }

        // 랜덤 선택
        string selectedId = candidates[Random.Range(0, candidates.Count)];

        // 예약 목록에 추가 (중복 방지)
        reservedPassiveIds.Add(selectedId);

        return selectedId;
    }
    // 미션 등급에 따른 확률적 혼령강화 추첨
    public string GetRandomPassiveForMission(int missionGrade)
    {
        float roll = Random.Range(0f, 100f);
        int targetRating = 1; // 기본값: 하급

        switch (missionGrade)
        {
            case 0: // 하급 미션
                if (roll < 50f) targetRating = 1;      // 50% 하급
                else if (roll < 70f) targetRating = 2; // 20% 중급
                else if (roll < 85f) targetRating = 3; // 15% 상급
                else targetRating = 4;                 // 15% 최상급
                break;

            case 1: // 중급 미션
                if (roll < 30f) targetRating = 1;      // 30% 하급
                else if (roll < 70f) targetRating = 2; // 40% 중급
                else if (roll < 90f) targetRating = 3; // 20% 상급
                else targetRating = 4;                 // 10% 최상급
                break;

            case 2: // 상급 미션
                if (roll < 10f) targetRating = 1;      // 10% 하급
                else if (roll < 40f) targetRating = 2; // 30% 중급
                else if (roll < 80f) targetRating = 3; // 40% 상급
                else targetRating = 4;                 // 20% 최상급
                break;
        }

        return GetRandomPassiveByGrade(targetRating);
    }
    // 혼령강화 구매 확정 (예약 목록에서 제거)
    public void ConfirmPassivePurchase(string passiveId)
    {
        if (string.IsNullOrEmpty(passiveId)) return;

        // 구매 처리
        PurchaseItem(passiveId);

        // 예약 목록에서 제거
        if (reservedPassiveIds.Contains(passiveId))
        {
            reservedPassiveIds.Remove(passiveId);
        }
    }
    //혼령강화 예약 취소 (미션 실패 시)
    public void CancelPassiveReservation(string passiveId)
    {
        if (string.IsNullOrEmpty(passiveId)) return;

        if (reservedPassiveIds.Contains(passiveId))
        {
            reservedPassiveIds.Remove(passiveId);
            Debug.Log($"혼령강화 예약 취소: {passiveId}");
        }
    }

    //모든 예약 초기화 (디버그용)
    public void ClearAllReservations()
    {
        reservedPassiveIds.Clear();
    }

    //천하장사
    public void DoPassive_1_1()
    {
        //var player_item_use = FindObjectOfType<Player_Item_Use>();
        //if (player_item_use)
        //{
        //    // 기존 보너스 제거
        //    GameManager.Instance.playerData.damageMultiplier -= lastBonusDamage;

        //    // 새 보너스 계산
        //    int emptyItemSlotCount = player_item_use.CheckEmptySlotsCount();
        //    lastBonusDamage = 0.1f * emptyItemSlotCount;

        //    // 새 보너스 적용
        //    GameManager.Instance.playerData.damageMultiplier += lastBonusDamage;
        //}
    }

    //정정당당
    public void DoPassive_1_2()
    {
        TryApplyEffect(new IncreaseDamageEffect(GameManager.Instance.playerData, 1f));
        //급소공략 불가능
    }

    public void RemovePassive_1_2()
    {
        TryRemoveEffect(new IncreaseDamageEffect(GameManager.Instance.playerData, 1f));
    }

    //문전박대
    public void DoPassive_2_1()
    {

    }

    //백발백중
    public void DoPassive_2_2()
    {

    }

    //쾌도난마
    public void DoPassive_2_3()
    {

    }

    //금의환향
    public void DoPassive_3_1()
    {
        GameManager.Instance.Soul *= 1.3f;
    }

    //다다익선
    public void DoPassive_3_2()
    {
        float newSpeedMultiplier = Mathf.Clamp(Mathf.FloorToInt(GameManager.Instance.Gold / 200), 0, 3) * 0.1f;
        //Debug.Log("newSpeed: " + newSpeedMultiplier);
        SetPassiveSpeedBonus("Soul_Add_3_2", newSpeedMultiplier);
        //GameManager.Instance.playerData.speedMultiplier += newSpeedMultiplier;
    }

    //금강불괴
    public void DoPassive_4_1()
    {
        TryApplyEffect(new DecreaseDamageTakenEffect(GameManager.Instance.playerData, 0.5f));
    }

    public void RemovePassive_4_1()
    {
        TryRemoveEffect(new DecreaseDamageTakenEffect(GameManager.Instance.playerData, 0.5f));
    }

    //외강내유
    public void DoPassive_4_2()
    {
        TryApplyEffect(new IncreaseMaxHPEffect(GameManager.Instance.playerData, 75));
        TryApplyEffect(new DecreaseMaxSPEffect(GameManager.Instance.playerData, 25));
    }

    public void RemovePassive_4_2()
    {
        TryRemoveEffect(new IncreaseMaxHPEffect(GameManager.Instance.playerData, 75));
        TryRemoveEffect(new DecreaseMaxSPEffect(GameManager.Instance.playerData, 25));
    }

    //외유내강
    public void DoPassive_4_3()
    {
        TryApplyEffect(new IncreaseMaxSPEffect(GameManager.Instance.playerData, 75));
        TryApplyEffect(new DecreaseMaxHPEffect(GameManager.Instance.playerData, 25));
    }

    //가담항설
    public void DoPassive_5_1()
    {
        TryApplyEffect(new ItemFindAbilityOn(GameManager.Instance.playerData));
    }

    public void RemovePassive_5_1()
    {
        TryRemoveEffect(new ItemFindAbilityOn(GameManager.Instance.playerData));
    }

    //취사선택
    public void DoPassive_5_2()
    {
        var player_item_use = FindObjectOfType<Player_Item_Use>();
        if (player_item_use)
        {
            //// 기존 보너스 제거
            //GameManager.Instance.playerData.speedMultiplier -= lastBonusSpeed_5_2;

            //// 새 보너스 계산
            //int emptyItemSlotCount = player_item_use.CheckEmptySlotsCount();
            //lastBonusSpeed_5_2 = 0.1f * emptyItemSlotCount;

            //// 새 보너스 적용
            //GameManager.Instance.playerData.speedMultiplier += lastBonusSpeed_5_2;

            int emptyItemSlotCount = player_item_use.CheckEmptySlotsCount();
            float newBonus = 0.1f * emptyItemSlotCount;
            SetPassiveSpeedBonus("Soul_Add_5_2", newBonus);
        }

    }
    public void RemovePassive_5_2()
    {
        //GameManager.Instance.playerData.speedMultiplier -= lastBonusSpeed_5_2;
        //lastBonusSpeed_5_2 = 0f;
    }
    //등용문
    public void DoPassive_6_1()
    {
        //if (GameManager.Instance.Day >= 4)
        //{
        //    //passive_6_1_count += 1;
        //    //TryApplyEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.1f * passive_6_1_count));
        //}
    }

    public void RemovePassive_6_1()
    {
        passive_6_1_count = 0;
        TryRemoveEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.1f * passive_6_1_count));
    }

    //승승장구
    public void DoPassive_6_2()
    {
        TryApplyEffect(new IncreaseDamageEffect(GameManager.Instance.playerData, 0.1f * GameManager.Instance.killcount));
    }

    public void RemovePassive_6_2()
    {
        TryRemoveEffect(new IncreaseDamageEffect(GameManager.Instance.playerData, 0.1f * GameManager.Instance.killcount));
    }
    //선견지명
    public void DoPassive_6_3()
    {
        var quickSlotItems = GameManager.Instance.currentQuickSlot;
        if (quickSlotItems != null)
        {
            int totalGold = 0;
            foreach (var item in quickSlotItems)
            {
                if (item != null)
                    totalGold += item.Coin;
            }
            GameManager.Instance.Add_Gold(totalGold);
        }
    }

    //구사일생
    public void DoPassive_7_1()
    {
        TryApplyEffect(new ItemSaveAbilityOnRevive(GameManager.Instance.playerData));
    }

    public void RemovePassive_7_1()
    {
        TryRemoveEffect(new ItemSaveAbilityOnRevive(GameManager.Instance.playerData));
    }

    //궁여지책
    public void DoPassive_7_2()
    {
        QuickSlotUI quickslotUI = FindObjectOfType<QuickSlotUI>();
        if (quickslotUI && quickslotUI.angleUnit >= 18)
        {
            TryApplyEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.5f));
        }
    }

    public void RemovePassive_7_2()
    {
        QuickSlotUI quickslotUI = FindObjectOfType<QuickSlotUI>();
        TryRemoveEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.5f));
    }

    public void HandleNextDay()
    {
        //금의환향
        if (HasEffect("Soul_Add_3_1"))
        {
            DoPassive_3_1();
        }

        //등용문
        if (HasEffect("Soul_Add_6_1"))
        {
            TryRemoveEffect(new IncreaseMoveSpeedEffect(GameManager.Instance.playerData, 0.1f * passive_6_1_count));
            passive_6_1_count = 0;
        }

        //선견지명
        if (HasEffect("Soul_Add_6_3"))
        {
            DoPassive_6_3();
        }
    }

    public void HandleSaleItemImmediately()
    {
        //등용문
        if (HasEffect("Soul_Add_6_1"))
        {
            DoPassive_6_1();
        }
    }

    public void HandlePickupItem()
    {
        if (HasEffect("Soul_Add_1_1"))
        {
            DoPassive_1_1();
        }
        if(HasEffect("Soul_Add_5_2"))
        {
            DoPassive_5_2();
        }
    }

    public void HandleDropItem()
    {
        if (HasEffect("Soul_Add_1_1"))
        {
            DoPassive_1_1();
        }
        if (HasEffect("Soul_Add_5_2"))
        {
            DoPassive_5_2();
        }
    }

    public void HadleTimeAngleUnit18()
    {
        if (HasEffect("Soul_Add_7_2"))
        {
            DoPassive_7_2();
        }
    }

    #endregion
}