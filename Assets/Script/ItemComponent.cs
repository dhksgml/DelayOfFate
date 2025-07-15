using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public int itemID; // 이 오브젝트가 참조할 아이템 ID
    public ItemDatabase database; // 인스펙터에 연결
    public Item itemData; // 실제 사용될 아이템 정보

    void Awake()
    {

    }
}