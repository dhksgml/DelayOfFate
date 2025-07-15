using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mumyeon_Gwi_Attack : EnemyAttack
{
    [Header("�����")]
    [SerializeField] Mumyeon_Gwi mumyeon_Gwi;
    PlayerController player;


    void Awake()
    {
        //�÷��̾ ã�Ƽ� �������� ��
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {


        transform.position = enemy.transform.position;
        //�Ÿ��� ����ϰ�
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (mumyeon_Gwi.isTrace)
        {
            // ���� �ð��� üũ
            time += Time.deltaTime;

            // ���� �ݶ��̴� ȸ��
            rotationColl();

            //���� �ð��� �ư�, ��ǥ���� �Ÿ��� 1f ���� �۰ų� ������ ����
            if (time >= enemyAttackSpeed && distance <= 7f)
            {
                //���ϸ��̼� Ȱ��ȭ

                //���� �ݶ��̴� Ȱ��ȭ ����
                enemyAttackCollider.enabled = true;

                // �ð��� �ʱ�ȭ����
                time = 0;

                //0.5���� ���� �ݶ��̴� ��Ȱ��ȭ
                Invoke("AttackDelay", 0.5f);
            }
        }
    }

    public void AttackDelay()
    {
        enemyAttackCollider.enabled = false;
        //���ϸ��̼�
    }
}
