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
    bool isSteal = false;

    public Sprite stealSprite;


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
        if (isEnemyRun && !isSteal)
        {
            isSteal = true;
            // ���ֽ� ��ü �ȳ����� ����
            StartCoroutine(EnemySteal());
        }
        else if (!isDie)
        {
            EnemyMove();
        }
    }

    float currentRunTime = 0f;

    public override void EnemyMove()
    {
        //�����ϴ� Ÿ���� ��ġ - �ڽ��� ��ġ�� ���� �� ����ȭ�� ���ش�
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;

        if(isStealGold && !hasStolen)
        {
            // �ڽ��� ��ġ�� ����������
            enemyPrice += (int)(GameManager.Instance.Gold * 0.3f);

            // ���� �޴������� ��带 ����
            GameManager.Instance.Sub_Gold(GameManager.Instance.Gold * 0.3f);
            hasStolen = true;
        }

        //������ ��������
        if (isStealGold)
        {

            // ��������Ʈ ȸ��
            EnemyTraceTurn();

            if(rigid != null)
            {
                // �ݴ� �������� �̵�
                rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
            }



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

                EnemyHit(attack.damage);
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

    public IEnumerator EnemySteal()
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

        Destroy(transform.parent.gameObject);
    }

}
