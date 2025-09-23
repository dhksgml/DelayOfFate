using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public int id; // 고유 ID
    public ItemType itemType;

    [Header("이름, 효과, 스프라이트")]
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
}
