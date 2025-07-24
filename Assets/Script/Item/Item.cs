using UnityEngine;

//[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class Item
{
    public int id; // 고유 ID

    [Header("이름, 인게임, 아이콘")]
    public string itemName;
    public Sprite InGameSprite;
    public Sprite icon;

    [Header("아이템 사용 SP")]
    public float spendSPAmount;

    [Space(10)]
    public int Coin;  // 코인
    public int Weight;
    [Header("점수")]
    public int ValPoint;

    [Header("중복형 아이템인가")]
    public bool Count_Check;
    public int Count = 1;

    [Header("판매 가능 / 즉시 판매 가능")]
    public bool Sell_whether;
    public bool Sell_immediately;

    [Header("사용 가능한 아이템인가")]
    public bool isUsable;
    public float Usage_cool_down;
    public bool Charging;

    [Header("랜덤 값 설정")]
    public int CoinDeviation; // 오차 값 (±)
    public int WeightDeviation; // 오차 값 (±)
    public int CountDeviation; // 오차 값 (±)

    [HideInInspector]
    public bool Drop_item; // 떨어트린 적이 있는 아이템인가

    public Item()
    {

    }

    public Item(ItemData itemData)
    {
        this.id = itemData.id;
        this.itemName = itemData.itemName;
        this.InGameSprite = itemData.InGameSprite;
        this.icon = itemData.icon;

        this.spendSPAmount = itemData.spendSPAmount;

        this.Coin = itemData.Coin;
        this.Weight = itemData.Weight;
        this.ValPoint = itemData.ValPoint;

        this.Count_Check = itemData.Count_Check;
        this.Count = itemData.Count;

        this.Sell_whether = itemData.Sell_whether;
        this.Sell_immediately = itemData.Sell_immediately;

        this.isUsable = itemData.isUsable;
        this.Usage_cool_down = itemData.Usage_cool_down;
        this.Charging = itemData.Charging;

        this.CoinDeviation = itemData.CoinDeviation;
        this.WeightDeviation = itemData.WeightDeviation;
        this.CountDeviation = itemData.CountDeviation;

        this.Drop_item = itemData.Drop_item;

    }
    public ItemData ToItemData()
    {
        ItemData data = new ItemData();

        data.id = this.id;
        data.itemName = this.itemName;
        data.InGameSprite = this.InGameSprite;
        data.icon = this.icon;

        data.spendSPAmount = this.spendSPAmount;

        data.Coin = this.Coin;
        data.Weight = this.Weight;
        data.ValPoint = this.ValPoint;

        data.Count_Check = this.Count_Check;
        data.Count = this.Count;

        data.Sell_whether = this.Sell_whether;
        data.Sell_immediately = this.Sell_immediately;

        data.isUsable = this.isUsable;
        data.Usage_cool_down = this.Usage_cool_down;
        data.Charging = this.Charging;

        data.CoinDeviation = this.CoinDeviation;
        data.WeightDeviation = this.WeightDeviation;
        data.CountDeviation = this.CountDeviation;

        data.Drop_item = this.Drop_item;
        if (data == null) return null;
        return data;
    } // 매서드를 호출하면 아이템 데이터만 뽑아감

    // 랜덤 값 세팅은 외부에서 할 수 있도록 남겨둡니다.
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
            CountDeviation = this.CountDeviation, // 오차 값 (±)
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
            // Count 오차 적용
            int minCount = Mathf.Max(1, Count - CountDeviation);
            int maxCount = Weight + CountDeviation + 1;

            Count = Random.Range(minCount, maxCount);
        }

        // coin 오차 적용
        int minCoin = Mathf.Max(1, Coin - CoinDeviation);
        int maxCoin = Coin + CoinDeviation + 1; // +1은 Random.Range(int, int)의 특성 (최댓값 미포함)

        Coin = Random.Range(minCoin, maxCoin);

        // weight 오차 적용
        int minWeight = Mathf.Max(1, Weight - WeightDeviation);
        int maxWeight = Weight + WeightDeviation + 1;

        Weight = Random.Range(minWeight, maxWeight);
    }
}
