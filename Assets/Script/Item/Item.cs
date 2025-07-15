using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class Item : ScriptableObject
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


    // ���� �� ������ �ܺο��� �� �� �ֵ��� ���ܵӴϴ�.
    public Item Clone()
    {
        return new Item
        {
            itemName = this.itemName,
            icon = this.icon,
            InGameSprite = this.InGameSprite,
            Count_Check = this.Count_Check,
            Count = this.Count,
            Coin = this.Coin,
            CoinDeviation = this.CoinDeviation,
            Weight = this.Weight,
            WeightDeviation = this.WeightDeviation,
            CountDeviation = this.CountDeviation, // ���� �� (��)
            Drop_item = this.Drop_item
        };
    }

    public void SetRandomValues()
    {
        if (!Count_Check)
        {
            Count = 1;
        }
        else
        {
            // Count ���� ����
            int minCount = Mathf.Max(1, Count - CountDeviation);
            int maxCount = Weight + CountDeviation + 1;

            Count = Random.Range(minCount, maxCount);
        }

        // coin ���� ����
        int minCoin = Mathf.Max(1, Coin - CoinDeviation);
        int maxCoin = Coin + CoinDeviation + 1; // +1�� Random.Range(int, int)�� Ư�� (�ִ� ������)

        Coin = Random.Range(minCoin, maxCoin);

        // weight ���� ����
        int minWeight = Mathf.Max(1, Weight - WeightDeviation);
        int maxWeight = Weight + WeightDeviation + 1;

        Weight = Random.Range(minWeight, maxWeight);
    }

}
