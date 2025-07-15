using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEnemy : Enemy
{
    PlayerController player;
    public bool isAttack;
    public bool isSteal;
    bool isChasing = false;
    bool hasStolen = false; // �� ��ģ �� ���� 1ȸ�� �����ϱ� ���� ����

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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

        EnemyMove();

    }

    public override void EnemyMove()
    {
        //�����ϴ� Ÿ���� ��ġ - �ڽ��� ��ġ�� ���� �� ����ȭ�� ���ش�
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;

        if(isSteal && !hasStolen)
        {
            GameManager.Instance.Sub_Gold(enemyData.CoinDeviation);
            hasStolen = true;
        }

        //������ ��������
        if (isSteal)
        {
            EnemyTraceTurn();
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }



        //�������϶� �Ǵ� �ѹ� �ν��� ������
        else if (isTrace && !isDie && !isEnemyHit || isChasing)
        {
            isChasing = true;
            EnemyTraceTurn2();

            //�ѹ� �������̸� ������ �����
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            //���ϸ��̼�, ���� true �ٲپ���
            //anim.SetBool("isTrace", true);

            rigid.MovePosition(transform.position + enemyTargetDir * enemyMoveSpeed * Time.deltaTime);

            if (enemyMovePoint.Length > 0)
            {
                enemyMoveDir = (enemyMovePoint[enemyCurrentMove].position - transform.position).normalized;
            }  
        }

        //�������� �ƴϸ�
        else if (!isTrace && !isDie && !isEnemyHit)
        {
            //��������Ʈ ������ �̰� �������
            EnemyNormalTurn2();

            //���ϸ��̼�, ���� false�� �ٲپ���
            //anim.SetBool("isTrace", false);

            //MovePostion�� �̿��� �̵��Ѵ�.
            if (enemyMovePoint.Length > 0)
            {
                rigid.MovePosition(transform.position + enemyMoveDir * enemyMoveSpeed * Time.deltaTime);
            }
            
            EnemyMoveTarget();
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
