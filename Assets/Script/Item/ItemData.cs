using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
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
}
