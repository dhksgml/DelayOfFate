using UnityEngine.SceneManagement;
using UnityEngine;

public class Stage_Manager : MonoBehaviour
{
    public GameObject ShopPrefab; // ��� ���� ���
    public GameObject QuestPrefab; //�̼� ī�� 3��
    public void Quest_ok() // �̼��� �� �� ���� �������� ��ȯ
    {
        ShopPrefab.SetActive(true);
        QuestPrefab.SetActive(false);
    }
    public void Shop_end() // ���� ���� �� �� ���������� �Ѿ��
    {
        SceneManager.LoadScene("InGame_Scenes");
    }
}
