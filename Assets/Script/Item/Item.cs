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
            Coin = this.Coin,
            CoinDeviation = this.CoinDeviation,
            Drop_item = this.Drop_item
        };
    }

    public void SetRandomValues()
    {
        if (id == 3)
        {
            float rand = Random.Range(0f, 100f);

            if (rand < 10f) Coin = 10;
            else if (rand < 45f) Coin = 50;   // 10 + 35
            else if (rand < 80f) Coin = 100;  // 45 + 35
            else if (rand < 98f) Coin = 200;  // 80 + 18
            else Coin = 500;                   // 98 + 2
        }
        else
        {
            int minCoin = Mathf.Max(1, Coin - CoinDeviation);
            int maxCoin = Coin + CoinDeviation + 1;
            Coin = Random.Range(minCoin, maxCoin);
        }
    }
}
