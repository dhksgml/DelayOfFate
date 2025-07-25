using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceManager : MonoBehaviour
{
    [HideInInspector] public Vector2 escape_pos;

    [HideInInspector] public bool resurrection; // ��Ȱ���� ����
    [HideInInspector] public Vector2 resurrection_pos;

    [HideInInspector] public bool sale; // �Ǹ�
    [HideInInspector] public Vector2 sale_pos;

    public void Go_to_escape()
    {
        //���ӸŴ����� �÷��̾� ���� ����
        GameManager.Instance.SaveCurrentQuickSlot(FindObjectOfType<Player_Item_Use>().quickSlots);
        GameManager.Instance.SavePlayerInfo(FindObjectOfType<PlayerController>());
        GameManager.Instance.New_Day_date(FindObjectOfType<QuickSlotUI>().angleUnit);
        SceneManager.LoadScene("Result_Scene");
    }
}
