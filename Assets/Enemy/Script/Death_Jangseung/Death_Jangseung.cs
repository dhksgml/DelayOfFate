using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Jangseung : Enemy
{
    [Header("죽음장승")]
    public float attackSeeTime;
    PlayerController player; //플레이어
    // 적 추적 코드에서 가져올 예정
    [HideInInspector] public Vector3 attackTargetTrs; // 공격 위치
    [HideInInspector] public bool isAttackReady = false;
    [HideInInspector] public bool isSpawnReaper = false;
    

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();
    }

    void Update()
    {
        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }

        if (isSpawnReaper)
        {
            enemyColl.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // 타입이 일치하면 즉사
                if (attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //이부분 없다 나와서 일단 주석 처리 해주었음.
                    //attack.CheckWeakness();
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }
        }
    }

    public override void EnemyMove()
    {

    }
}
