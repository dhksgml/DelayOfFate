using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    [SerializeField] Enemy enemy;

    void Update()
    {
        transform.position = enemy.transform.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && !collision.gameObject.CompareTag("Enemy"))
        {
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            Debug.Log(enemy.isEnemyHit);
            Debug.Log(attack);

            if (collision.gameObject.CompareTag("Attack") && !enemy.isEnemyHit && attack != null)
            {
                Debug.Log(2);
                // Ÿ���� ��ġ�ϸ� ���
                if (attack.attackType.ToString() == enemy.enemyWeakness.ToString())
                {
                    //�̺κ� ���� ���ͼ� �ϴ� �ּ� ó�� ���־���.
                    //attack.CheckWeakness();
                    enemy.enemyHp = 0f;
                }
                else
                {
                    enemy.enemyHp -= attack.damage;
                }

                enemy.EnemyHit(attack.damage);

            }
        }
    }
}
