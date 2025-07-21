using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuickSlotUI : MonoBehaviour
{
    public Image[] slotImages;          // �� ������ ������ ������
    public Image[] slotBackgrounds;     // �� ������ ��� �̹��� (Ȱ��ȭ ǥ��)
    public TMP_Text[] slotCounts;       // �� ������ ������ ���� �ؽ�Ʈ
    public Sprite default_Item_Sprite;  // �⺻ ������ ������
    public Sprite defaultSlotSprite;    // �⺻ ���� ���
    public Sprite selectedSlotSprite;   // ���õ� ���� ���
    public TMP_Text Item_Name;          // ������ �������� �̸�
    public TMP_Text Item_Coin;          // ������ �������� ��ġ
    public TMP_Text Item_Weight;        // ������ �������� ����

    public Player_Item_Use playerItemUse;
    private PlayerController playerController;

    public TMP_Text timeText; // UI �ؽ�Ʈ ������Ʈ
    public int angleUnit = 0;

    void Start()
    {
        Item_Name.text = null;
        Item_Coin.text = null;
        Item_Weight.text = null;
        playerItemUse = FindObjectOfType<Player_Item_Use>();
        playerController = FindObjectOfType<PlayerController>();
        UpdateUI();
    }
    void Update()
    {
        float time = Time.time; // ��� �ð� (��)
        angleUnit = Mathf.FloorToInt(time / 20f); // 20�ʸ��� 1��
    }

    public void DisplayItemInfo(int index, ItemData item)
    {
        print(item);
        if (index < 0 || index >= 4) return;

        if (item != null && !string.IsNullOrEmpty(item.itemName))
        {
            slotImages[index].sprite = item.icon;

            if (item.Count_Check)
            {
                slotCounts[index].gameObject.SetActive(true);
                slotCounts[index].text = item.Count.ToString();
            }
            else
            {
                slotCounts[index].gameObject.SetActive(false);
            }
        }
        else
        {
            // �� ���� ó��
            slotImages[index].sprite = default_Item_Sprite;
            slotCounts[index].gameObject.SetActive(false);
        }
    }
    public void UpdateUI()
    {
        if (SceneManager.GetActiveScene().name == "Stage_Scene")
        {
            timeText.text = GameManager.Instance.Day + " ��";
        }
        else
        {
            if (playerController.currentState == PlayerController.PlayerState.Idle)
            {
                timeText.text = angleUnit + " ��";
            }
            else
            {
                timeText.text = null;
            }
        }
        //print(playerItemUse);
        if (playerItemUse == null)
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            Item item = playerItemUse.quickSlots[i];

            if (item != null && !string.IsNullOrEmpty(item.itemName))
            {
                // ������ ������ ����
                slotImages[i].sprite = item.icon;

                // ������ ��� ǥ�� ����
                if (item.Count_Check)
                {
                    slotCounts[i].gameObject.SetActive(true);
                    slotCounts[i].text = item.Count.ToString();
                }
                else
                {
                    slotCounts[i].gameObject.SetActive(false);
                }
            }
            else
            {
                // �� ���� ó��
                slotImages[i].sprite = default_Item_Sprite;
                slotCounts[i].gameObject.SetActive(false);
            }

            // ���õ� ���� ��� ǥ��
            slotBackgrounds[i].sprite = (i == playerItemUse.selectedSlotIndex)
                ? selectedSlotSprite
                : defaultSlotSprite;
        }
        // ���õ� ������ ������ ������ UI�� ǥ��
        Item selectedItem = playerItemUse.quickSlots[playerItemUse.selectedSlotIndex];
        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.itemName))
        {
            int total_coin = selectedItem.Coin * selectedItem.Count;
            int total_Weight = selectedItem.Weight * selectedItem.Count;
            Item_Name.text = string.Format("[{0}]", selectedItem.itemName);
            Item_Coin.text = total_coin.ToString() + " ��";
            Item_Weight.text = total_Weight.ToString() + " ��";
        }
        else
        {
            Item_Name.text = null;
            Item_Coin.text = null;
            Item_Weight.text = null;
        }
    }
    public void UpdateUI(Item[] quickSlots, int selectedIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            Item item = quickSlots[i];

            if (item != null && !string.IsNullOrEmpty(item.itemName))
            {
                slotImages[i].sprite = item.icon;

                if (item.Count_Check)
                {
                    slotCounts[i].gameObject.SetActive(true);
                    slotCounts[i].text = item.Count.ToString();
                }
                else
                {
                    slotCounts[i].gameObject.SetActive(false);
                }
            }
            else
            {
                slotImages[i].sprite = default_Item_Sprite;
                slotCounts[i].gameObject.SetActive(false);
            }

            slotBackgrounds[i].sprite = (i == selectedIndex) ? selectedSlotSprite : defaultSlotSprite;
        }

        Item selectedItem = quickSlots[selectedIndex];
        if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.itemName))
        {
            int total_coin = selectedItem.Coin * selectedItem.Count;
            int total_Weight = selectedItem.Weight * selectedItem.Count;
            Item_Name.text = $"[{selectedItem.itemName}]";
            Item_Coin.text = $"{total_coin} ��";
            Item_Weight.text = $"{total_Weight} ��";
        }
        else
        {
            Item_Name.text = null;
            Item_Coin.text = null;
            Item_Weight.text = null;
        }
    }
}
