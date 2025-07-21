using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerController playerController;
    public Image playerHpBar;
    public Image playerSpBar;
    public Image playerMPsc;
    public Image playerMPSC;// 가장자리 이미지 지정
    public Sprite[] Mp_sc; // 가장자리 이미지 3개

    public TMP_Text coin_text;
    public TMP_Text soul_text;

    private float maxHpBarWidth; // 실제 UI에서의 최대 바 너비
    private float maxSpBarWidth;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        maxHpBarWidth = playerHpBar.rectTransform.sizeDelta.x;
        maxSpBarWidth = playerSpBar.rectTransform.sizeDelta.x;
    }

    private void Update()
    {

        if (playerController == null) // 플레이어가 없는 경우 (상점, 스테이지 선택)
        {
            coin_text.text = $"냥: {GameManager.Instance.Gold}";
            soul_text.text = $"혼: {GameManager.Instance.Soul} / {GameManager.Instance.N_Day_Cost}";

            float hpRatio = GameManager.Instance.playerData.currentHp / GameManager.Instance.playerData.maxHp;
            float spRatio = GameManager.Instance.playerData.currentSp / GameManager.Instance.playerData.maxSp;

            Vector2 hpSize = playerHpBar.rectTransform.sizeDelta;
            hpSize.x = maxHpBarWidth * Mathf.Clamp01(hpRatio);
            playerHpBar.rectTransform.sizeDelta = hpSize;

            Vector2 spSize = playerSpBar.rectTransform.sizeDelta;
            spSize.x = maxSpBarWidth * Mathf.Clamp01(spRatio);
            playerSpBar.rectTransform.sizeDelta = spSize;
        }
        else // 인게임 에서 보여줄것
        {
            coin_text.text = $"냥: {GameManager.Instance.Gold}";
            soul_text.text = $"혼: {GameManager.Instance.Soul}";

            float hpRatio = playerController.currentHp / playerController.maxHp;
            float spRatio = playerController.currentSp / playerController.maxSp;

            Vector2 hpSize = playerHpBar.rectTransform.sizeDelta;
            hpSize.x = maxHpBarWidth * Mathf.Clamp01(hpRatio);
            playerHpBar.rectTransform.sizeDelta = hpSize;

            Vector2 spSize = playerSpBar.rectTransform.sizeDelta;
            spSize.x = maxSpBarWidth * Mathf.Clamp01(spRatio);
            playerSpBar.rectTransform.sizeDelta = spSize;
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
}
