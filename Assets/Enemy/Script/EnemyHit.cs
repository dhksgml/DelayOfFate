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
                // 타입이 일치하면 즉사
                if (attack.attackType.ToString() == enemy.enemyWeakness.ToString())
                {
                    //이부분 없다 나와서 일단 주석 처리 해주었음.
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
