using UnityEngine;
using UnityEngine.UI;

public class ItemSaleZone : MonoBehaviour
{
    [SerializeField] private LayerMask itemLayer;
    private Collider2D saleArea;
    public Place sale_place;
    public SpriteRenderer spriteRenderer;
    public Image saleGaugeImage; // UI �̹��� (Filled ���� ����)

    private void Awake()
    {
        saleArea = GetComponent<Collider2D>();
    }
    private void Update()
    {
        // sale_cu_Time�� ����� ������ ����
        if (saleGaugeImage != null && sale_place != null)
        {
            float ratio = Mathf.Clamp01(sale_place.sale_cu_Time / sale_place.sale_max_Time);
            saleGaugeImage.fillAmount = 1f - ratio;
            // 0�̸� �� �� ���̰�, max�ϼ��� �������� �پ��
        }
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

            if (itemObject.itemData.isUsable) continue;

            //��, ��ȥ ȹ��
            GameManager.Instance?.Add_Gold(itemObject.itemData.Coin);
            GameManager.Instance?.Add_Soul(itemObject.itemData.Coin * 2);

            Destroy(itemObject.gameObject);

            sale_place.sale_cu_Time = sale_place.sale_max_Time;
            sale_place.contactTime = 0f;
            if (sale_place.holdGauge != null)
            {
                sale_place.holdGauge.fillAmount = 0f;
                sale_place.holdGauge.gameObject.SetActive(false);
            }
        }
    }
}
