using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEnemy : Enemy
{
    PlayerController player;
    public bool isAttack;
    public bool isStealGold;
    bool isChasing = false;
    bool hasStolen = false; // 돈 훔친 후 최초 1회만 차감하기 위한 변수

    [SerializeField] float runTime;
    bool isSteal = false;

    public Sprite stealSprite;


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

        // 도주
        if (isEnemyRun && !isSteal)
        {
            isSteal = true;
            // 도주시 시체 안남도록 수정
            StartCoroutine(EnemySteal());
        }
        else if (!isDie)
        {
            EnemyMove();
        }
    }

    float currentRunTime = 0f;

    public override void EnemyMove()
    {
        //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;

        if(isStealGold && !hasStolen)
        {
            // 자신의 가치를 증가시켜줌
            enemyPrice += (int)(GameManager.Instance.Gold * 0.3f);

            // 게임 메니저에서 골드를 빼줌
            GameManager.Instance.Sub_Gold(GameManager.Instance.Gold * 0.3f);
            hasStolen = true;
        }

        //물건을 훔쳤을때
        if (isStealGold)
        {

            // 스프라이트 회전
            EnemyTraceTurn();

            if(rigid != null)
            {
                // 반대 방향으로 이동
                rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
            }



            currentRunTime += Time.deltaTime;

            if (currentRunTime >= runTime)
            {
                isEnemyRun = true;
            }

        }



        //추적중일때 또는 한번 인식을 했을때
        else if (isTrace && !isDie && !isEnemyHit || isChasing)
        {
            // 추적중 bool 값 true로
            isChasing = true;

            // 스프라이트 회전
            EnemyTraceTurn2();

            //한번 추적중이면 끝까지 따라옴
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            // 목표로 이동
            transform.Translate(enemyTargetDir * enemyMoveSpeed * 2 * Time.deltaTime);
        }

        //추적중이 아니면
        else if (!isTrace && !isDie && !isEnemyHit)
        {
            //스프라이트 때문에 이걸 사용해줌
            EnemyNormalTurn2();

            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }

    }


    //원래 이동속도를 저장할 변수
    //이부분은 플레이어의 빛의 범위에 Collder를 달아줘야 할 것 같음
    float originalMoveSpeed;

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

            //빛 반응
            if (collision.gameObject.CompareTag("Light"))
            {
                originalMoveSpeed = enemyMoveSpeed;
                enemyMoveSpeed = enemyMoveSpeed * 0.5f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Light"))
            {
                enemyMoveSpeed = originalMoveSpeed;
            }
        }
    }

    public IEnumerator EnemySteal()
    {
        // 사망시 이펙트 
        Instantiate(enemyDeathEffect, transform.position, Quaternion.identity);

        Color color = sp.color;

        //먼저 추적 범위와 공격 범위를 지워줌.
        Destroy(enemyTrace);
        Destroy(enemyAttack);
        Destroy(enemyColl);
        Destroy(rigid);

        // 이동속도 0으로 해서 움직이지 못하게
        enemyMoveSpeed = 0;


        //투명도 값을 1.0에서 0.01씩 뺴주면서 천천히 투명하게 해줌
        for (float i = 1.0f; i >= 0.0f; i -= 0.02f)
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(transform.parent.gameObject);
    }

}
