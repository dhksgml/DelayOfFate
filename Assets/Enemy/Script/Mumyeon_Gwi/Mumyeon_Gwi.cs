using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mumyeon_Gwi : Enemy
{
    [Header("�����")]
    PlayerController player;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyInt(); // �޼��� �Ⱥ��� Enemy ��ũ��Ʈ Ȯ�� 0603
    }

    void Start()
    {
        // ó���� ������ ���� ����
        ChooseNewDirection();

        // �ֱ������� ���� ��ȯ
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        EnemyMove();
    }

    public override void EnemyMove()
    {
        // ���� ���̸�
        if(isTrace)
        {
            // �÷��̾��� ��ġ�� �ڱ� �ڽ��� ��ġ�� ���� �� ����ȭ�� ����
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            // �׸��� ������ ���ش�.
            transform.Translate(enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        else
        {
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
        
    }
}
