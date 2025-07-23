using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper : Enemy
{
    PlayerController player; //�÷��̾�

    //�������µ� �ɸ��� �ð�
    [SerializeField] float powerUpTime;
    //�̵��ӵ� ���ġ
    [SerializeField] float speedUpValue;
    //ũ�� ���ġ
    [SerializeField] Vector3 scaleUpValue;
    //�ݶ��̴� ���ġ
    [SerializeField] float colliderUpValue;
    //�ݶ��̴�
    [SerializeField] CircleCollider2D circleCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyInt();
    }

    void Start()
    {
        // ���»��� ��ȯ�� ������¿��ٰ� alpha�� ȸ��
        StartCoroutine("ReaperAlpha");
    }

    void Update()
    {
        EnemyMove();
    }

    //Ȱ��ȭ�� ����
    void OnEnable()
    {
        StartCoroutine(ScaleAndSpeedUp());
    }

    //�÷��̾ ����� ��
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision != null)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                // ������ ó�� �κ�
                player.DamagedHP(444444);
            }
        }

        //�� �ǰ� �κ�
        //�̺κ��� �Ƹ� �� ���� �ڵ�� ����� �� ����.
        Attack_sc attack = collision.GetComponent<Attack_sc>();

        if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
        {
            // ����� ������ �����Ƿ� �ּ�ó��
            // Ÿ���� ��ġ�ϸ� ���
            //if (attack.attackType.ToString() == enemyWeakness.ToString())
            //{
            //    //�̺κ� ���� ���ͼ� �ϴ� �ּ� ó�� ���־���.
            //    //attack.CheckWeakness();
            //    enemyHp = 0f;
            //}
            //else
            //{
            //    enemyHp -= attack.damage;
            //}

            // ������ ó��
            enemyHp -= attack.damage;

            EnemyHit();
            Invoke("EnemyHitRegen", enemyHitTime);
        }
    }

    public override void EnemyMove()
    {
        //�����ϴ� Ÿ���� ��ġ - �ڽ��� ��ġ�� ���� �� ����ȭ�� ���ش�
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        // ��������Ʈ ȸ�� ó��
        EnemyTraceTurn2();

        rigid.MovePosition(transform.position + enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
    }

    //40�ʸ��� Ŀ���� �ϱ� ����
    IEnumerator ScaleAndSpeedUp()
    {
        while (true)
        {
            //������ �ð���ŭ �����̸� ��
            yield return new WaitForSeconds(powerUpTime);

            // ũ�� ����
            transform.localScale += scaleUpValue;

            // �̵��ӵ� ����
            enemyMoveSpeed += speedUpValue;

            // �ݶ��̴� ũ�� ����
            circleCollider.radius += colliderUpValue;
        }
    }

    // ���»��� ���� ����
    public IEnumerator ReaperAlpha()
    {
        Color color = sp.color;

        // ���ĸ� 0���� ����
        color.a = 0f;
        sp.color = color;

        // ���ĸ� 0.0 �� 1.0���� ������Ŵ
        for (float i = 0.0f; i <= 1.0f; i += 0.01f)
        {
            color.a = i;
            sp.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
