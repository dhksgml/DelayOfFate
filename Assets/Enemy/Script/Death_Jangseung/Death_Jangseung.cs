using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Jangseung : Enemy
{
    [Header("�������")]
    public float attackSeeTime;
    PlayerController player; //�÷��̾�
    // �� ���� �ڵ忡�� ������ ����
    [HideInInspector] public Vector3 attackTargetTrs; // ���� ��ġ
    [HideInInspector] public bool isAttackReady = false;
    [HideInInspector] public bool isSpawnReaper = false;
    

    void Awake()
    {
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
            StartCoroutine(EnemyDie());
        }

        if (isSpawnReaper)
        {
            enemyColl.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
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
}
