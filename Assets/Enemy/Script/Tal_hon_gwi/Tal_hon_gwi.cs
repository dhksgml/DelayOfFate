using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tal_hon_gwi : Enemy
{
    [Header("탈혼귀")]
    [SerializeField] Player_Item_Use player_item_use;
    [SerializeField] Sprite[] randomImages;
    [SerializeField] Sprite[] talhongwiOriginSprite;
    [HideInInspector] public bool isSeek = false;
    [SerializeField] bool isDestroy = false;
    [SerializeField] int talhongwiDamage = 20;
    PlayerController player;

    [Header("참조")]
    public ItemData[] itemDataTemplate;
    public Item item;
    public GameObject holdGaugeUI; 
    public Image holdGauge;
    public GameObject infoPanel;                   // 아이템 정보 UI 패널 (월드 상에서 표시)
    public GameObject Sale_Effect;                 // 판매 시 이펙트 프리팹
    public TMP_Text name_text;                     // 아이템 이름 텍스트
    public TMP_Text coin_text;
    private Transform uiCanvas;

    [SerializeField] GameObject surpriseCanvas;
    [SerializeField] Image surpriseImage;
    [SerializeField] float startScale = 0.1f;
    [SerializeField] float endScale = 10f; 
    [SerializeField] float scaleUpTime = 2f;

    const float maxHoldTime = 1f;

    void Awake()
    {
        // 회전 값을 위한 랜덤
        int randomFlipX = Random.Range(0, 2);
        int randomFlipY = Random.Range(0, 2);

        // X회전
        sp.flipX = true;

        // Y회전
        sp.flipY = true;

        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();

        player_item_use = FindObjectOfType<Player_Item_Use>();

        // 이미지를 랜덤으로 가져와 줌
        int random = Random.Range(0, randomImages.Length);
        sp.sprite = randomImages[random];

        if (itemDataTemplate != null)
        {
            item = new Item(itemDataTemplate[random]);
        }

        // UI 참조 설정 및 비활성화
        uiCanvas = GameObject.Find("Player_Canvas")?.transform;
        infoPanel?.SetActive(false);
        holdGaugeUI?.SetActive(false);
    }


    void Update()
    {
        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;

            sp.sprite = talhongwiOriginSprite[1];

            StartCoroutine(EnemyDie());
        }

        // 플레이어가 E키를 누르면 
        if (isSeek && !isDestroy)
        {
            isDestroy = true;

            if (player == null) { return; }

            // 본모습 등장
            sp.sprite = talhongwiOriginSprite[0];

            // 놀래키기
            StartCoroutine(ScaleImage());

            // 정신 데미지
            player.DamagedMP(talhongwiDamage);
            // 기력 데미지
            player.DamagedSP(player.maxSp * 0.3f);
        }


        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < 1.5f)
        {
            float progress = Mathf.Clamp(player_item_use.holdTime / maxHoldTime, 0f, 1f);
            UpdateHoldGauge(progress); // 게이지 진행도 반영
        }
        else
        {
            UpdateHoldGauge(0f); // 멀어지면 게이지 숨김
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //적 피격 부분
            //이부분은 아마 적 공동 코드로 사용할 것 같다.
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // 타입이 일치하면 즉사
                if (attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //이부분 없다 나와서 일단 주석 처리 해주었음.
                    //attack.CheckWeakness();
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }
                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }

            if (collision.CompareTag("Player"))
            {
                infoPanel?.SetActive(true);
                holdGaugeUI?.SetActive(true);
                collision.GetComponent<PlayerController>().isPickUpableItem = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            infoPanel?.SetActive(false);
            holdGaugeUI?.SetActive(false);
            other.GetComponent<PlayerController>().isPickUpableItem = false;
        }
    }


    public override void EnemyMove()
    {

    }

    public IEnumerator EnemySeek()
    {
        // 사망시 이펙트 
        Instantiate(enemyDeathEffect, transform.position, Quaternion.identity);

        Color color = sp.color;

        //먼저 추적 범위와 공격 범위를 지워줌.
        Destroy(enemyTrace);
        Destroy(enemyAttack);
        Destroy(enemyColl);
        Destroy(rigid);

        // 이동속도 0으로 해서 움직이지 못하게
        enemyMoveSpeed = 0;


        //투명도 값을 1.0에서 0.01씩 뺴주면서 천천히 투명하게 해줌
        for (float i = 1.0f; i >= 0.0f; i -= 0.02f)
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_ghost_death"));
        
        // 시체의 부모 통째로 제거
        Destroy(transform.parent.gameObject);
    }

    // 게이지 및 텍스트 UI 업데이트
    public void UpdateHoldGauge(float progress)
    {
        if (holdGaugeUI != null)
        {
            holdGaugeUI.SetActive(progress > 0); // 게이지 0 이상일 때만 표시
        }

        if (holdGauge != null)
        {
            holdGauge.fillAmount = progress; // 게이지 진행도 반영
        }

        // 아이템 이름 표시
        if (name_text != null)
            name_text.text = string.Format("[{0}]", item.itemName);

        // 아이템 가치 표시
        if (coin_text != null)
        {
            int total_coin = item.Coin * item.Count;
            if (item.Sell_immediately)
            {
                coin_text.text = string.Format("[<b>E</b>] 줍기\n[<b>E~</b>] 즉시 판매: {0} 혼", total_coin);
            }
            else
            {
                coin_text.text = string.Format("[<b>E</b>] 줍기\n{0} 값", total_coin);
            }
        }
    }

    IEnumerator ScaleImage()
    {

        // 프리팹을 인스턴스화
        GameObject canvasInstance = Instantiate(surpriseCanvas);

        // 이미지 찾기
        Image surpriseImage = canvasInstance.GetComponentInChildren<Image>();

        // 매번 크기 초기화 (여기 중요!)
        surpriseImage.rectTransform.localScale = Vector3.one * startScale;

        float elapsed = 0f;

        while (elapsed < scaleUpTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleUpTime;

            float scale = Mathf.Lerp(startScale, endScale, t * t * t);
            surpriseImage.rectTransform.localScale = Vector3.one * scale;

            yield return null;
        }

        surpriseImage.rectTransform.localScale = Vector3.one * endScale;

        yield return new WaitForSeconds(scaleUpTime);

        Debug.Log(2);
        
        Destroy(canvasInstance);

        StartCoroutine(EnemySeek());
    }
}
