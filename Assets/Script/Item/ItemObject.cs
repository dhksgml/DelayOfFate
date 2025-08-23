using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemObject : MonoBehaviour
{
    public ItemData itemDataTemplate;
    public Item itemData;
    public GameObject infoPanel; // ���� UI (Canvas) ����
    public TMP_Text name_text; // �ؽ�Ʈ ����
    public TMP_Text coin_text; // �ؽ�Ʈ ����

    private SpriteRenderer spriteRenderer;
    private Player_Item_Use player_item_use;
    private const float maxHoldTime = 1f; // �Ǹ� Ű �ִ� �ð� 
    private PlayerController playerController;
    private Material material;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player_item_use = FindObjectOfType<Player_Item_Use>();
        playerController = FindObjectOfType<PlayerController>();
        material = spriteRenderer.material;

        if (itemDataTemplate != null)
        {
            itemData = new Item(itemDataTemplate);
        }
        if (spriteRenderer != null && itemData != null && itemData.InGameSprite != null)
        {
            spriteRenderer.sprite = itemData.InGameSprite;
        }
        if (itemData != null && itemData.Drop_item == false) { itemData.SetRandomValues(); } // ���� ������ ���� �� ����
        infoPanel?.SetActive(false);
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
            float glow = distance <= playerController.flashLightDistance ? 2f : 0f;
            material?.SetFloat("_GlowIntensity", glow);
        }
        else
        {
            UpdateHoldGauge(); // �÷��̾ �־����� ������ ����
        }

        if (itemData == null) // ���� ������ �� �ִ°�?
        {
            //item_soul.SetActive(false);
        }
        else
        {
            //item_soul.SetActive(true);
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
            other.GetComponent<PlayerController>().isPickUpableItem = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel?.SetActive(false);
            other.GetComponent<PlayerController>().isPickUpableItem = false;
        }
    }

    public void UpdateHoldGauge()
    {

        if (name_text != null) name_text.text = string.Format("[{0}]", itemData.itemName);

        if (coin_text != null)
        {
<<<<<<< HEAD
            int total_coin = itemData.Coin;
            coin_text.text = string.Format("[<b>E</b>] 줍기\n[<b>F</b>] 즉시 판매: {0}<sprite=9>", total_coin);
=======
            int total_coin = itemData.Coin * itemData.Count;
            if (itemData.Sell_immediately) { coin_text.text = string.Format("[<b>E</b>] 줍기\n[<b>F</b>] 즉시 판매: 000<sprite=9>", total_coin); }
>>>>>>> parent of e3d24ca (Mjs 2 (#187))
        }
    }
}
