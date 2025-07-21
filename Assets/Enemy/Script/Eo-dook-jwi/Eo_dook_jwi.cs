using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eo_dook_jwi : Enemy
{
    [Header("Eo_dook_jwi")]
    [SerializeField] float waitTime = 0f;
    [SerializeField] float moveTime = 0f;
    [SerializeField] bool isLighting;
    [SerializeField] bool isAction;
    [SerializeField] bool isStop;
    [SerializeField] int enemyDamage;
    PlayerController player;



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

        if (!isStop) { EnemyMove(); }

        // 10�� �����̰� 5�� ������
        if (moveTime >= 10f)
        {
            //������ ��
            isStop = true;

            // ��� �ð��� ������
            waitTime += Time.deltaTime;

            //5�ʰ� ������ false�� �ٲپ� ��
            if (waitTime >= 5f)
            {
                // bool �ʱ�ȭ
                isArrive = false;
                isStop = false;

                // �ʱ�ȭ
                waitTime = 0f;
                moveTime = 0f;
            }
        }
    }

    public override void EnemyMove()
    {
        // ���� ������ �������� ����� �ϴ� ��������
        ////���� ����� ��
        //if(isLighting)
        //{
        //    enemyMoveDir = -(player.transform.position - transform.position).normalized;

        //    transform.Translate(enemyMoveDir * enemyRunSpeed * Time.deltaTime);
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

        //    // �ð��� ������
        //    moveTime += Time.deltaTime;
        //}

        //�������� �ƴϸ�
        if (!isTrace && !isDie && !isEnemyHit)
        {
            //��������Ʈ ������ �̰� �������
            //EnemyNormalTurn2();

            //���ϸ��̼�, ���� false�� �ٲپ���
            //anim.SetBool("isTrace", false);

            // ���� �������� �̵�
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);

            // �ð��� ������
            moveTime += Time.deltaTime;
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

                EnemyHit();

            }

            if (collision.gameObject.CompareTag("Light"))
            {
                //���� ������ true�� ���ְ� �������� ����
                isLighting = true;
            }

            // �浹 �� �������� �ο�
            if (collision.gameObject.CompareTag("Player"))
            {
                player.DamagedHP(enemyDamage);
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


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Light"))
            {
                if(!isAction)
                {
                    //������ ����� 3�� �� ���� ������ ����
                    StartCoroutine(Delay());
                }
            }
        }
    }

    IEnumerator Delay()
    {
        isAction = true;

        yield return new WaitForSeconds(3f);

        isLighting = false;
        isAction = false;
    }
}
