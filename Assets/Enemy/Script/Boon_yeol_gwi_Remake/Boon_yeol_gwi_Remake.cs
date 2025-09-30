using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boon_yeol_gwi_Remake : Enemy
{

    [Header("분열귀")]
    public bool isFind;

    PlayerController player;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();

        // 이동 방향 설정
        ChooseNewDirection();
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        HpBarUpdate();

        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;

            StartCoroutine(EnemyDie());

        }

        EnemyMove();
    }

    public override void EnemyMove()
    {
        // 추격 목표
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        if (isFind)
        {
            Debug.Log(1);
            EnemyTraceTurn2();

            transform.Translate(enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
        }

        // 그 외
        else if (!isFind)
        {
            Debug.Log(2);
            EnemyNormalTurn2();

            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //피격 효과
            Attack_sc attack = collision.GetComponent<Attack_sc>();
            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // 타입이 일치하면 즉사
                if (!attack.CheckWeaknessPassive() && attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    Enemy_Weakness_Hit(attack.damage, attack.attackType.ToString(), enemyHp);
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }


            // 빛에 충돌시 은신 풀림
            if (collision.gameObject.CompareTag("Light"))
            {
                EnemyLightHit();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            // 빛에서 벗어날시 은신
            if (collision.gameObject.CompareTag("Light"))
            {
                EnemyCloaking();
            }
        }
    }
}
