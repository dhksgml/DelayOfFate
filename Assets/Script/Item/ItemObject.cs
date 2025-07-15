using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemObject : MonoBehaviour
{
    public Item itemData;
    private SpriteRenderer spriteRenderer;
    private Player_Item_Use player_item_use;
    public GameObject item_soul;
    public GameObject infoPanel; // ���� UI (Canvas) ����
    public TMP_Text coin_text; // �ؽ�Ʈ ����
    public GameObject holdGaugeUI; // �Ǹ� Ű ������ UI
    public Image holdGauge; // ������ �̹��� (Fill Amount ���)

    private const float maxHoldTime = 1f; // �Ǹ� Ű �ִ� �ð� 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player_item_use = FindObjectOfType<Player_Item_Use>();

        if (spriteRenderer != null && itemData != null && itemData.InGameSprite != null)
        {
            spriteRenderer.sprite = itemData.InGameSprite;
        }
        if (itemData.Drop_item == false) { itemData.SetRandomValues(); } // ���� ������ ���� �� ����

        
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

        if (distance < 1.5f) // �÷��̾ ������ ���� ���� ������ Ȱ��ȭ
        {
            float progress = Mathf.Clamp(player_item_use.holdTime / maxHoldTime, 0f, 1f);
            UpdateHoldGauge(progress);
        }
        else
        {
            UpdateHoldGauge(0f); // �÷��̾ �־����� ������ ����
        }

        if (itemData == null) // ���� ������ �� �ִ°�?
        {
            item_soul.SetActive(false);
        }
        else
        {
            item_soul.SetActive(true);
        }
    }
    private float GetCollisionRadius() // �浹�� ������ ����
    {
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            return circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y); // ������ �ݿ�
        }

        return 0.5f; // fallback
    }

    private void FixedUpdate() // ������ ���� �о�� �ڵ��ε� �۵� ����...
    {
        float radius = GetCollisionRadius();
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Item"));

        foreach (Collider2D col in overlaps)
        {
            if (col.gameObject != this.gameObject)
            {
                Vector2 direction = (transform.position - col.transform.position).normalized;
                float distance = Vector2.Distance(transform.position, col.transform.position);

                GetComponent<Rigidbody2D>().AddForce(direction * 10f); // �� ���� ����
                
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel?.SetActive(true);
            holdGaugeUI?.SetActive(true); // �÷��̾ ������ ���� ������ UI Ȱ��ȭ
            other.GetComponent<PlayerController>().isPickUpableItem = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel?.SetActive(false);
            holdGaugeUI?.SetActive(false); // �÷��̾ �־����� UI ����
            other.GetComponent<PlayerController>().isPickUpableItem = false;
        }
    }


    public void UpdateHoldGauge(float progress)
    {
        if (holdGaugeUI != null)
        {
            holdGaugeUI.SetActive(progress > 0); // progress�� 0���� Ŭ ���� UI Ȱ��ȭ
        }
        if (holdGauge != null)
        {
            holdGauge.fillAmount = progress;
        }
        if (coin_text != null)
        {
            int total_coin = itemData.Coin * itemData.Count;
            coin_text.text = string.Format("[{0}] {1} ��", itemData.itemName, total_coin);
        }
    }
}
