using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse_Doll : Enemy
{
    [Header("저주인형")]
    [SerializeField] float stopTime;
    [SerializeField] float rushSpeed;
    [SerializeField] bool isFind;
    [SerializeField] float enemyDamage;
    [SerializeField] float healBanTime;

    PlayerController player;

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
        HpBarUpdate();

        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }
        else
        {
            EnemyMove();
        }
    }

    public override void EnemyMove()
    {
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        // 돌진
        if (isFind)
        {
            // 에니메이션

            //EnemyTraceTurn2();

            // 이동
            transform.Translate(enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
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
                if (!attack.CheckWeaknessPassive() && attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    attack.CheckWeakness();
                    Enemy_Weakness_Hit(attack.damage, attack.attackType.ToString(), enemyHp);
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                EnemyHit(attack.damage);

            }


            // 충돌 시 데미지를 부여
            if (collision.gameObject.CompareTag("Player"))
            {
                if (enemyDamage != 0 && isFind)
                {
                    player.DamagedHP(enemyDamage);
                    player.isHealBan = true;
                    player.healBanTime = healBanTime;
                    Destroy(gameObject);
                }
            }

            // 빛에 충돌시 은신 풀림
            if (collision.gameObject.CompareTag("Light"))
            {
                StartCoroutine(CurseDollLightHit());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            // 빛을 벗어나면 은신
            if (collision.gameObject.CompareTag("Light"))
            {
                CurseDollCloaking();
            }
        }
    }

    public void CurseDollCloaking()
    {
        Debug.Log("은신중");
        // 클로킹 중
        iscloaking = true;
    }

    IEnumerator CurseDollLightHit()
    {
        // 클로킹 중이 아님
        iscloaking = false;

        float elapsed = 0f;

        // stopTime 동안 기다리되, 클로킹이 되면 중단
        while (elapsed < stopTime)
        {
            if (iscloaking)
            {
                Debug.Log("중단");
                // 클로킹 상태가 되면 코루틴 즉시 종료
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 대기 중 클로킹이 되지 않았다면 아래 코드 실행
        enemyMoveSpeed = rushSpeed;
        isFind = true;
    }
}
