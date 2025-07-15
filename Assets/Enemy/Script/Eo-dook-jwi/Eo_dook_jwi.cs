using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eo_dook_jwi : Enemy
{
    [Header("Eo_dook_jwi")]
    [SerializeField] float waitTime = 0f;
    [SerializeField] bool isLighting;
    [SerializeField] bool isAction;
    [SerializeField] bool isStop;
    PlayerController player;

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
        //�������� ���� ��
        if (!isArrive) 
        {
            EnemyMove();
        }
        //���� ����� ��
        else if (isLighting)
        {
            EnemyMove();
        }

        //���� ������
        else if(isArrive)
        {
            if (!isStop) { EnemyMove(); }
            //�Ÿ��� ����ؼ�
            float distance = Vector3.Distance(transform.position, enemyMovePoint[0].position);
            // 1f���ϸ�
            if (distance < 1f)
            {
                //������ ��
                isStop = true;

                //�ð��� �����ְ�
                waitTime += Time.deltaTime;
                //10�ʰ� ������ false�� �ٲپ� ��
                if (waitTime >= 10f)
                {
                    isArrive = false;
                    isStop = false;
                    waitTime = 0f;
                }
            }

        }
    }

    public override void EnemyMove()
    {
        //���� ����� ��
        if(isLighting)
        {
            enemyMoveDir = -(player.transform.position - transform.position).normalized;
            rigid.MovePosition(transform.position + enemyMoveDir * enemyRunSpeed * Time.deltaTime);
        }

        //�������� �ƴϸ�
        else if (!isTrace && !isDie && !isEnemyHit)
        {
            //��������Ʈ ������ �̰� �������
            //EnemyNormalTurn2();

            //���ϸ��̼�, ���� false�� �ٲپ���
            //anim.SetBool("isTrace", false);

            //MovePostion�� �̿��� �̵��Ѵ�.
            rigid.MovePosition(transform.position + enemyMoveDir * enemyMoveSpeed * Time.deltaTime);
            EnemyMoveTarget();
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
