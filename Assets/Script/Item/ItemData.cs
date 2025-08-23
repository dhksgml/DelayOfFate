using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public int id; // 고유 ID

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
}
