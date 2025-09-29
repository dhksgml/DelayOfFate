using UnityEngine;

//[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]

public enum ItemType
{
    nomal,
    weapon
}

public class Item
{
    public int id; // 고유 ID
    public ItemType itemType;

    [Header("이름, 인게임, 아이콘")]
    public string itemName;
    public string item_Passive;
    public Sprite InGameSprite;

    [Header("아이템 사용 SP")]
    public float spendSPAmount;

    [Space(10)]
    public int Coin;  // 코인

    [Header("중복형 아이템인가")]
    public bool Count_Check;
    public int Count = 1;

    [Header("사용 가능한 아이템인가")]
    public bool isUsable;
    public float Usage_cool_down;
    public bool Charging;

    [Header("랜덤 값 설정")]
    public int CoinDeviation; // 오차 값 (±)

    [HideInInspector]
    public bool Drop_item; // 떨어트린 적이 있는 아이템인가

    public Item()
    {

    }

    public Item(ItemData itemData)
    {
        this.id = itemData.id;
        this.itemType = itemData.itemType;
        this.itemName = itemData.itemName;
        this.item_Passive = itemData.item_Passive;
        this.InGameSprite = itemData.InGameSprite;

        this.spendSPAmount = itemData.spendSPAmount;

        this.Coin = itemData.Coin;

        this.Count_Check = itemData.Count_Check;
        this.Count = itemData.Count;

        this.isUsable = itemData.isUsable;
        this.Usage_cool_down = itemData.Usage_cool_down;
        this.Charging = itemData.Charging;

        this.CoinDeviation = itemData.CoinDeviation;

        this.Drop_item = itemData.Drop_item;

    }
    public ItemData ToItemData()
    {
        ItemData data = new ItemData();

        data.id = this.id;
        data.itemType = this.itemType;
        data.itemName = this.itemName;
        data.item_Passive = this.item_Passive;
        data.InGameSprite = this.InGameSprite;

        data.spendSPAmount = this.spendSPAmount;

        data.Coin = this.Coin;

        data.Count_Check = this.Count_Check;
        data.Count = this.Count;

        data.isUsable = this.isUsable;
        data.Usage_cool_down = this.Usage_cool_down;
        data.Charging = this.Charging;

        data.CoinDeviation = this.CoinDeviation;

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
            item_Passive = this.item_Passive,
            InGameSprite = this.InGameSprite,
            Count_Check = this.Count_Check,
            Count = this.Count,
            Coin = this.Coin,
            CoinDeviation = this.CoinDeviation,
            Drop_item = this.Drop_item
        };
    }

    public void SetRandomValues()
    {
        // coin 오차 적용
        int minCoin = Mathf.Max(1, Coin - CoinDeviation);
        int maxCoin = Coin + CoinDeviation + 1; // +1은 Random.Range(int, int)의 특성 (최댓값 미포함)

        Coin = Random.Range(minCoin, maxCoin);
    }
}
