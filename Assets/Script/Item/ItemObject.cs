using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemObject : MonoBehaviour
{
    public ItemData itemDataTemplate;
    private Transform uiCanvas; // �÷��̾� ĵ������ �־����
    public Item itemData;
    private SpriteRenderer spriteRenderer;
    private Player_Item_Use player_item_use;
    //public GameObject item_soul;
    public GameObject infoPanel; // ���� UI (Canvas) ����
    public GameObject Sale_Effect; // �Ǹ� ����
    public TMP_Text name_text; // �ؽ�Ʈ ����
    public TMP_Text coin_text; // �ؽ�Ʈ ����
    public GameObject holdGaugeUI; // �Ǹ� Ű ������ UI
    public Image holdGauge; // ������ �̹��� (Fill Amount ���)

    private const float maxHoldTime = 1f; // �Ǹ� Ű �ִ� �ð� 
    private PlayerController playerController;
    private Material material;


    Collider2D coll;
    Rigidbody2D rb;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player_item_use = FindObjectOfType<Player_Item_Use>();
        playerController = FindObjectOfType<PlayerController>();
        material = spriteRenderer.material;

        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (itemDataTemplate != null)
        {
            itemData = new Item(itemDataTemplate);
        }
        if (spriteRenderer != null && itemData != null && itemData.InGameSprite != null)
        {
            spriteRenderer.sprite = itemData.InGameSprite;
        }
        if (itemData != null && itemData.Drop_item == false) { itemData.SetRandomValues(); } // ���� ������ ���� �� ����

        uiCanvas = GameObject.Find("Player_Canvas")?.transform;
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

            float glow = distance <= playerController.flashLightDistance ? 2f : 0f;
            material?.SetFloat("_GlowIntensity", glow);
        }
        else
        {
            UpdateHoldGauge(0f); // �÷��̾ �־����� ������ ����
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
    public void Sale(string ty) // "one" or "all"
    {
        int itemValue = itemData.Coin;

        if (ty == "one")
        {
            // Soul ����Ʈ �� 1ȸ (��ġ ���� X)
            SpawnEffectParts(itemValue, "Soul");

            GameManager.Instance.Add_Soul(itemValue);
            SoundManager.Instance?.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_money_1"));
        }
        else if (ty == "all")
        {
            // ���� ���ӻ� ��ȭ ����
            GameManager.Instance?.Add_Gold(itemValue);
            if (itemData.id != 3)
            {
                // 2. Soul ����Ʈ: ���� ��ġ�� 2�踸ŭ
                SpawnEffectParts(itemValue * 2, "Soul");
                SpawnEffectParts(itemValue, "Coin");
                GameManager.Instance?.Add_Soul(itemValue * 2);
            }
            else
            {
                // 1. Coin ����Ʈ: ���� ��ġ��ŭ
                SpawnEffectParts(itemValue, "Coin");
            }
        }

        Destroy(gameObject); // ������ ������Ʈ ����
    }

    private void SpawnEffectParts(int totalValue, string type)
    {
        int remainingValue = totalValue;

        while (remainingValue > 0)
        {
            int shardValue = Random.Range(10, 21);
            if (shardValue > remainingValue)
                shardValue = remainingValue;

            GameObject fx = Instantiate(Sale_Effect, transform.position, Quaternion.identity);
            fx.transform.SetParent(uiCanvas, false);
            MoneyEffect effect = fx.GetComponent<MoneyEffect>();
            effect.ty = type;

            remainingValue -= shardValue;
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
        
        if (name_text != null) name_text.text = string.Format("[{0}]", itemData.itemName);

        if (coin_text != null)
        {
            int total_coin = itemData.Coin * itemData.Count;
            if (itemData.Sell_immediately) { coin_text.text = string.Format("[<b>E</b>] 줍기\n[<b>E~</b>] 즉시 판매: {0} 혼", total_coin); }
            else {coin_text.text = string.Format("[<b>E</b>] 줍기\n{0} 값", total_coin);} //��� �ǸŰ� �Ұ����� �������� ���
        }
    }
}
