using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerController playerController;
    public Image playerHpBar;
    public Image playerSpBar;
    public Image playerMPsc;
    public Image playerMPSC;// �����ڸ� �̹��� ����
    public Sprite[] Mp_sc; // �����ڸ� �̹��� 3��

    public TMP_Text coin_text;
    public TMP_Text soul_text;

    private float maxHpBarWidth; // ���� UI������ �ִ� �� �ʺ�
    private float maxSpBarWidth;

<<<<<<< HEAD
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

=======
>>>>>>> parent of 83c7234 (0721 HP증가 시, UI에도 적용)
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        maxHpBarWidth = playerHpBar.rectTransform.sizeDelta.x;
        maxSpBarWidth = playerSpBar.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
<<<<<<< HEAD
        coin_text.text = $"��: {GameManager.Instance.Gold}";
        soul_text.text = $"ȥ: {GameManager.Instance.Soul} / {GameManager.Instance.N_Day_Cost}";
        if (playerController == null) // �÷��̾ ���� ��� (����, �������� ����)
        {

            if (playerController == null) // �÷��̾ ���� ��� (����, �������� ����)
=======

        if (playerController == null) // �÷��̾ ���� ��� (����, �������� ����)
        {
            coin_text.text = $"��: {GameManager.Instance.Gold}";
            soul_text.text = $"ȥ: {GameManager.Instance.Soul} / {GameManager.Instance.N_Day_Cost}";

            float hpRatio = GameManager.Instance.playerData.currentHp / GameManager.Instance.playerData.maxHp;
            float spRatio = GameManager.Instance.playerData.currentSp / GameManager.Instance.playerData.maxSp;

            Vector2 hpSize = playerHpBar.rectTransform.sizeDelta;
            hpSize.x = maxHpBarWidth * Mathf.Clamp01(hpRatio);
            playerHpBar.rectTransform.sizeDelta = hpSize;

            Vector2 spSize = playerSpBar.rectTransform.sizeDelta;
            spSize.x = maxSpBarWidth * Mathf.Clamp01(spRatio);
            playerSpBar.rectTransform.sizeDelta = spSize;
        }
        else // �ΰ��� ���� �����ٰ�
        {
            coin_text.text = $"��: {GameManager.Instance.Gold}";
            soul_text.text = $"ȥ: {GameManager.Instance.Soul}";

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

            // ������ ����
            float alpha = Mathf.Lerp(0.1f, 0f, mpRatio);
            Color color = playerMPsc.color;
            color.a = alpha;
            playerMPsc.color = color;

            // �̹��� ��������Ʈ ��ü
            if (Mp_sc != null && Mp_sc.Length >= 3)
>>>>>>> parent of 83c7234 (0721 HP증가 시, UI에도 적용)
            {
                PlayerData playerData = GameManager.Instance.playerData;

                coin_text.text = $"��: {GameManager.Instance.Gold}";
                soul_text.text = $"ȥ: {GameManager.Instance.Soul} / {GameManager.Instance.N_Day_Cost}";

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
            else // �ΰ��� ���� �����ٰ�
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

                // ������ ����
                float alpha = Mathf.Lerp(0.1f, 0f, mpRatio);
                Color color = playerMPsc.color;
                color.a = alpha;
                playerMPsc.color = color;

                // �̹��� ��������Ʈ ��ü
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
                    playerMPSC.color = color_sc; // ����� ���İ��� ���⼭ �ݿ�
                }
            }
        }
    }
}
