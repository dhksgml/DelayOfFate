using UnityEngine;

public class ItemSaleZone : MonoBehaviour
{
    [SerializeField] private LayerMask itemLayer;
    private Collider2D saleArea;
    public Place sale_place;

    private void Awake()
    {
        saleArea = GetComponent<Collider2D>();
    }

    //�ϰ� �Ǹ�
    public void SellItems()
    {
        Vector2 center = saleArea.bounds.center;
        Vector2 size = saleArea.bounds.size;

        Collider2D[] itemColliders = Physics2D.OverlapBoxAll(center, size, 0f, itemLayer);
        
        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();

            //��, ��ȥ ȹ��
            GameManager.Instance?.Add_Gold(itemObject.itemData.Coin);
            GameManager.Instance?.Add_Soul(itemObject.itemData.Coin * 2);

            Destroy(itemObject.gameObject);

            //sale_place.sale_cu_Time = sale_place.sale_max_Time;
            sale_place.sale_cu_Time = 0;
        }
    }
}
