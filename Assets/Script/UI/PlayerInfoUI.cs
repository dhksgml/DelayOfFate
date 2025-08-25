using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerController playerController;
    public Image playerHpBar;
    public Image playerBonusHpBar;
    public TMP_Text playerHP_Text;
    public Image playerMPsc;
    public Image playerMPSC;// 가장자리 이미지 지정
    public Sprite[] Mp_sc; // 가장자리 이미지 3개

    public TMP_Text coin_text;
    public TMP_Text soul_text;
    public TMP_Text add_coin_text; //일시적 추가 했을때 ui
    public TMP_Text add_soul_text; //일시적 추가 했을때 ui

    public float showDuration = 1f; // 완전 표시되는 시간
    public float fadeDuration = 1f; // 페이드 아웃 시간

    private float maxHpBarWidth; // 실제 UI에서의 최대 바 너비

    //ItemFinderArrow Item
    [SerializeField] private RectTransform UIArrow;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float hideUIArrowDistance;

    private Coroutine showRoutine;
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
        add_text_reset();
    }

    private void Update()
    {
        coin_text.text = $" : {(int)GameManager.Instance.Gold}";
        soul_text.text = $" : {(int)GameManager.Instance.Soul} / <color=#ff0000>{(int)GameManager.Instance.N_Day_Cost}</color>";
        //playerHP_Text.text = $"{(int)playerController.currentHp} / {(int)playerController.maxHp}";

        if (playerController == null) // 플레이어가 없는 경우 (상점, 스테이지 선택)
        {
            playerHP_Text.text = $"{(int)GameManager.Instance.playerData.currentHp} / {(int)GameManager.Instance.playerData.maxHp}";
        }
        else // 인게임 에서 보여줄것
        {
            playerHP_Text.text = $"{(int)playerController.currentHp} / {(int)playerController.maxHp}";

            float hpRatio = playerController.currentHp / playerController.maxHp;

            Vector2 hpSize = playerHpBar.rectTransform.sizeDelta;
            hpSize.x = maxHpBarWidth * Mathf.Clamp01(hpRatio);
            playerHpBar.rectTransform.sizeDelta = hpSize;

            //UpdateHealthBar(playerController.currentHp, playerController.maxHp, playerController.currentExtraHp, playerController.extraHp);
            //if(PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_5_1"))
            if (GameManager.Instance != null && GameManager.Instance.playerData.isFindNearestItem)
            {
                if (playerController.GetNearestItemDir() != null)
                    ShowDirectionToItem((Vector3)playerController.GetNearestItemDir());
                else
                    HideDirectionToItem();
            }
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

    /*public void UpdateHealthBar(float currentHP, float maxHP, float currentExtraHP, float extraHP)
    {
        float totalMaxHP = maxHP + extraHP;
        float totalWidth = Mathf.Min(HP_WIDTH * (totalMaxHP / maxHP), MAX_BAR_WIDTH);

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
    }*/

    public void ShowDirectionToItem(Vector3 itemWorldPosition)
    {
        Transform playerTransform = playerController.gameObject.transform;
        Vector3 dir = (itemWorldPosition - playerTransform.position);
        float distance = dir.magnitude;

        if (distance < hideUIArrowDistance)
        {
            UIArrow.gameObject.SetActive(false);
        }
        else
        {
            UIArrow.gameObject.SetActive(true);

            dir.Normalize();
            Vector3 offset = dir * distanceFromPlayer;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(playerTransform.position);
            UIArrow.position = screenPos + offset;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            UIArrow.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    public void One_Time_Show(float gold, bool soul, bool add)
    {
        if (gold == 0) return;

        string prefix = add ? "+" : "";
        string text = prefix + gold.ToString();

        if (soul)
            add_soul_text.text = text;
        else
            add_coin_text.text = text;

        showRoutine = StartCoroutine(ShowAndFade());
    }


    private IEnumerator ShowAndFade()
    {
        //bool isShowing = true;
        add_coin_text.alpha = 1f; // 순간적으로 보이기
        add_soul_text.alpha = 1f; // 순간적으로 보이기
        yield return new WaitForSeconds(showDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            add_coin_text.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            add_soul_text.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        add_text_reset();
        GameManager.Instance.ReSet_One_time_SG();
        //isShowing = false;
    }
    void add_text_reset()
    {
        add_coin_text.alpha = 0f;
        add_soul_text.alpha = 0f;
        add_coin_text.text = "";
        add_coin_text.text = "";
    }
    public void HideDirectionToItem()
    {
        UIArrow.gameObject.SetActive(false);
    }
}
