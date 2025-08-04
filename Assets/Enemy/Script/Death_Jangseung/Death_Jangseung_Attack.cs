using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Jangseung_Attack : EnemyAttack
{
    [Header("�������")]
    [SerializeField] Death_Jangseung death_Jangseung;
    [HideInInspector] PlayerController player;
    [SerializeField] GameObject attackRange;
    public GameObject attackSprite;
    [HideInInspector] public bool isAttack = false;
    
        
    // ����� ���� ������ �����ְ� �� �� �ڿ� �������� ����
    [SerializeField] float jangseungAttackDelay;
    float jangseungAttackTime;

    void Awake()
    {
        //�÷��̾ ã�Ƽ� �������� ��
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (death_Jangseung.isSpawnReaper) 
        { 
            attackRange.SetActive(false);
            enemyAttackCollider.enabled = false;
            return; 
        }

        // ���� �߿� �ð��� �ȿ���
        if (!isAttack) time += Time.deltaTime;

        // ���� �غ� �� �Ǿ����� ����
        if (time >= enemyAttackSpeed && death_Jangseung.isAttackReady)
        {
            Debug.Log("��Ÿ� ǥ��");
            isAttack = true;

            // ��ġ�� ���� ���� ������ �ǰ���
            transform.position = death_Jangseung.attackTargetTrs;

            // ��Ÿ� ǥ��
            attackRange.SetActive(true);

            // ������ ǥ�õǰ� ������ �� �ڿ� ����
            jangseungAttackTime += Time.deltaTime;


            if(jangseungAttackTime >= jangseungAttackDelay)
            {
                Debug.Log("����");
                attackSprite.SetActive(true);

                // ��Ÿ� ��Ȱ��ȭ
                attackRange.SetActive(false);

                // �ݶ��̴��� Ȱ��ȭ ����
                enemyAttackCollider.enabled = true;

                // �ʱ�ȭ
                jangseungAttackTime = 0;
                isAttack = false;
                death_Jangseung.isAttackReady = false;
                time = 0;

                // �ݶ��̴� ��Ȱ��ȭ
                Invoke("Dealy", 0.1f);
            }
        }
    }


    // �÷��̾� ����
    void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾� ����
        if (collision.gameObject.CompareTag("Player") && !isAttack)
        {
            // ����
            player.DamagedHP(enemyDamage);
        }
    }

    void Dealy()
    {
        attackSprite.SetActive(false);
        enemyAttackCollider.enabled = false;
    }
}
