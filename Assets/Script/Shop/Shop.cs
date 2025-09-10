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

    private List<int> weaponPrices = new List<int>();

    private List<string> soulNames = new List<string>();
    private List<int> soulPrices = new List<int>();
    private bool[] soulPurchased = new bool[4]; // 영혼 4개 구매 여부

    private List<string> allSoulIds = new List<string>();

    public Image[] soulIcons; // UI에 보여줄 아이콘 4개

    public TMP_Text[] weaponSlots; // 상품 목록들 무기, 영혼, 초롱
    public ItemData[] weaponData; // 무기 데이터
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
    private string[] notEnoughLines = { "이건 자선사업이\n아니네", "냥이 없나?", "냥이 부족한거\n같네만?", "공짜는 안되네" };//부족
    private string[] jokeLines = {"이 곰방대는 안파네", "뭐라도 하나\n사지 그러나", "(한숨)", "자네도 삿갓을\n좋아하나?", "천천히 둘러보게나", "몸은 괜찮나?", "안전이 최고지", "흠...", "또 악귀들이\n기승인가?", "좋아하는 색이 있나?" };//농담
    private PassiveItemManager passiveItemManager;
    void Awake()
    {
        passiveItemManager = FindObjectOfType<PassiveItemManager>();

        allSoulIds.Clear();

        // Build base list: groups 1..7, numbers 1..2
        for (int g = 1; g <= 7; g++)
        {
            // Skip all group 2 (refining, temporary)
            if (g == 2) continue;

            for (int n = 1; n <= 2; n++)
            {
                // Skip 6_1 only (temporary)
                if (g == 6 && n == 1) continue;

                allSoulIds.Add($"Soul_Add_{g}_{n}");
            }
        }

        // Manually add known _3 variants (except 2_3 which we are excluding)
        allSoulIds.Add("Soul_Add_4_3");
        allSoulIds.Add("Soul_Add_6_3"); // allowed; only 6_1 is excluded
        
        RerollSouls(); // run before Start
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
    }
    void InitializeShop()
    {
        weaponPrices.Clear();
        for (int i = 0; i < 5; i++)
        {
            weaponPrices.Add(GameManager.Instance.Day * 100);
            if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_3_2")) //다다익선 보유시
            {
                weaponSlots_text(i, 0, "Gold");
            }
            else
            {
                weaponSlots_text(i, 30 + (GameManager.Instance.Day * 30), "Gold");
            }
        }

        weaponSlots_text(9, lantern_1, "Soul"); // 초롱가격
        weaponSlots_text(10, rerollCost, "Soul"); // 리롤 가격

        // 영혼 구매 상태 초기화
        soulPurchased = new bool[4];
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
    public void BuyWeapon(int index) // 무기 구매
    {
        if (index < 0 || index >= 5) return;

        int price = weaponPrices[index];

        if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_3_2"))
            price = 0;

        if (Gold < price) 
        { 
            speech_bubble_on("부족");
            return;
        }

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
            return;
        }

        // 다다익선 효과가 없을 때만 소울 차감
        bool hasSoulAddEffect = PassiveItemManager.Instance != null &&
                                PassiveItemManager.Instance.HasEffect("Soul_Add_3_2");
        if (!hasSoulAddEffect)
        {
            GameManager.Instance.Sub_Gold(price);
        }

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
    }

    public void UpdateWeaponPrice()
    {
        if (PassiveItemManager.Instance == null) return;

        for (int i = 0; i < 5; i++)
        {
            if (PassiveItemManager.Instance.HasEffect("Soul_Add_3_2")) //다다익선 보유시
            {
                weaponSlots_text(i, 0, "Gold");
            }
            else
            {
                weaponSlots_text(i, 30 + (GameManager.Instance.Day * 30), "Gold");
            }
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
    public void BuySoul(int index)
    {
        if (soulPurchased[index]) return;

        int price = soulPrices[index];
        if (Gold >= price)
        {
            GameManager.Instance.Sub_Gold(price);
            soulPurchased[index] = true;

            weaponSlots[index + 5].text = "구매 완료";
            speech_bubble_on("구매");
            Button btn = weaponSlots[index + 5].GetComponentInParent<Button>();
            Soul_in_text slot = soulIcons[index].GetComponentInParent<Soul_in_text>();
            if (btn != null)
            {
                btn.interactable = false;
                slot.show = false; // 구매 완료 한건 살펴 보기 해도 안보이고 인벤토리 가서 봐야함
            }
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_2"));
            // 구매 효과 적용 신호 보내기
            string itemId = soulNames[index]; // ← 이미 RerollSouls()에서 할당됨
            PassiveItemManager.Instance.PurchaseItem(itemId);
            UpdateWeaponPrice();
        }
        else
        {
            speech_bubble_on("부족");
        }
    }
    public void BuyLantern() // 호롱 업글
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
                weaponSlots_text(9, nextPrice, "Soul");
            }

            Debug.Log($"Purchased lantern ({F_leval}/2) for {price} coins.");
        }
        else
        {
            speech_bubble_on("부족");
        }
    }


    public void RerollSouls()
    {
        // 리스트 초기화 보장
        if (soulNames.Count < 4)
        {
            soulNames.Clear();
            soulPrices.Clear();
            for (int i = 0; i < 4; i++)
            {
                soulNames.Add("");
                soulPrices.Add(0);
            }
        }

        // 1. 후보군 만들기 (미구매 아이템만)
        List<string> availableSouls = new List<string>();
        foreach (var id in allSoulIds)
        {
            if (!PassiveItemManager.Instance.IsPurchased(id)) // 구매 안 한 것만
                availableSouls.Add(id);
        }

        // 2. 랜덤 섞기 & 4개 추출 (중복 제거)
        availableSouls = availableSouls.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < 4; i++)
        {
            if (soulPurchased[i]) continue;

            if (i >= availableSouls.Count)
            {
                Debug.LogWarning("미구매 아이템이 4개 미만입니다!");
                soulNames[i] = "";
                soulPrices[i] = 0;
                weaponSlots_text(5 + i, 0, "Gold");
                soulIcons[i].sprite = null;
                continue;
            }

            string id = availableSouls[i];
            soulNames[i] = id;
            PassiveItemData itemData = passiveItemManager.passiveItems.Find(x => x.id == id);
            int rating = itemData != null ? itemData.rating : 1; // 기본값은 1
            switch (rating)
            {
                case 1:
                    soulPrices[i] = 150;
                    soulPrices[i] += Random.Range(-10, +11);
                    break;
                case 2:
                    soulPrices[i] = 215;
                    soulPrices[i] += Random.Range(-15, +16);
                    break;
                case 3:
                    soulPrices[i] = 300;
                    soulPrices[i] += Random.Range(-20, +21);
                    break;
                case 4:
                    soulPrices[i] = 400;
                    soulPrices[i] += Random.Range(-25, +26);
                    break;
                default:
                    break;
            }
            // UI 텍스트 갱신
            weaponSlots_text(5 + i, soulPrices[i], "Gold");

            // 아이콘 갱신
            SetSoulIcon(i, id);

            // 슬롯에 있는 ShopSlot 컴포넌트에 itemId 전달
            Soul_in_text slot = soulIcons[i].GetComponentInParent<Soul_in_text>();
            if (slot != null)
            {
                slot.itemId = id;
            }
        }
    }

    void SetSoulIcon(int slotIndex, string id)
    {
        // 예: "Soul_Add_2_3" → group = 2, num = 3
        string[] parts = id.Split('_');
        int group = int.Parse(parts[2]); // 1~7
        int number = int.Parse(parts[3]); // 1~3

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

    public void Goul_c_Soul() // 100 전 → 50 혼
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
            weaponSlots_text(10, rerollCost, "Soul");
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
