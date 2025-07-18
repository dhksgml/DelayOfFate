using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public int id; // ���� ID

    [Header("�̸�, �ΰ���, ������")]
    public string itemName;
    public Sprite InGameSprite;
    public Sprite icon;

    public int Coin;  // ����
    public int Weight;
    [Header("����")]
    public int ValPoint;

    [Header("�ߺ��� �������ΰ�")]
    public bool Count_Check;
    public int Count = 1;

    [Header("�Ǹ� ���� / ��� �Ǹ� ����")]
    public bool Sell_whether;
    public bool Sell_immediately;

    [Header("��� ������ �������ΰ�")]
    public bool isUsable;
    public float Usage_cool_down;
    public bool Charging;

    [Header("���� �� ����")]
    public int CoinDeviation; // ���� �� (��)
    public int WeightDeviation; // ���� �� (��)
    public int CountDeviation; // ���� �� (��)

    [HideInInspector]
    public bool Drop_item; // ����Ʈ�� ���� �ִ� �������ΰ�
}
