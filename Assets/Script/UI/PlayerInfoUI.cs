using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerController playerController;
    public Image playerHpBar;
    public Image playerBonusHpBar;
    public Image playerSpBar;
    public Image playerMPsc;
    public Image playerMPSC;// 가장자리 이미지 지정
    public Sprite[] Mp_sc; // 가장자리 이미지 3개

    public TMP_Text coin_text;
    public TMP_Text soul_text;

    private float maxHpBarWidth; // 실제 UI에서의 최대 바 너비
    private float maxSpBarWidth;

    [SerializeField] private RectTransform frameRect;
    [SerializeField] private RectTransform hpBarRect;
    [SerializeField] private RectTransform extraHpRect;

    private const float HP_WIDTH = 288f;
    private const float HP_HEIGHT = 32f;
    private const float TOTAL_WIDTH = 320f;
    private const float TOTAL_HEIGHT = 40f;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.playerData.Init();
    }

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        maxHpBarWidth = playerHpBar.rectTransform.sizeDelta.x;
        maxSpBarWidth = playerSpBar.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        coin_text.text = $"냥: {GameManager.Instance.Gold}";
        soul_text.text = $"혼: {GameManager.Instance.Soul} / {GameManager.Instance.N_Day_Cost}";

        if (playerController == null) // 플레이어가 없는 경우 (상점, 스테이지 선택)
        {
            PlayerData playerData = GameManager.Instance.playerData;

            float hpRatio = GameManager.Instance.playerData.currentHp / GameManager.Instance.playerData.maxHp;
            float spRatio = GameManager.Instance.playerData.currentSp / GameManager.Instance.playerData.maxSp;

            Vector2 hpSize = playerHpBar.rectTransform.sizeDelta;
            hpSize.x = maxHpBarWidth * Mathf.Clamp01(hpRatio);
            playerHpBar.rectTransform.sizeDelta = hpSize;

            Vector2 spSize = playerSpBar.rectTransform.sizeDelta;
            spSize.x = maxSpBarWidth * Mathf.Clamp01(spRatio);
            playerSpBar.rectTransform.sizeDelta = spSize;

            UpdateHealthBar(playerData.currentHp, playerData.maxHp, playerData.currentExtraHp, playerData.extraHp);
        }
        else // 인게임 에서 보여줄것
        {
            float hpRatio = playerController.currentHp / playerController.maxHp;
            float spRatio = playerController.currentSp / playerController.maxSp;

            Vector2 hpSize = playerHpBar.rectTransform.sizeDelta;
            hpSize.x = maxHpBarWidth * Mathf.Clamp01(hpRatio);
            playerHpBar.rectTransform.sizeDelta = hpSize;

            Vector2 spSize = playerSpBar.rectTransform.sizeDelta;
            spSize.x = maxSpBarWidth * Mathf.Clamp01(spRatio);
            playerSpBar.rectTransform.sizeDelta = spSize;

            UpdateHealthBar(playerController.currentHp, playerController.maxHp, playerController.currentExtraHp, playerController.extraHp);
        }
        if (playerMPsc != null && playerController != null)
        {
            float mpRatio = playerController.currentMp / playerController.maxMp;

            // 투명도 조절
            float alpha = Mathf.Lerp(0.1f, 0f, mpRatio);
            Color color = playerMPsc.color;
            color.a = alpha;
            playerMPsc.color = color;

            // 이미지 스프라이트 교체
            if (Mp_sc != null && Mp_sc.Length >= 3)
            {
                Color color_sc = playerMPSC.color;

                if (mpRatio <= 0.25f)
                {
                    playerMPSC.sprite = Mp_sc[2];
                    color_sc.a = 0.5f;
                }
                else if (mpRatio <= 0.5f)
                {
                    playerMPSC.sprite = Mp_sc[1];
                    color_sc.a = 0.25f;
                }
                else if (mpRatio <= 0.75f)
                {
                    playerMPSC.sprite = Mp_sc[0];
                    color_sc.a = 0.125f;
                }
                else
                {
                    playerMPSC.sprite = Mp_sc[0];
                    color_sc.a = 0f;
                }

                playerMPSC.color = color_sc; // 변경된 알파값을 여기서 반영
            }

        }

    }

    public void UpdateHealthBar(float currentHP, float maxHP, float currentExtraHP, float extraHP)
    {
        float totalMaxHP = maxHP + extraHP;
        float totalWidth = HP_WIDTH * (totalMaxHP / maxHP);

        // 기본 체력 바 크기
        float hpWidth = totalWidth * (currentHP / totalMaxHP);
        hpBarRect.sizeDelta = new Vector2(hpWidth, HP_HEIGHT);

        // 추가 체력 바
        if (extraHP > 0)
        {
            float extraWidth = totalWidth * (extraHP / totalMaxHP);
            extraHpRect.sizeDelta = new Vector2(extraWidth, HP_HEIGHT);
            extraHpRect.gameObject.SetActive(true);

            Vector2 anchored = hpBarRect.anchoredPosition;
            anchored.x += hpBarRect.sizeDelta.x;
            extraHpRect.anchoredPosition = new Vector2(anchored.x, hpBarRect.anchoredPosition.y);

            float extraHpRatio = currentExtraHP / extraHP;
            playerBonusHpBar.fillAmount = extraHpRatio;
        }
        else
        {
            extraHpRect.gameObject.SetActive(false);
        }

        float totalFrameWidth = totalWidth + (TOTAL_WIDTH - HP_WIDTH);
        frameRect.sizeDelta = new Vector2(totalFrameWidth, TOTAL_HEIGHT);
    }
}
