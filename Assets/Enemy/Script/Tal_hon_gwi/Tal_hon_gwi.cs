using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tal_hon_gwi : Enemy
{
    [Header("Żȥ��")]
    [SerializeField] Sprite[] randomImages;
    [SerializeField] Sprite[] talhongwiOriginSprite;
    [HideInInspector] public bool isSeek = false;
    [SerializeField] bool isDestroy = false;
    [SerializeField] int talhongwiDamage = 20;
    PlayerController player;

    void Awake()
    {
        // �̹����� �������� ������ ��
        int random = Random.Range(0, randomImages.Length);
        sp.sprite = randomImages[random];

        // ȸ�� ���� ���� ����
        int randomFlipX = Random.Range(0, 2);
        int randomFlipY = Random.Range(0, 2);

        // Xȸ��
        if (randomFlipX == 0) { sp.flipX = true; }
        else { sp.flipX = false; }

        // Yȸ��
        if (randomFlipY == 0) { sp.flipY = true; }
        else { sp.flipY = false; }

        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();
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

            // ���� ������
            player.DamagedMP(talhongwiDamage);
            // ��� ������
            player.DamagedSP(player.maxSp * 0.3f);

            StartCoroutine(EnemySeek());
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
                if (attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //�̺κ� ���� ���ͼ� �ϴ� �ּ� ó�� ���־���.
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


        //���� ���� 1.0���� 0.01�� ���ָ鼭 õõ�� �����ϰ� ����
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
}
