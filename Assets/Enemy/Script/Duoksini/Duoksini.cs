using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duoksini : Enemy
{
    [Header("두억시니")]
    PlayerController player;
    public float attackSeeTime;
    public bool isAttack = false;
    [HideInInspector] public Vector3 attackTargetTrs;
    [HideInInspector] public bool isAttackReady = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }

    void Start()
    {
        EnemyInt();

        // 처음에 랜덤한 방향 설정
        ChooseNewDirection();

        // 주기적으로 방향 전환
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }
        else
        {
            // 공격시 멈춤
            if (isAttack)
            {
                return;
            }

            // 공격이 아니면 움직임
            else if (!isAttack)
            {
                EnemyMove();
            }
        }
    }

    public override void EnemyMove()
    {
        //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        ////추적중일때
        //if (isTrace && !isDie && !isEnemyHit)
        //{
        //    //EnemyTraceTurn2();

        //    //에니메이션, 추적 true 바꾸어줌
        //    //anim.SetBool("isTrace", true);

        //    // 목표를 향해 이동
        //    transform.Translate(enemyTargetDir * enemyMoveSpeed * 3 * Time.deltaTime);

        //}

        ////추적중이 아니면
        //else if (!isTrace && !isDie && !isEnemyHit)
        //{
        //    //스프라이트 때문에 이걸 사용해줌
        //    //EnemyNormalTurn2();

        //    //에니메이션, 추적 false로 바꾸어줌
        //    //anim.SetBool("isTrace", false);

        //    // 현재 방향으로 이동
        //    transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        //}

        //현재 방향으로 이동
        transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //적 피격 부분
            //이부분은 아마 적 공동 코드로 사용할 것 같다.
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

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                WallCollOrigin();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                WallNotCross();
            }
        }
    }
}
