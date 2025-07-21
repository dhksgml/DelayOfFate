using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using static Enemy_data;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
//��������Ʈ ������ ȸ���̳� ���ϸ��̼� ó���� ����� ��

//���� ���� �ӽ÷� 5���� �����Ͽ���.
//��, �����, ����, ��ũ��, ȣ����
public enum EnemyWeakness 
{
    Sword,
    Bat,
    Paper,
    Scroll,
    Bottle,
    None
}

// ���� ��� �븻, �߰�����, ������ ����
public enum EnemyMobType
{
    Normal,
    MiddleBoss,
    Boss
}

public abstract class Enemy     : MonoBehaviour
{
    [Header("�� ������")]
    public Enemy_data enemyData;

    //�� ��ũ��Ʈ�� �ݶ��̴� �� ���� �־���� �� �� ����, �ǰݰ� �浹�� ����
    [Header("Enemy Stat")]
    public string        enemyName; //���� �̸�
    public float         enemyHp;  //���� ü��
    public float         enemyMoveSpeed; //���� �̵��ӵ�
    public float         enemyRunSpeed; //����ġ�� ���� �̵��ӵ�
    
    [Space(20f)]
    //���� ���� �ּ�ġ�� �ִ�ġ
    public int           enemyPriceMin;
    public int           enemyPriceMax;
    public int           enemyPrice; //���� ����

    [Space(20f)]
    //���� ���� �ּ�ġ�� �ִ�ġ
    public int           enemyHeightMin;
    public int           enemyHeightMax;
    public int           enemyHeight;

    [Space(20f)]
    // �������� Ȯ���ϱ� ����
    public EnemyMobType enemyMobType;

    [Space(20f)]
    [Range(0f, 10f)]
    public float         enemyHitTime; //���� �ǰݴ������� ���� �ð�
    [Space(20f)]
    public EnemyWeakness enemyWeakness; //���� ����
    [Space(20f)]
    public Classification enemyType; //���� Ÿ��

    [Header("Enemy Move Point")]
    public int           enemyCurrentMove; //���� ���� �̵��� Ƚ��
    public Transform[]   enemyMovePoint; //���� ������ ��Ҹ� ��ȸ�ϰ� �������

    [Header("Trigger")]
    public bool          isTrace; //�÷��̸� ���� ������ Ȯ���ϴ� bool
    public bool          isEnemyRun; //���� ������ �������� ����
    public bool          isEnemyAttack; //���� ������ �ϳ� Ȯ���ϴ� bool
    public bool          isEnemyHit; //�ǰ��߳� Ȯ���ϴ� bool
    public bool          isEnemyChase; //������ �ʾƵ� ���� �i�ư����� Ȯ���ϴ� bool
    public bool          isArrive; //��������Ʈ�� ���� ���� �� ������ �� �������� Ȯ���ϴ� bool

    //�⺻������ ���� �׾������� ���� �ǰݴ������� �������� �ʰ� �����س����ϴ�.

    [Header("Reference")]
    
    public EnemyTrace           enemyTrace;
    public EnemyAttack          enemyAttack;    
    public SpriteRenderer       sp;
    public Animator             anim;
    public GameObject           enemyCorpse; //�� ��ü
    public GameObject           enemySelf;
    public Collider2D           enemyColl;

    [HideInInspector] public Vector3       enemyTargetDir; //���� Ÿ�� ����

    [HideInInspector] public bool                 isDie = false;
    [HideInInspector] public Rigidbody2D          rigid;
    [HideInInspector] public Vector3              enemyMoveDir; //���� ���� �̵� ����
    public Vector3              moveDirection;

    //���۽� �ʱ�ȭ ���ִ� �Լ�
    public void EnemyInt()
    {
        enemyName = enemyData.Name;
        enemyHeight = enemyData.Weight;
        //�̰� �������־��� enemyData�κ�
        enemyWeakness = enemyData.weakness;
        enemyType = enemyData.classification;

        // �븻
        if (enemyMobType == EnemyMobType.Normal)
        {
            //ü�µ� ���� ������ - ������ + ���������� ���� ������ �Ҵ�����
            enemyHp = Random.Range(enemyData.Hp - enemyData.HpDeviation,
                                   enemyData.Hp + enemyData.HpDeviation + 1);
            //�ϴ� ���� �� �������� �⺻ ���ΰ� - ������ ~~ ���ΰ� + �������̷��� ���־���
            enemyPrice = Random.Range(enemyData.Coin - enemyData.CoinDeviation,
                                      enemyData.Coin + enemyData.CoinDeviation + 1);
        }
        // �߰� ����
        else if (enemyMobType == EnemyMobType.MiddleBoss)
        {
            // �߰� ������ 2��
            //ü�µ� ���� ������ - ������ + ���������� ���� ������ �Ҵ�����
            enemyHp = Random.Range((enemyData.Hp - enemyData.HpDeviation) * 2,
                                   (enemyData.Hp + enemyData.HpDeviation * 2) + 1);
            //�ϴ� ���� �� �������� �⺻ ���ΰ� - ������ ~~ ���ΰ� + �������̷��� ���־���
            enemyPrice = Random.Range(enemyData.Coin - enemyData.CoinDeviation * 2,
                                      (enemyData.Coin + enemyData.CoinDeviation * 2 ) + 1);

            // ũ��� �ι��
            transform.localScale = new Vector3(2, 2, 2);
        }

        // ����
        else if (enemyMobType == EnemyMobType.Boss)
        {

        }


    }

    //���� �Ⱦ�
    public void EnemyPrice()
    {
        //���� ������ ������. int���̱⿡ �ִ밪�� +1�� �����־���
        enemyPrice = Random.Range(enemyPriceMin, enemyPriceMax + 1);
    }
    //���� �Ⱦ�
    public void EnemyHeight()
    {
        enemyHeight = Random.Range(enemyHeightMin, enemyHeightMax + 1);
    }

    public abstract void EnemyMove();

    #region ��ǥ �̵�
    //���� �̵��� �ϴ� �޼���
    public void EnemyMoveTarget()
    {
        if (enemyMovePoint.Length > 0)
        {
            enemyMoveDir = (enemyMovePoint[enemyCurrentMove].position - transform.position).normalized;

            //���� ���� ���� �̵� ����Ʈ�� �Ÿ��� 0.02f * enemyMoveSpeed���� ������
            if (Vector3.Distance(transform.position, enemyMovePoint[enemyCurrentMove].position) < 0.02f * enemyMoveSpeed)
            {
                //�̵� ����Ʈ ����
                EnemyNextMove();
            }
        }
    }

    //���� ���� �̵� ������ �����ִ� �޼���
    public void EnemyNextMove()
    {
        //���� �̵� Ƚ���� ���� �̵� ����Ʈ ���̺��� ������
        if (enemyCurrentMove < enemyMovePoint.Length - 1 )
        {
            //�̵� Ƚ���� ++ ���� ��
            enemyCurrentMove++;
            isArrive = false;
        }
        //���� ������ �̵�����Ʈ��
        else
        {
            //���� �̵� Ƚ���� 0���� ���� ��
            enemyCurrentMove = 0;
            isArrive = true;
        }
        //�̵� ����Ʈ�� ��ġ�� �ڱ� �ڽ��� �Ÿ��� ���� �� ����ȭ�� �ϰ� Vector3 ������ �������ش�.
        enemyMoveDir = (enemyMovePoint[enemyCurrentMove].position - transform.position).normalized;
    }
    #endregion

    #region ������ �̵�
    //360���������� �̵�
    public void ChooseNewDirection()
    {
        // 0~360�� �� ������ ������ �̵� ���� ����
        float angle = Random.Range(0f, 360f);
        float radian = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }

    public IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f)); // �ֱ������� ���� ��ȯ �������� ���־���
            ChooseNewDirection();
        }
    }
    #endregion

    #region �ǰ�
    //���� �÷��̾�� �ǰݴ�������. 
    public void EnemyHit()
    {
        isEnemyHit = true;

        //�ǰݽ� �������̰� ��� ��������
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;

        //��������Ʈ �߰��� ��� 0603
        //Color color = sp.color;
        //color.a = 0.5f;
        //sp.color = color;

        Invoke("EnemyHitRegen", enemyHitTime);
    }

    //���� �ǰݴ����� ���� ���ƿö�
    public void EnemyHitRegen()
    {
        //��������Ʈ �߰��� ��� 0603
        //Color color = sp.color;
        //color.a = 1f;
        //sp.color = color;

        isEnemyHit = false;
        //�����صа� �ٽ� Ǯ����
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    #endregion

    #region ���ó��
    //���� ������ �����
    public IEnumerator EnemyDie()
    {
        Color color = sp.color;

        //���� ���� ������ ���� ������ ������.
        Destroy(enemyTrace);
        Destroy(enemyAttack);


        //���� ���� 1.0���� 0.01�� ���ָ鼭 õõ�� �����ϰ� ����
        for (float i = 1.0f; i >= 0.0f; i -= 0.01f )
        {
            color.a = i;
            sp.color = color;
            //�����̸� ���� �ڷ�ƾ�� �������
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(transform.parent.gameObject);
        //EnemyCorpseSummon();
    }

    //���� ������ ��ü�� ��ȯ�ϴ� �޼���
    void EnemyCorpseSummon()
    {
        //������ �� ��ü ���ӿ�����Ʈ�� ������ ��������
        GameObject corpse = Instantiate(this.enemyCorpse, transform.position, transform.rotation);
        //�� �� EnemyCorpse ������Ʈ�� ��������
        EnemyCorpse enemyCorpse = corpse.GetComponent<EnemyCorpse>();
        //�̸� ������ ������ ��������
        enemyCorpse.corpseName = $"{enemyName}�� ��ȥ";
        enemyCorpse.corpseGold = enemyPrice;
        enemyCorpse.corpseHeight = enemyHeight;

        //���������� �ڱ� �ڽ��� ������
        //�θ� ������Ʈ�� ������ ������
        Destroy(transform.parent.gameObject);
    }

    #endregion

    #region ȸ�� ó��

    //���� ��������Ʈ ȸ���� ����
    public void EnemyNormalTurn()
    {
        //���� ���ƴٴҶ�
        if (moveDirection.x < 0) { sp.flipX = true; }
        else if (moveDirection.x > 0) { sp.flipX = false; }

    }

    //��������Ʈ ������ �ΰ� �������
    public void EnemyNormalTurn2()
    {
        //���� ���ƴٴҶ�
        if (moveDirection.x < 0) { sp.flipX = false; }
        else if (moveDirection.x > 0) { sp.flipX = true; }

    }

    //���� �i����
    public void EnemyTraceTurn()
    {
        if (enemyTargetDir.x < 0) { sp.flipX = true; }
        else if (enemyTargetDir.x > 0) { sp.flipX = false; }
    }

    //���� ��ġ�� ��������
    public void EnemyTraceTurn2()
    {
        if (enemyTargetDir.x < 0) { sp.flipX = false; }
        else if (enemyTargetDir.x > 0) { sp.flipX = true; }
    }
    #endregion

    #region �� �浹 ó��

    //  ���� ���Ѱ� ���ִ� �޼���
    public void WallNotCross()
    {
        enemyColl.isTrigger = false;
    }

    // �ٽ� ������� ���ִ� �޼���
    public void WallCollOrigin()
    {
        enemyColl.isTrigger = true;
    }

    #endregion



    //������
    //void EnemyCorpseSummon()
    //{
    //    //������ �� ��ü ���ӿ�����Ʈ�� ������ ��������
    //    GameObject corpse = Instantiate(this.enemyCorpse, transform.position, transform.rotation);
    //    //�� �� EnemyCorpse ������Ʈ�� ��������
    //    Item enemyCorpse = corpse.GetComponent<Item>();//itemData
    //    //�̸� ������ ������ ��������
    //    enemyCorpse.itemName = $"{enemyName}�� ��ȥ";
    //    enemyCorpse.Coin = enemyPrice;
    //    enemyCorpse.Weight = enemyHeight;
    //}
}
