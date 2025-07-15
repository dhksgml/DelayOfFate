using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemObject : MonoBehaviour
{
    public Item itemData;
    private SpriteRenderer spriteRenderer;
    private Player_Item_Use player_item_use;
    public GameObject item_soul;
    public GameObject infoPanel; // 월드 UI (Canvas) 참조
    public TMP_Text coin_text; // 텍스트 참조
    public GameObject holdGaugeUI; // 판매 키 게이지 UI
    public Image holdGauge; // 게이지 이미지 (Fill Amount 방식)

    private const float maxHoldTime = 1f; // 판매 키 최대 시간 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player_item_use = FindObjectOfType<Player_Item_Use>();

        if (spriteRenderer != null && itemData != null && itemData.InGameSprite != null)
        {
            spriteRenderer.sprite = itemData.InGameSprite;
        }
        if (itemData.Drop_item == false) { itemData.SetRandomValues(); } // 값이 없으면 랜덤 값 적용

        
        infoPanel?.SetActive(false);
        holdGaugeUI?.SetActive(false);
    }

    void Update()
    {
        if (player_item_use == null)
        {
            Debug.LogWarning("Player_Item_Use not found!");
            return;
        }

        float distance = Vector3.Distance(transform.position, player_item_use.transform.position);

        if (distance < 1.5f) // 플레이어가 가까이 있을 때만 게이지 활성화
        {
            float progress = Mathf.Clamp(player_item_use.holdTime / maxHoldTime, 0f, 1f);
            UpdateHoldGauge(progress);
        }
        else
        {
            UpdateHoldGauge(0f); // 플레이어가 멀어지면 게이지 숨김
        }

        if (itemData == null) // 내부 데이터 가 있는가?
        {
            item_soul.SetActive(false);
        }
        else
        {
            item_soul.SetActive(true);
        }
    }
    private float GetCollisionRadius() // 충돌용 반지름 계산기
    {
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            return circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y); // 스케일 반영
        }

        return 0.5f; // fallback
    }

    private void FixedUpdate() // 아이템 끼리 밀어내는 코드인데 작동 안함...
    {
        float radius = GetCollisionRadius();
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Item"));

        foreach (Collider2D col in overlaps)
        {
            if (col.gameObject != this.gameObject)
            {
                Vector2 direction = (transform.position - col.transform.position).normalized;
                float distance = Vector2.Distance(transform.position, col.transform.position);

                GetComponent<Rigidbody2D>().AddForce(direction * 10f); // 힘 조절 가능
                
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel?.SetActive(true);
            holdGaugeUI?.SetActive(true); // 플레이어가 가까이 오면 게이지 UI 활성화
            other.GetComponent<PlayerController>().isPickUpableItem = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel?.SetActive(false);
            holdGaugeUI?.SetActive(false); // 플레이어가 멀어지면 UI 숨김
            other.GetComponent<PlayerController>().isPickUpableItem = false;
        }
    }


    public void UpdateHoldGauge(float progress)
    {
        if (holdGaugeUI != null)
        {
            holdGaugeUI.SetActive(progress > 0); // progress가 0보다 클 때만 UI 활성화
        }
        if (holdGauge != null)
        {
            holdGauge.fillAmount = progress;
        }
        if (coin_text != null)
        {
            int total_coin = itemData.Coin * itemData.Count;
            coin_text.text = string.Format("[{0}] {1} 냥", itemData.itemName, total_coin);
        }
    }
}
