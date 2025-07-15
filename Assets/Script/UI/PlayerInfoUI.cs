using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerController playerController;
    public Image playerHpBar;
    public Image playerSpBar;
    public Image playerMPsc;
    public Image playerMPSC;// �����ڸ� �̹��� ����
    public Sprite[] Mp_sc; // �����ڸ� �̹��� 3��

    private float maxHpBarWidth; // ���� UI������ �ִ� �� �ʺ�
    private float maxSpBarWidth;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        maxHpBarWidth = playerHpBar.rectTransform.sizeDelta.x;
        maxSpBarWidth = playerSpBar.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        if (playerController == null) // �÷��̾ ���� ��� (����, �������� ����)
        {

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
        }
        if (playerMPsc != null && playerController != null)
        {
            float mpRatio = playerController.currentMp / playerController.maxMp;

            // ���� ����
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
