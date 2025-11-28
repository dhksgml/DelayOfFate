using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public float Gold;
    public float Soul;

    private int rerollCost = 30;

    private const int lantern_1 = 500;
    private const int lantern_2 = 1000;

    private List<string> soulNames = new List<string>();
    private List<int> soulPrices = new List<int>();
    private bool[] soulPurchased = new bool[8]; // 영혼 4개 구매 여부

    private List<string> allSoulIds = new List<string>();

    public Image[] soulIcons; // UI에 보여줄 아이콘 4개

    public TMP_Text[] weaponSlots; // 상품 목록들 무기, 영혼, 초롱
    

    [SerializeField] private List<Transform> menuItems; // 버튼 15개
    [SerializeField] private int currentIndex = 0;
    [SerializeField] private Transform selector; // 선택 표시용 오브젝트

    public GameObject ch_soul_gold_bt;//교환 버튼 (비활성화 용)
    public GameObject ch_gold_soul_bt;//교환 버튼 (비활성화 용)
    public GameObject Smoke_Effect; //담배연기 이펙트
    public Vector2 SE_pos;//담배연기 좌표
    public Image speech_bubble_image;//말풍선 이미지
    //public Sprite[] speech_bubble_sprite;//말풍선 이미지
    public TMP_Text speech_bubble_text;//말풍선 텍스트

    private System.Random rand = new System.Random();
    private bool isJokeOnCooldown = false;
    private Coroutine currentBubbleRoutine;
    private string[] buyLines = {"고맙네", "신상품이네", "문제 없는\n물건이네", "오늘은\n매출이 좋구만", "신상품이지"};//구매
    private string[] tradeLines = {"고맙네","후원 고맙네", "사고 싶은\n물건이라도?" };//교환
    private string[] rerollLines = {"여기 새 품목들이네", "이 물건 맞나?", "이걸 찾나?" };//리롤
    private string[] notEnoughLines = { "이건 자선사업이\n아니네", "냥이 없나?", "냥이 부족한거\n같네만?", "공짜는 안되네"};//부족
    private string[] jokeLines = {"이 곰방대는 안파네", "뭐라도 하나\n사지 그러나", "(한숨)", "자네도 삿갓을\n좋아하나?", "천천히 둘러보게나", "몸은 괜찮나?", "안전이 최고지", "흠...", "또 악귀들이\n기승인가?", "좋아하는 색이 있나?" };//농담
    private string[] noWeaponLines = { "어이, \n무기는 가져가야지!", "무기를 까먹은거 같네만?" };
    private string itemId = "Soul_Add_8_1";
    private string[] soul_num = new string[8];//영혼 설명용 임시 지역 변수
    private PassiveItemUI passiveItemUI;
    private PassiveItemManager passiveItemManager;
    private Stage_Manager stage_Manager;
    void Awake()
    {
        passiveItemUI = FindObjectOfType<PassiveItemUI>();
        passiveItemManager = FindObjectOfType<PassiveItemManager>();
        stage_Manager = FindObjectOfType<Stage_Manager>();
        allSoulIds.Clear();

        // Build base list: groups 1..7, numbers 1..2
        for (int n = 1; n <= 4; n++)
        {
            allSoulIds.Add($"Soul_Add_{1}_{n}"); // 천하장사 정정당당 속전속결 기고만장
        }
        for (int n = 1; n <= 3; n++)
        {
            allSoulIds.Add($"Soul_Add_{2}_{n}"); // 문전박대 백발백중 쾌도난마
        }
        for (int n = 1; n <= 3; n++)
        {
            allSoulIds.Add($"Soul_Add_{3}_{n}"); // 금의환향 다다익선 일확천금
        }
        for (int n = 1; n <= 3; n++)
        {
            allSoulIds.Add($"Soul_Add_{4}_{n}"); // 금강불괴 외강내유 외유내강
        }
        for (int n = 1; n <= 3; n++)
        {
            allSoulIds.Add($"Soul_Add_{5}_{n}"); // 가담항설 취사선택 경화수월
        }
        for (int n = 1; n <= 4; n++)
        {
            allSoulIds.Add($"Soul_Add_{6}_{n}"); //등용문 승승장구 선견지명 일취월장
        }
        for (int n = 1; n <= 4; n++)
        {
            allSoulIds.Add($"Soul_Add_{7}_{n}"); // 구사일생 궁여지책 무아지경 배수진
        }

        RerollSouls();
    }


    void Start()
    {
        InitializeShop();
        if (GameManager.Instance.Day == 1) ch_bt_1day_no(); //1일차면 교환 막기 + 환도 강제 구매
        passiveItemManager = FindObjectOfType<PassiveItemManager>();
        // 말풍선초기화
        speech_bubble_image.gameObject.SetActive(false);
        speech_bubble_text.text = "";
        speech_bubble_on("농담");
        StartCoroutine(SpawnSmokeLoop()); //담배
    }
    void Update()
    {
        Gold = GameManager.Instance.Gold;
        Soul = GameManager.Instance.Soul;

        Shop_Key_col();
    }
    void Shop_Key_col()
    {
        int previousIndex = currentIndex; // 이전 인덱스 저장

        // ============================================
        // 현재 줄과 해당 줄의 시작/끝 인덱스 계산
        // 1줄(0~3): 4개, 2줄(4~7): 4개, 3줄(8~12): 5개
        // 3줄 배치 순서 이전, 리롤, 전투, 교환, 교환 
        // ============================================
        int currentRow = GetRowFromIndex(currentIndex);
        int rowStart = GetRowStartIndex(currentRow);
        int rowEnd = GetRowEndIndex(currentRow);
        int rowLength = rowEnd - rowStart + 1;

        // 위로 이동
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentRow == 0) // 1줄에서 위로
            {
                // 3줄로 이동 (같은 열 유지, 3줄이 더 길면 끝으로)
                int columnInRow = currentIndex - rowStart;
                currentIndex = 8 + columnInRow; // 3줄 시작(8) + 열 위치
                if (currentIndex > 12) currentIndex = 12; // 3줄 끝
            }
            else if (currentRow == 1) // 2줄에서 위로
            {
                // 1줄로 이동
                int columnInRow = currentIndex - rowStart;
                currentIndex = 0 + columnInRow; // 1줄 시작(0) + 열 위치
            }
            else if (currentRow == 2) // 3줄에서 위로
            {
                // 2줄로 이동 (3줄이 5개라 4번째부터는 2줄 끝으로)
                int columnInRow = currentIndex - rowStart;
                currentIndex = 4 + columnInRow; // 2줄 시작(4) + 열 위치
                if (currentIndex > 7) currentIndex = 7; // 2줄 끝
            }
        }

        // 아래로 이동
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentRow == 0) // 1줄에서 아래로
            {
                // 2줄로 이동
                int columnInRow = currentIndex - rowStart;
                currentIndex = 4 + columnInRow; // 2줄 시작(4) + 열 위치
            }
            else if (currentRow == 1) // 2줄에서 아래로
            {
                // 3줄로 이동 (같은 열 유지)
                int columnInRow = currentIndex - rowStart;
                currentIndex = 8 + columnInRow; // 3줄 시작(8) + 열 위치
            }
            else if (currentRow == 2) // 3줄에서 아래로
            {
                // 1줄로 이동 (3줄이 5개라 4번째부터는 1줄 끝으로)
                int columnInRow = currentIndex - rowStart;
                currentIndex = 0 + columnInRow; // 1줄 시작(0) + 열 위치
                if (currentIndex > 3) currentIndex = 3; // 1줄 끝
            }
        }

        // 왼쪽 이동
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex--;
            if (currentIndex < rowStart) currentIndex = rowEnd; // 현재 줄의 끝으로
        }

        // 오른쪽 이동
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex++;
            if (currentIndex > rowEnd) currentIndex = rowStart; // 현재 줄의 시작으로
        }

        // 11, 12번은 1일차에는 접근 불가 (3줄 4, 5번째)
        if (GameManager.Instance.Day == 1)
        {
            if (currentIndex == 11 || currentIndex == 12)
            {
                if (previousIndex < currentIndex) // 왼쪽에서 오른쪽으로 or 위에서 아래로
                    currentIndex = previousIndex; // 원래 위치로 복귀
                else // 오른쪽에서 왼쪽으로 or 아래에서 위로
                    currentIndex = 10;
            }
        }

        // UI 업데이트
        UpdateSelectorPosition();

        // ===== itemId 설정 =====
        // 1줄(0~3): soul_num[0~3]
        // 2줄(4~7): soul_num[4~7]
        // 3줄(8~12): Soul_Add_10_1 ~ Soul_Add_10_5
        if (currentIndex >= 0 && currentIndex <= 7)
        {
            itemId = soul_num[currentIndex];
        }
        else if (currentIndex >= 8 && currentIndex <= 12)
        {
            int soulAddIndex = currentIndex - 8 + 1; // 8→1, 9→2, 10→3, 11→4, 12→5
            itemId = $"Soul_Add_10_{soulAddIndex+1}";
        }
        else
        {
            itemId = "";
        }

        var item = PassiveItemManager.Instance.passiveItems.Find(i => i.id == itemId);
        if (item != null)
        {
            passiveItemUI.Show(item.itemName, item.description, item.rating);
        }

        // 선택 실행
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ExecuteOption(currentIndex);
        }
    }
    int GetRowFromIndex(int index)// 0~3: 0줄, 4~7: 1줄, 8~12: 2줄
    {
        if (index >= 0 && index <= 3) return 0; // 1줄
        if (index >= 4 && index <= 7) return 1; // 2줄
        if (index >= 8 && index <= 12) return 2; // 3줄
        return 0;
    }
    int GetRowStartIndex(int row)// Helper: 줄의 시작 인덱스
    {
        switch (row)
        {
            case 0: return 0;  // 1줄 시작
            case 1: return 4;  // 2줄 시작
            case 2: return 8;  // 3줄 시작
            default: return 0;
        }
    }
    int GetRowEndIndex(int row)// Helper: 줄의 끝 인덱스
    {
        switch (row)
        {
            case 0: return 3;  // 1줄 끝 (4개)
            case 1: return 7;  // 2줄 끝 (4개)
            case 2: return 12; // 3줄 끝 (5개)
            default: return 0;
        }
    }


    private void UpdateSelectorPosition()
    {
        if (selector != null && menuItems.Count > 0)
        {
            selector.position = menuItems[currentIndex].position;
        }
    }

    private void ExecuteOption(int index)
    {
        //var itemId = "Soul_Add_4_3";
        //var item = PassiveItemManager.Instance.passiveItems.Find(i => i.id == itemId);
        Debug.Log("선택된 아이템: " + index);
        if (index >= 0 && index <= 7)
        {
            BuySoul(index);
        }
        if (index == 8) { stage_Manager.Weapon_ch(); }
        if (index == 9) { Reroll(); } 
        if (index == 10) { stage_Manager.Battle_ch(); }
        if (index == 11) { Soul_c_Gold(); }
        if (index == 12) { Gold_c_Soul(); }
        //passiveItemUI.Show(item.itemName, item.description, item.rating);

        // index 기준으로 상점 로직 실행 (예: 구매, 설명창 열기 등)
    }
    void InitializeShop()
    {
        for (int i = 0; i < 8; i++)
        {
            weaponSlots_text(i, 0, "Gold");
        }

        weaponSlots_text(8, rerollCost, "Soul"); // 리롤 가격

        // 영혼 구매 상태 초기화
        soulPurchased = new bool[8];
        RerollSouls();
    }
    void weaponSlots_text(int Slot,int coin,string name)
    {
        weaponSlots[Slot].text = (coin).ToString();
        if (name == "Soul")
        {
            weaponSlots[Slot].text += "<sprite=8> ";
        }
        else if (name == "Gold")
        {
            weaponSlots[Slot].text += "<sprite=9> ";
        }
    }


    public void UpdateWeaponPrice()
    {
        if (PassiveItemManager.Instance == null) return;

        for (int i = 0; i < 5; i++)
        {
            //weaponSlots_text(i, 30 + (GameManager.Instance.Day * 30), "Gold"); 무기가격
        }
    }
    public void OnItemHover(int i, ItemData item)
    {
        QuickSlotUI quickSlotUI = FindObjectOfType<QuickSlotUI>();
        if (quickSlotUI != null)
        {
            quickSlotUI.DisplayItemInfo(i, item);
        }
    }
    // Shop.cs의 BuySoul 메서드 수정
    public void BuySoul(int index)
    {
        if (soulPurchased[index]) return;

        int price = soulPrices[index];
        if (Gold >= price)
        {
            GameManager.Instance.Sub_Gold(price);
            soulPurchased[index] = true;

            weaponSlots[index].text = "구매 완료";
            speech_bubble_on("구매");
            Button btn = weaponSlots[index].GetComponentInParent<Button>();
            Soul_in_text slot = soulIcons[index].GetComponentInParent<Soul_in_text>();
            if (btn != null)
            {
                btn.interactable = false;
                slot.show = false;
            }
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_2"));

            // 구매 효과 적용 신호 보내기
            string itemId = soulNames[index];
            PassiveItemManager.Instance.ConfirmPassivePurchase(itemId);

            // ★★★ 추가: 같은 종류 2개 보유 확인 후 상점 비활성화 ★★★
            CheckAndDisableSameTypeSouls(itemId);

            UpdateWeaponPrice();
        }
        else
        {
            speech_bubble_on("부족");
        }
    }

    private void CheckAndDisableSameTypeSouls(string purchasedItemId)
    {
        // 구매한 아이템의 그룹 추출 (예: "Soul_Add_2_3" → 그룹 2)
        string[] parts = purchasedItemId.Split('_');
        if (parts.Length < 4) return;

        int purchasedGroup = int.Parse(parts[2]);

        // 플레이어가 같은 그룹의 혼령강화를 몇 개 보유했는지 확인
        int ownedCount = PassiveItemManager.Instance.GetOwnedCountByGroup(purchasedGroup);

        // 2개 이상 보유했다면 상점에서 같은 그룹의 모든 아이템 비활성화
        if (ownedCount >= 2)
        {
            for (int i = 0; i < 8; i++)
            {
                if (soulPurchased[i]) continue; // 이미 구매된 건 스킵

                string shopItemId = soulNames[i];
                if (string.IsNullOrEmpty(shopItemId)) continue;

                string[] shopParts = shopItemId.Split('_');
                if (shopParts.Length < 4) continue;

                int shopGroup = int.Parse(shopParts[2]);

                // 같은 그룹이면 비활성화
                if (shopGroup == purchasedGroup)
                {
                    soulPurchased[i] = true;
                    weaponSlots[i].text = "구매 불가";

                    Button btn = weaponSlots[i].GetComponentInParent<Button>();
                    Soul_in_text slot = soulIcons[i].GetComponentInParent<Soul_in_text>();

                    if (btn != null)
                    {
                        btn.interactable = false;
                        if (slot != null) slot.show = false;
                    }
                }
            }

            Debug.Log($"[상점] 그룹 {purchasedGroup} 혼령강화 2개 보유 → 상점 내 같은 그룹 비활성화");
        }
    }
    /*public void BuyLantern() // 호롱 업글
    {
        int F_leval = GameManager.Instance.playerData.flashLightLevel;
        if (F_leval >= 2)
        {
            Debug.Log("Lantern cannot be purchased anymore.");
            return;
        }

        int price = 0;

        if (F_leval == 0)
        {
            price = lantern_1; // 2단계
        }
        else if (F_leval == 1)
        {
            price = lantern_2; // 3단계
        }

        if (Soul >= price)
        {
            GameManager.Instance.Sub_Soul(price);
            GameManager.Instance.playerData.flashLightLevel = Mathf.Clamp(GameManager.Instance.playerData.flashLightLevel + 1, 1, 3);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_2"));
            // 다음 단계 가격 표시 또는 "구매 완료"
            if (F_leval == 2)
            {
                weaponSlots[9].text = "구매 완료";
                speech_bubble_on("구매");
            }
            else
            {
                int nextPrice = (F_leval == 1) ? lantern_2 : 0;
                weaponSlots_text(8, nextPrice, "Soul");
            }

            Debug.Log($"Purchased lantern ({F_leval}/2) for {price} coins.");
        }
        else
        {
            speech_bubble_on("부족");
        }
    }*/
    public void RerollSouls()
    {
        if (soulNames.Count < 8)
        {
            soulNames.Clear();
            soulPrices.Clear();
            for (int i = 0; i < 8; i++)
            {
                soulNames.Add("");
                soulPrices.Add(0);
            }
        }

        // 리롤 시작 전에 상점 예약 초기화
        PassiveItemManager.Instance.ClearAllReservations();

        List<string> selectedSouls = new List<string>();

        for (int i = 0; i < 8; i++)
        {
            if (soulPurchased[i]) continue;

            string passiveId = null;
            int attempts = 0;
            int maxAttempts = 100;

            // 선택 가능한 혼령강화를 찾을 때까지 반복
            while (string.IsNullOrEmpty(passiveId) && attempts < maxAttempts)
            {
                // 일차별 확률로 등급 결정
                int targetRating = PassiveItemManager.Instance.GetRatingByCurrentDay(); // rating → targetRating으로 변경

                // 해당 등급에서 선택 시도
                passiveId = PassiveItemManager.Instance.GetRandomPassiveByGrade(targetRating); // 인수 1개만 전달

                attempts++;
            }

            // 최대 시도 횟수 초과 시
            if (string.IsNullOrEmpty(passiveId))
            {
                Debug.LogWarning($"슬롯 {i}: 모든 혼령강화가 소진되었습니다.");
                soulNames[i] = "";
                soulPrices[i] = 0;
                weaponSlots_text(i, 0, "Gold");
                if (soulIcons[i] != null) soulIcons[i].sprite = null;
                continue;
            }

            selectedSouls.Add(passiveId);
            soulNames[i] = passiveId;

            PassiveItemData itemData = passiveItemManager.passiveItems.Find(x => x.id == passiveId);
            int itemRating = itemData != null ? itemData.rating : 1; // rating → itemRating으로 변경

            // 등급별 가격 책정
            switch (itemRating) // itemRating 사용
            {
                case 1:
                    soulPrices[i] = 200 + Random.Range(-10, +11);
                    break;
                case 2:
                    soulPrices[i] = 360 + Random.Range(-15, +16);
                    break;
                case 3:
                    soulPrices[i] = 500 + Random.Range(-20, +21);
                    break;
                case 4:
                    soulPrices[i] = 750 + Random.Range(-25, +26);
                    break;
                default:
                    soulPrices[i] = 200;
                    break;
            }

            weaponSlots_text(i, soulPrices[i], "Gold");
            SetSoulIcon(i, passiveId);

            Soul_in_text slot = soulIcons[i].GetComponentInParent<Soul_in_text>();
            if (slot != null)
            {
                slot.itemId = passiveId;
            }
        }

        Debug.Log($"[상점 리롤] Day {GameManager.Instance.Day}");
        Debug.Log($"[상점 리롤] 총 선택: {selectedSouls.Count}개");
        Debug.Log($"[상점 리롤] 등급 1: {selectedSouls.Count(x => passiveItemManager.passiveItems.Find(i => i.id == x)?.rating == 1)}개");
        Debug.Log($"[상점 리롤] 등급 2: {selectedSouls.Count(x => passiveItemManager.passiveItems.Find(i => i.id == x)?.rating == 2)}개");
        Debug.Log($"[상점 리롤] 등급 3: {selectedSouls.Count(x => passiveItemManager.passiveItems.Find(i => i.id == x)?.rating == 3)}개");
        Debug.Log($"[상점 리롤] 등급 4: {selectedSouls.Count(x => passiveItemManager.passiveItems.Find(i => i.id == x)?.rating == 4)}개");
    }
    void SetSoulIcon(int slotIndex, string id)
    {
        // 예: "Soul_Add_2_3" → group = 2, num = 3
        string[] parts = id.Split('_');
        int group = int.Parse(parts[2]); // 1~7
        int number = int.Parse(parts[3]); // 1~3
        soul_num[slotIndex] = $"Soul_Add_{group}_{number}";
        Sprite icon = passiveItemManager.GetIcon(group, number);
        if (soulIcons[slotIndex] != null)
            soulIcons[slotIndex].sprite = icon;
    }

    public void Soul_c_Gold() // 100 혼 → 50 전
    {
        if (Soul >= 100f)
        {
            GameManager.Instance.Sub_Soul(100);
            GameManager.Instance.Add_Gold(50);
            speech_bubble_on("교환");
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else
        {
            speech_bubble_on("부족");
        }
    }

    public void Gold_c_Soul() // 100 전 → 50 혼
    {
        if (Gold >= 100f)
        {
            GameManager.Instance.Sub_Gold(100);
            GameManager.Instance.Add_Soul(50);
            speech_bubble_on("교환");
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else
        {
            speech_bubble_on("부족");
        }
    }
    private IEnumerator SpawnSmokeLoop()
    {
        while (true)
        {
            // 생성
            Instantiate(Smoke_Effect, SE_pos, Quaternion.identity, this.transform);

            // 대기 시간 (2~4초 랜덤)
            float delay = Random.Range(2f, 4f);
            yield return new WaitForSeconds(delay);
        }
    }
    public void speech_bubble_on(string text_t)
    {
        string line = "";

        if (text_t == "구매")
        {
            line = GetRandomLine(buyLines);
            if (!isJokeOnCooldown) StartCoroutine(JokeCooldown());
        }
        else if (text_t == "교환")
        {
            line = GetRandomLine(tradeLines);
            if (!isJokeOnCooldown) StartCoroutine(JokeCooldown());
        }
        else if (text_t == "리롤")
        {
            line = GetRandomLine(rerollLines);
            if (!isJokeOnCooldown) StartCoroutine(JokeCooldown());
        }
        else if (text_t == "부족")
        {
            line = GetRandomLine(notEnoughLines);
            if (!isJokeOnCooldown) StartCoroutine(JokeCooldown());
        }
        else if (text_t == "농담")
        {
            if (isJokeOnCooldown) return;
            line = GetRandomLine(jokeLines);
            StartCoroutine(JokeCooldown());
        }
        else if(text_t == "무기")
        {
            line = GetRandomLine(noWeaponLines);
            if (!isJokeOnCooldown) StartCoroutine(JokeCooldown());
        }

        if (!string.IsNullOrEmpty(line))
        {
            // 이전 코루틴 정리
            if (currentBubbleRoutine != null)
                StopCoroutine(currentBubbleRoutine);

            currentBubbleRoutine = StartCoroutine(ShowSpeechBubble(line));
        }
    }

    private IEnumerator ShowSpeechBubble(string text)
    {
        speech_bubble_image.gameObject.SetActive(true);
        speech_bubble_text.text = text;

        Color imgColor = speech_bubble_image.color;
        Color txtColor = speech_bubble_text.color;

        // 처음엔 완전히 투명
        imgColor.a = 0f;
        txtColor.a = 0f;
        speech_bubble_image.color = imgColor;
        speech_bubble_text.color = txtColor;

        // 0.25초 동안 페이드 인
        float fadeInTime = 0.25f;
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            imgColor.a = alpha;
            txtColor.a = alpha;
            speech_bubble_image.color = imgColor;
            speech_bubble_text.color = txtColor;
            yield return null;
        }

        // 3초 동안 유지 (알파=1 확실히 고정)
        imgColor.a = 1f;
        txtColor.a = 1f;
        speech_bubble_image.color = imgColor;
        speech_bubble_text.color = txtColor;
        yield return new WaitForSeconds(3f);

        // 1초 동안 페이드 아웃
        float fadeOutTime = 1f;
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            imgColor.a = alpha;
            txtColor.a = alpha;
            speech_bubble_image.color = imgColor;
            speech_bubble_text.color = txtColor;
            yield return null;
        }

        // 초기화
        speech_bubble_image.gameObject.SetActive(false);
        speech_bubble_text.text = "";
        currentBubbleRoutine = null;
    }

    private IEnumerator JokeCooldown()
    {
        isJokeOnCooldown = true;

        // 10~15초 랜덤 쿨타임
        float cooldown = UnityEngine.Random.Range(10f, 15f);
        yield return new WaitForSeconds(cooldown);

        isJokeOnCooldown = false;

        // 자동 농담 실행
        speech_bubble_on("농담");
    }

    private string GetRandomLine(string[] lines)
    {
        if (lines.Length == 0) return "";
        return lines[rand.Next(lines.Length)];
    }

    void ch_bt_1day_no()//1일차에 버튼 비활성화
    {
        ch_soul_gold_bt.gameObject.SetActive(false);
        ch_gold_soul_bt.gameObject.SetActive(false);
    }

    public void Reroll()
    {
        if (Soul >= rerollCost)
        {
            GameManager.Instance.Sub_Soul(rerollCost);
            rerollCost += 30;
            weaponSlots_text(8, rerollCost, "Soul");
            RerollSouls();
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
            speech_bubble_on("리롤");
        }
        else
        {
            speech_bubble_on("부족");
        }
    }
}
