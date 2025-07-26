using UnityEngine;
using UnityEngine.UI;

public class ItemSaleZone : MonoBehaviour
{
    [SerializeField] private LayerMask itemLayer;
    private Collider2D saleArea;
    public Place sale_place;
    public SpriteRenderer spriteRenderer;
    public Image saleGaugeImage; // UI 이미지 (Filled 모드로 설정)

    private void Awake()
    {
        saleArea = GetComponent<Collider2D>();
    }
    private void Update()
    {
        // sale_cu_Time에 비례해 게이지 조절
        if (saleGaugeImage != null && sale_place != null)
        {
            float ratio = Mathf.Clamp01(sale_place.sale_cu_Time / sale_place.sale_max_Time);
            saleGaugeImage.fillAmount = 1f - ratio;
            // 0이면 꽉 차 보이고, max일수록 게이지가 줄어듬
        }
    }

    //일괄 판매
    public void SellItems()
    {
        Vector2 center = saleArea.bounds.center;
        Vector2 size = saleArea.bounds.size;

        Collider2D[] itemColliders = Physics2D.OverlapBoxAll(center, size, 0f, itemLayer);
        
        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();

            if (itemObject.itemData.isUsable) continue;

            //돈, 영혼 획득
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
