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
        if (!stealEnemy.isStealGold) time += Time.deltaTime;

        rotationColl();


        if (stealEnemy.isStealGold)
        {
            stealEnemy.anim.SetBool("isAttack", false);
        }

        //���� �ð��� �ư�, ��ǥ���� �Ÿ��� 3f ���� �۰ų� ������ ����
        else if (time >= enemyAttackSpeed && !stealEnemy.isStealGold
            && distance <= 3f)
        {
            //���ϸ��̼� Ȱ��ȭ
            stealEnemy.anim.SetBool("isAttack", true);

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

            // ���ϸ��̼� ����
            stealEnemy.anim.SetBool("isSteal", true);

            // ��������Ʈ ����
            stealEnemy.sp.sprite = stealEnemy.stealSprite;

            stealEnemy.isStealGold = true;
            stealEnemy.isAttack = true;

            // ����
            player.DamagedHP(enemyDamage * enemy.cloakingDamage);
            // ��� ������. -50% �̱⿡ �ִ� ���׹̳� / 2 �� ���־���
        }
    }

    public void Delay()
    {
        enemyAttackCollider.enabled = false;
        stealEnemy.isAttack = false;
        //stealEnemy.anim.SetBool("isAttack", false);
    }
}
