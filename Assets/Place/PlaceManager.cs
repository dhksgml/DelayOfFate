using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceManager : MonoBehaviour
{
    public Vector2 escape_pos;

    public bool resurrection; // ��Ȱ���� ����
    public Vector2 resurrection_pos;

    public bool sale; // �Ǹ�
    public Vector2 sale_pos;

    public void Go_to_escape()
    {
        //���ӸŴ����� �÷��̾� ���� ����
        GameManager.Instance.SaveCurrentQuickSlot(FindObjectOfType<Player_Item_Use>().quickSlots);
        GameManager.Instance.SavePlayerInfo(FindObjectOfType<PlayerController>());
        SceneManager.LoadScene("Result_Scene");
    }
}
