using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duoksini : Enemy
{
    [Header("�ξ�ô�")]
    PlayerController player;
    public float attackSeeTime;
    public bool isAttack = false;
    [HideInInspector] public Vector3 attackTargetTrs;
    [HideInInspector] public bool isAttackReady = false;

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
        else
        {
            // ���ݽ� ����
            if (isAttack)
            {
                return;
            }

            // ������ �ƴϸ� ������
            else if (!isAttack)
            {
                EnemyMove();
            }
        }
    }

    public override void EnemyMove()
    {
        //�����ϴ� Ÿ���� ��ġ - �ڽ��� ��ġ�� ���� �� ����ȭ�� ���ش�
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        ////�������϶�
        //if (isTrace && !isDie && !isEnemyHit)
        //{
        //    //EnemyTraceTurn2();

        //    //���ϸ��̼�, ���� true �ٲپ���
        //    //anim.SetBool("isTrace", true);

        //    // ��ǥ�� ���� �̵�
        //    transform.Translate(enemyTargetDir * enemyMoveSpeed * 3 * Time.deltaTime);

        //}

        ////�������� �ƴϸ�
        //else if (!isTrace && !isDie && !isEnemyHit)
        //{
        //    //��������Ʈ ������ �̰� �������
        //    //EnemyNormalTurn2();

        //    //���ϸ��̼�, ���� false�� �ٲپ���
        //    //anim.SetBool("isTrace", false);

        //    // ���� �������� �̵�
        //    transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        //}

        //���� �������� �̵�
        transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
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

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                WallCollOrigin();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                WallNotCross();
            }
        }
    }
}
