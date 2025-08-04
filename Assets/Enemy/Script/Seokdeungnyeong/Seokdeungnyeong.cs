using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Seokdeungnyeong : Enemy
{
    PlayerController player;
    [SerializeField] GameObject lightObj;
    public bool isPlayer = false;

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

        if (isPlayer)
        {
            // �� Ȱ��ȭ
            lightObj.SetActive(true);

            if (enemyColl == null) { return; }

            // ����o
            enemyColl.enabled = false;
        }

        else if (!isPlayer)
        {
            // �� ��Ȱ��ȭ
            lightObj.SetActive(false);

            if (enemyColl == null) { return; }

            // ����x
            enemyColl.enabled = true;
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
}
