using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEnemyAttack : EnemyAttack
{
    [SerializeField] StealEnemy stealEnemy;
    PlayerController player;

    void Awake()
    {
        //�÷��̾ ã�Ƽ� �������� ��
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        //�Ÿ��� ����ϰ�
        float distance = Vector3.Distance(transform.position, player.transform.position);

        transform.position = enemy.transform.position;

        //��ġ�� �ʾ����� ����
        if (!stealEnemy.isSteal) time += Time.deltaTime;

        rotationColl();

        //���� �ð��� �ư�, ��ǥ���� �Ÿ��� 1f ���� �۰ų� ������ ����
        if (time >= enemyAttackSpeed && !stealEnemy.isSteal
            && distance <= 1f)
        {
            //���ϸ��̼� Ȱ��ȭ
            //stealEnemy.anim.SetBool("isAttack", true);
            //���� �ݶ��̴� Ȱ��ȭ ����
            enemyAttackCollider.enabled = true;
            enemy.isEnemyAttack = false;
            time = 0;
            Invoke("Delay", 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !stealEnemy.isAttack)
        {
            stealEnemy.isSteal = true;
            stealEnemy.isAttack = true;
        }
    }

    public void Delay()
    {
        enemyAttackCollider.enabled = false;
        stealEnemy.isAttack = false;
        //stealEnemy.anim.SetBool("isAttack", false);
    }
}
