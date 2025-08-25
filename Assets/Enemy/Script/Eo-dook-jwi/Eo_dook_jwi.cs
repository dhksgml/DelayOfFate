using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Eo_dook_jwi : Enemy
{
    [Header("Eo_dook_jwi")]
    [SerializeField] float waitTime = 0f;
    [SerializeField] bool isStop;
    [SerializeField] int enemyDamage;
    [SerializeField] int enemyOriginDamage = 0;
    PlayerController player;

    // 돌진 
    [SerializeField] public bool isRush = false;
    [HideInInspector] public Vector3 playerTrs;
    [Header("어둑쥐 돌진 스텟")]
    [SerializeField] float rushTime = 0f;
    [SerializeField] float enemyOriginSpeed = 0f;
    [SerializeField] float enemyRushSpeed = 0f;
    [SerializeField] int enemyRushDamage = 0;
    [SerializeField] bool isRushReady = false;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();

        enemyOriginSpeed = enemyMoveSpeed;

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

            EnemyMove();

            // 돌진 상태가 되면
            if (isRush && !isRushReady)
            {
                StartCoroutine(RushRoutine());
            }

        }
    }

    public override void EnemyMove()
    {
        if (isStop)
        {
            // 에니메이션
            anim.SetBool("isMove", false);
            return;
        }

        // 돌진
        else if (isRush)
        {
            // 에니메이션

            EnemyTraceTurn2();

            // 이동
            transform.Translate(enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
        }

        //추적중이 아니면
        else
        {
            // 에니메이션
            anim.SetBool("isMove", true);

            // 스프라이트 때문에 이걸 사용해줌
            EnemyNormalTurn2();

            //에니메이션, 추적 false로 바꾸어줌
            //anim.SetBool("isTrace", false);

            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);

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
                if (enemyDamage != 0) { player.DamagedHP(enemyDamage); }
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
                WallKnuckBack();
            }
        }
    }

    void PlayerTrsFind()
    {
        enemyTargetDir = (player.transform.position - transform.position).normalized;
    }


    IEnumerator RushRoutine()
    {
        // 준비 동작

        isRushReady = true;
        isStop = true;

        yield return new WaitForSeconds(waitTime);

        PlayerTrsFind();
        isStop = false;

        // 돌진 시작
        anim.SetBool("isRush", true);


        // 이동 속도 및 데미지 추가
        enemyMoveSpeed += enemyRushSpeed;
        enemyDamage = enemyRushDamage;

        yield return new WaitForSeconds(rushTime);

        // 돌진 종료
        anim.SetBool("isRush", false);

        // 초기화
        enemyMoveSpeed = enemyOriginSpeed;
        enemyDamage = enemyOriginDamage;

        ChooseNewDirection();

        isRush = false;
        isRushReady = false;
    }
}
