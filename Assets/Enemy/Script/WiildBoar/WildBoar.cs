using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoar : Enemy
{
    PlayerController player;
    public bool isAttack;
    public bool isOneDie; // 고기덩이 되는거 확인용
    [SerializeField] float dieHealth;
    public bool isStop;


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

    bool isReviving = false;

    void Update()
    {
        HpBarUpdate();

        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie || (enemyHp <= dieHealth && isReviving))
        {
            isReviving = true;
            //StartCoroutine(EnemyDie());
            NotDie();
        }

        if (!isDie)
        {
            EnemyMove();
        }
    }


    public override void EnemyMove()
    {
        if (isStop)
        {
            return;
        }

        //추적중일때 또는 한번 인식을 했을때
        else if (isTrace && !isDie)
        {
            // 에니메이션
            anim.SetBool("isWalk", true);

            // 스프라이트 회전
            EnemyTraceTurn();

            //한번 추적중이면 끝까지 따라옴
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            // 목표로 이동
            transform.Translate(enemyTargetDir * enemyMoveSpeed * 2 * Time.deltaTime);
        }

        //추적중이 아니면
        else if (!isTrace && !isDie)
        {
            // 에니메이션
            anim.SetBool("isWalk", true);

            //스프라이트 때문에 이걸 사용해줌
            EnemyNormalTurn();

            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }

    }

    // 부활 시간
    [SerializeField] float recoveryTimeSet;
    float recoveryTime;
    bool hasResetHealth = false;
    bool isdownAnim = false;

    public void NotDie()
    {
        // 처음만 초기화
        if (!hasResetHealth)
        {
            enemyHp = dieHealth;
            isStop = true;
            hasResetHealth = true;

            // 에니메이션
            anim.SetBool("isWalk", false);

            if (!isdownAnim)
            {
                anim.SetTrigger("isDown");
                isdownAnim = true;
            }
        }

        recoveryTime += Time.deltaTime;

        // 이때 죽으면 그냥 사망
        if (enemyHp <= 0 && !isDie)
        {
            Debug.Log("사망");
            isDie = true;
            isReviving = false;
            StartCoroutine(EnemyDie());
            return;
        }


        // 부활 시간 되면
        if (recoveryTime >= recoveryTimeSet)
        {
            recoveryTime = 0;
            // 최대채력 절반
            enemyHp = enemyMaxHp / 2;

            anim.SetTrigger("isWakeUp");
            isdownAnim = false;

            Invoke("Delay", 1f);
        }
    }

    void Delay()
    {
        // 트리거 초기화
        isStop = false;
        isReviving = false;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //적 피격 부분
            //이부분은 아마 적 공동 코드로 사용할 것 같다.
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && attack != null)
            {
                // 타입이 일치하면 즉사
                if (!attack.CheckWeaknessPassive() && attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //이부분 없다 나와서 일단 주석 처리 해주었음.
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

            //빛 반응
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
            if (collision.gameObject.CompareTag("Light"))
            {
                EnemyCloaking();
            }
        }
    }

}
