using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tal_hon_gwi : Enemy
{
    [Header("Żȥ��")]
    [SerializeField] Player_Item_Use player_item_use;
    [SerializeField] Sprite[] randomImages;
    [SerializeField] Sprite[] talhongwiOriginSprite;
    [HideInInspector] public bool isSeek = false;
    [SerializeField] bool isDestroy = false;
    [SerializeField] int talhongwiDamage = 20;
    PlayerController player;

    [Header("����")]
    public ItemData[] itemDataTemplate;
    public Item item;
    public GameObject holdGaugeUI; 
    public Image holdGauge;
    public GameObject infoPanel;                   // ������ ���� UI �г� (���� �󿡼� ǥ��)
    //public GameObject Sale_Effect;                 // �Ǹ� �� ����Ʈ ������
    public TMP_Text name_text;                     // ������ �̸� �ؽ�Ʈ
    public TMP_Text coin_text;
    //private Transform uiCanvas;

    [SerializeField] GameObject surpriseCanvas;
    [SerializeField] Image surpriseImage;
    [SerializeField] float startScale = 0.1f;
    [SerializeField] float endScale = 10f; 
    [SerializeField] float scaleUpTime = 2f;

    const float maxHoldTime = 1f;

    void Awake()
    {
        // ȸ�� ���� ���� ����
        int randomFlipX = Random.Range(0, 2);
        int randomFlipY = Random.Range(0, 2);

        // Xȸ��
        sp.flipX = true;

        // Yȸ��
        sp.flipY = true;

        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();

        player_item_use = FindObjectOfType<Player_Item_Use>();

        // �̹����� �������� ������ ��
        int random = Random.Range(0, randomImages.Length);
        sp.sprite = randomImages[random];

        if (itemDataTemplate != null)
        {
            item = new Item(itemDataTemplate[random]);
        }

        // UI ���� ���� �� ��Ȱ��ȭ
        // uiCanvas = GameObject.Find("Player_Canvas")?.transform;
        infoPanel?.SetActive(false);
        holdGaugeUI?.SetActive(false);
    }


    void Update()
    {
        //���� ü���� 0�����Ͻ�.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;

            sp.sprite = talhongwiOriginSprite[1];

            StartCoroutine(EnemyDie());
        }

        // �÷��̾ EŰ�� ������ 
        if (isSeek && !isDestroy)
        {
            isDestroy = true;

            if (player == null) { return; }

            // ����� ����
            sp.sprite = talhongwiOriginSprite[0];

            // �Ű��
            StartCoroutine(ScaleImage());

            // ���� ������
            player.DamagedMP(talhongwiDamage);
        }


        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < 1.5f)
        {
        }
        else
        {
            UpdateHoldGauge(0f); // �־����� ������ ����
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //�� �ǰ� �κ�
            //�̺κ��� �Ƹ� �� ���� �ڵ�� ����� �� ����.
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // Ÿ���� ��ġ�ϸ� ���
                if (!attack.CheckWeaknessPassive() && attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //�̺κ� ���� ���ͼ� �ϴ� �ּ� ó�� ���־���.
                    Enemy_Weakness_Hit(attack.damage, attack.attackType.ToString(), enemyHp);
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }
                EnemyHit(attack.damage);
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
        // ����� ����Ʈ 
        Instantiate(enemyDeathEffect, transform.position, Quaternion.identity);

        Color color = sp.color;

        //���� ���� ������ ���� ������ ������.
        Destroy(enemyTrace);
        Destroy(enemyAttack);
        Destroy(enemyColl);
        Destroy(rigid);

        // �̵��ӵ� 0���� �ؼ� �������� ���ϰ�
        enemyMoveSpeed = 0;


        //������ ���� 1.0���� 0.01�� ���ָ鼭 õõ�� �����ϰ� ����
        for (float i = 1.0f; i >= 0.0f; i -= 0.02f)
        {
            color.a = i;
            sp.color = color;
            //�����̸� ���� �ڷ�ƾ�� �������
            yield return new WaitForSeconds(0.01f);
        }
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_ghost_death"));
        
        // ��ü�� �θ� ��°�� ����
        Destroy(transform.parent.gameObject);
    }

    // ������ �� �ؽ�Ʈ UI ������Ʈ
    public void UpdateHoldGauge(float progress)
    {
        // ������ �̸� ǥ��
        if (name_text != null)
            name_text.text = string.Format("[{0}]", item.itemName);

        // ������ ��ġ ǥ��
        if (coin_text != null)
        {
            int total_coin = item.Coin;
            coin_text.text = string.Format("[<b>E</b>] �ݱ�\n[<b>E~</b>] ��� �Ǹ�: {0} ȥ", total_coin);
            //coin_text.text = string.Format("[<b>E</b>] �ݱ�\n{0} ��", total_coin);
            
        }
    }

    IEnumerator ScaleImage()
    {

        // �������� �ν��Ͻ�ȭ
        GameObject canvasInstance = Instantiate(surpriseCanvas);

        // �̹��� ã��
        Image surpriseImage = canvasInstance.GetComponentInChildren<Image>();

        // �Ź� ũ�� �ʱ�ȭ (���� �߿�!)
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
