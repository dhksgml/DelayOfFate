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
        if(GameManager.Instance.isTutorial)
        {
            GameManager.Instance.LoadScene("Tutorial_Scenes");
        }
        else
        {
            ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
            GameManager.Instance.SlotsData = shopQuickSlot.SlotsData; // �ӽ� �����Ϳ� �ִ��� ���ӸŴ����� �ű��
            GameManager.Instance.LoadScene("InGame_Scenes");
        }
    }
}
