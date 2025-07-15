using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSharkAttack : EnemyAttack
{
    [Header("�����")]
    [SerializeField] LandShark landShark;
    [SerializeField] BoxCollider2D hideOutAttackColl;
    bool isOutAttackReady; //����� ���� Ȯ�ο�
    PlayerController player;
    public int landSharkJumpAttackDamage;
    public int currentDamage;

    void Awake()
    {
        //�÷��̾ ã�Ƽ� �������� ��
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        currentDamage = enemyDamage;
    }


    void Update()
    {
        transform.position = enemy.transform.position;
        //�Ÿ��� ����ϰ�
        float distance = Vector3.Distance(transform.position, player.transform.position);

        //���� �����̰�, �ẹ ���°� �ƴϸ�, �������Ͻ�
        if (landShark.isOut && !landShark.isIn && landShark.isTrace)
        {
            transform.position = enemy.transform.position;
            //���� �غ� �ȵ� �ÿ���
            if (!isOutAttackReady) time += Time.deltaTime;

            rotationColl();

            //���� �ð��� �ư�, ��ǥ���� �Ÿ��� 1f ���� �۰ų� ������ ����
            if (time >= enemyAttackSpeed && !isOutAttackReady
                && distance <= 5f)
            {
                //���� �غ� Ȱ��ȭ
                isOutAttackReady = true;
                //���ϸ��̼� Ȱ��ȭ
                Debug.Log(4);
                //���� �ݶ��̴� Ȱ��ȭ ����
                hideOutAttackColl.enabled = true;
                //�ð� �ʱ�ȭ
                time = 0;
                //0.5���� ���� �ݶ��̴� ��Ȱ��ȭ
                Invoke("AttackDelay", 0.5f);
            }
        }

    }

    public void AttackDelay()
    {
        Debug.Log(5);
        hideOutAttackColl.enabled = false;
        isOutAttackReady = false;
        //���ϸ��̼�
    }

    public void JumpAttack()
    {
        enemyDamage = 0;

        if (enemyAttackCollider is CircleCollider2D circleColl) { circleColl.radius = 3.0f; }
        //�ݶ��̴� Ȱ��ȭ�� ����
        enemyAttackCollider.enabled = true;
        landShark.isStop = true;

        StartCoroutine(landShark.LandSharkJumpAttackMove());
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                //�浹�� �������� ��
                player.currentHp -= enemyDamage;
            }
        }
    }
}
