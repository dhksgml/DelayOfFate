using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEnemy : Enemy
{
    PlayerController player;
    public bool isAttack;
    public bool isStealGold;
    bool isChasing = false;
    bool hasStolen = false; // �� ��ģ �� ���� 1ȸ�� �����ϱ� ���� ����

    [SerializeField] float runTime;
    bool isReadyRun = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
    }

    void Start()
    {
        EnemyInt();

        // ó���� ������ ���� ����
        ChooseNewDirection();

        // �ֱ������� ���� ��ȯ
        StartCoroutine(ChangeDirectionRoutine());
    }


    void Update()
    {
        //���� ü���� 0�����Ͻ�.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }

        // ����
        if (isEnemyRun)
        {
            StartCoroutine(EnemyDie());
        }

        EnemyMove();

    }

    float currentRunTime = 0f;

    public override void EnemyMove()
    {
        //�����ϴ� Ÿ���� ��ġ - �ڽ��� ��ġ�� ���� �� ����ȭ�� ���ش�
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;

        if(isStealGold && !hasStolen)
        {
            // ���� �޴������� ��带 ����
            GameManager.Instance.Sub_Gold(enemyData.CoinDeviation);
            hasStolen = true;
        }

        //������ ��������
        if (isStealGold)
        {
            // ��������Ʈ ȸ��
            EnemyTraceTurn();

            // �ݴ� �������� �̵�
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);


            currentRunTime += Time.deltaTime;

            if (currentRunTime >= runTime)
            {
                isEnemyRun = true;
            }

        }



        //�������϶� �Ǵ� �ѹ� �ν��� ������
        else if (isTrace && !isDie && !isEnemyHit || isChasing)
        {
            // ������ bool �� true��
            isChasing = true;

            // ��������Ʈ ȸ��
            EnemyTraceTurn2();

            //�ѹ� �������̸� ������ �����
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            // ��ǥ�� �̵�
            transform.Translate(enemyTargetDir * enemyMoveSpeed * 2 * Time.deltaTime);
        }

        //�������� �ƴϸ�
        else if (!isTrace && !isDie && !isEnemyHit)
        {
            //��������Ʈ ������ �̰� �������
            EnemyNormalTurn2();

            // ���� �������� �̵�
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }

    }


    //���� �̵��ӵ��� ������ ����
    //�̺κ��� �÷��̾��� ���� ������ Collder�� �޾���� �� �� ����
    float originalMoveSpeed;

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

                EnemyHit();
                Invoke("EnemyHitRegen", enemyHitTime);
            }

            //�� ����
            if (collision.gameObject.CompareTag("Light"))
            {
                originalMoveSpeed = enemyMoveSpeed;
                enemyMoveSpeed = enemyMoveSpeed * 0.5f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Light"))
            {
                enemyMoveSpeed = originalMoveSpeed;
            }
        }
    }
}
