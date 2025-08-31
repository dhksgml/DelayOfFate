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
    public Sprite InGameSprite;
    public Sprite icon;
    public int Coin;  // 코인

    [Header("판매 가능")]
    public bool Sell_whether;

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
        this.InGameSprite = itemData.InGameSprite;
        this.icon = itemData.icon;

        this.Coin = itemData.Coin;

        this.Sell_whether = itemData.Sell_whether;

        this.CoinDeviation = itemData.CoinDeviation;

        this.Drop_item = itemData.Drop_item;

    }
    public ItemData ToItemData()
    {
        ItemData data = new ItemData();

        data.id = this.id;
        data.itemType = this.itemType;
        data.itemName = this.itemName;
        data.InGameSprite = this.InGameSprite;
        data.icon = this.icon;

        data.Coin = this.Coin;

        data.Sell_whether = this.Sell_whether;

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
            icon = this.icon,
            InGameSprite = this.InGameSprite,
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
