using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public int itemID; // �� ������Ʈ�� ������ ������ ID
    public ItemDatabase database; // �ν����Ϳ� ����
    public Item itemData; // ���� ���� ������ ����

    void Awake()
    {

    }
}