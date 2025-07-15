using UnityEngine;
using UnityEngine.UI;

public class Place : MonoBehaviour
{
    private PlaceManager placeManager;
    private ItemSaleZone sale_zone_obj; // 판매구역
    private enum Place_enum
    {
        escape,
        resurrection,
        sale
    }

    [SerializeField] private Place_enum place_enum;
    [SerializeField] private float requiredTime = 3f;
    [SerializeField] private Image holdGauge;

    [SerializeField] public float sale_max_Time = 10f; //임시 쿨타임
    public float sale_cu_Time = 0f;

    private float contactTime = 0f;
    private bool playerInRange = false;
    private bool registered = false;

    private void Start()
    {
        placeManager = FindObjectOfType<PlaceManager>();

        Transform saleZoneTransform = transform.Find("Sale_zone");
        if (saleZoneTransform != null) sale_zone_obj = saleZoneTransform.GetComponent<ItemSaleZone>(); // 판매 장소만 보유하고 있음
        if (holdGauge != null)holdGauge.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (sale_cu_Time > 0) { sale_cu_Time -= Time.deltaTime; }
        if (registered) return;

        playerInRange = IsPlayerNearby();

        if (playerInRange) // 플레이어가 주변에 있으면
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
                        if (sale_cu_Time == 0) sale_zone_obj.SellItems(); // 판매 쿨타임이 되야 판매 가능
                        break;
                }
            }
        }
        else // 플레이어가 주변에 없다면 기능 작동 초기화 ( 게이지 비활성화 )
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
    private void UpdateGaugeFill(float ratio) // 게이지 표기량
    {
        if (holdGauge != null) holdGauge.fillAmount = Mathf.Clamp01(ratio);
    }
    private void RegisterResurrection() //부활 등록 완료
    {
        registered = true;
        if (holdGauge != null) holdGauge.gameObject.SetActive(false);
        placeManager.resurrection = true;
    }
    private void EscapeScene() // 탈출
    {
        registered = true;
        if (holdGauge != null) holdGauge.gameObject.SetActive(false);
        placeManager.Go_to_escape();
    }
}
