using UnityEngine;
using UnityEngine.UI;

public class Place : MonoBehaviour
{
    private PlaceManager placeManager;
    private ItemSaleZone sale_zone_obj; // �Ǹű���
    private enum Place_enum
    {
        escape,
        resurrection,
        sale
    }

    [SerializeField] private Place_enum place_enum;
    [SerializeField] private float requiredTime = 3f;
    [SerializeField] public Image holdGauge;
    
    [HideInInspector] public float sale_max_Time = 10f; //�ӽ� ��Ÿ��
    [HideInInspector] public float sale_cu_Time = 0f;

    [HideInInspector] public float contactTime = 0f;
    private bool playerInRange = false;
    private bool registered = false;

    private void Start()
    {
        placeManager = FindObjectOfType<PlaceManager>();

        Transform saleZoneTransform = transform.Find("Sale_zone");
        if (saleZoneTransform != null) sale_zone_obj = saleZoneTransform.GetComponent<ItemSaleZone>(); // �Ǹ� ��Ҹ� �����ϰ� ����
        if (holdGauge != null)holdGauge.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (sale_cu_Time > 0) { sale_cu_Time -= Time.deltaTime; }
        if (registered) return;

        playerInRange = IsPlayerNearby();

        if (playerInRange) // �÷��̾ �ֺ��� ������
        {
            ActivateGauge();

            contactTime += Time.deltaTime;

            UpdateGaugeFill(contactTime / requiredTime);

            if (contactTime >= requiredTime)
            {
                switch (place_enum)
                {
                    case Place_enum.resurrection:
                        RegisterResurrection();
                        break;
                    case Place_enum.escape:
                        EscapeScene();
                        break;
                    case Place_enum.sale:
                        if (sale_cu_Time <= 0)  // ��Ÿ���� 0 ������ ���� �Ǹ� ����
                        {
                            sale_zone_obj.SellItems();
                            sale_cu_Time = requiredTime;
                        }
                        break;
                }
            }
        }
        else // �÷��̾ �ֺ��� ���ٸ� ��� �۵� �ʱ�ȭ ( ������ ��Ȱ��ȭ )
        {
            contactTime = 0f;
            if (holdGauge != null)
            {
                holdGauge.fillAmount = 0f;
                holdGauge.gameObject.SetActive(false);
            }
        }
    }
    private bool IsPlayerNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return true;
        }
        return false;
    }
    private void ActivateGauge()
    {
        if (holdGauge != null && !holdGauge.gameObject.activeSelf) holdGauge.gameObject.SetActive(true);
    }
    private void UpdateGaugeFill(float ratio) // ������ ǥ�ⷮ
    {
        if (holdGauge != null) holdGauge.fillAmount = Mathf.Clamp01(ratio);
    }
    private void RegisterResurrection() //��Ȱ ��� �Ϸ�
    {
        registered = true;
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_resurrection_register"));
        if (holdGauge != null) holdGauge.gameObject.SetActive(false);
        placeManager.resurrection = true;
    }
    private void EscapeScene() // Ż��
    {
        registered = true;
        if (holdGauge != null) holdGauge.gameObject.SetActive(false);
        placeManager.Go_to_escape();
    }
}
