using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class LandShark : Enemy
{
    [Header("�����")]
    public bool isOut; //����
    public bool isIn; //�ẹ
    public bool isAttackReady; //���� �غ�
    public bool isStop; //����
    public float landSharkAttackSpeed;
    [SerializeField] LandSharkAttack landSharkAttack;


    PlayerController player; //�÷��̾�

    float enemyOriginHP;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }


    void Start()
    {
        EnemyInt();

        enemyOriginHP = enemyHp;

        // ó���� ������ ���� ����
        ChooseNewDirection();

        // �ֱ������� ���� ��ȯ
        StartCoroutine(ChangeDirectionRoutine());
    }

    float healTime;

    void Update()
    {
        //���� ü���� 0�����Ͻ�.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }
        //�ẹ�� ȸ�� �ð�
        healTime += Time.deltaTime;


        //�ẹ�� ����� �ϴ� �ൿ
        LandSharkStat();

        if(!isStop) { EnemyMove(); }
    }

    public override void EnemyMove()
    {
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;

        //���� ��
        if (isOut && !isIn && isTrace)
        {
            rigid.MovePosition(transform.position + enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        //�ִ� ä���� �ƴϸ� ����
        else if(isEnemyRun)
        {
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        else
        {
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }

    //�ẹ���� ��ȯ �ϴ� �޼���
    public void IsHide()
    {
        isIn = true;
        isOut = false;
    }
    //����� ��ȯ �ϴ� �޼���
    public void isHideOut()
    {
        isIn = false;
        isOut = true;
    }

    void LandSharkStat()
    {
        if (isIn) //�ẹ��
        {
            healTime += Time.deltaTime;

            //���� ü���� �ִ� ü���� �ƴ϶��
            if (enemyHp < enemyOriginHP) { isEnemyRun = true; }
            //�ִ�ü���̶��
            else if (enemyHp == enemyOriginHP)
            {
                isAttackReady = true;
            }

            if (healTime >= 1f)
            {
                enemyHp += 5;
                if (enemyHp >= enemyOriginHP) { enemyHp = enemyOriginHP; }

                healTime = 0f;
            }
        }

        else if (isOut) //�����
        {
            isAttackReady = false;
        }
    }

    public IEnumerator LandSharkJumpAttackMove()
    {
        float stopDistance = 0f;

        float distanceToTarget = Vector3.Distance(transform.position, enemyTrace.targetPos);
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;
        Vector3 targetPosition = enemyTrace.targetPos + (enemyTargetDir * stopDistance);
        
        //�Ÿ��� ���ߴ� �Ÿ����� ũ�� �ٵ� ��ǻ� true�� �ʿ� ���缭 ���߰� ���־���
        if (distanceToTarget > stopDistance)
        {
            //�������� ���� ���� ��������ŭ �Ҵ������
            landSharkAttack.enemyDamage = landSharkAttack.landSharkJumpAttackDamage;
            //�÷��̾ �հ� ���� �ϱ� ������ ��� ���� ��
            rigid.simulated = false;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition - Vector3.right,
                landSharkAttackSpeed * Time.deltaTime
            );
            //0.7��. �̰� �ൿ ���� ��������� �� ��
            yield return new WaitForSeconds(0.7f);
        }

        //���� Ʈ���� �ʱ�ȭ
        isAttackReady = false;
        isStop = false;
        enemyTrace.landSharkAttackTime = 0;

        //�ݶ��̴� ��Ȱ��ȭ
        landSharkAttack.enemyAttackCollider.enabled = false;
        
        //��Ȱ��ȭ �� simulated�� Ȱ��ȭ �����ش�.
        rigid.simulated = true;

        //�������� ���� �������� ��������
        landSharkAttack.enemyDamage = landSharkAttack.currentDamage;

        //�ẹ�� ������ ����� �ٲ���
        isHideOut();

        yield return null;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                WallNotCross();
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
                WallKnuckBack();
            }
        }
    }



}
