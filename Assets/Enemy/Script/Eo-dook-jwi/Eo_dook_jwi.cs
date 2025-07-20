using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eo_dook_jwi : Enemy
{
    [Header("Eo_dook_jwi")]
    [SerializeField] float waitTime = 0f;
    [SerializeField] float moveTime = 0f;
    [SerializeField] bool isLighting;
    [SerializeField] bool isAction;
    [SerializeField] bool isStop;
    [SerializeField] int enemyDamage;
    PlayerController player;



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

        if (!isStop) { EnemyMove(); }

        // 10초 움직이고 5초 멈춰줌
        if (moveTime >= 10f)
        {
            //멈춰준 후
            isStop = true;

            // 대기 시간을 더해줌
            waitTime += Time.deltaTime;

            //5초가 지나면 false로 바꾸어 줌
            if (waitTime >= 5f)
            {
                // bool 초기화
                isArrive = false;
                isStop = false;

                // 초기화
                waitTime = 0f;
                moveTime = 0f;
            }
        }
    }

    public override void EnemyMove()
    {
        // 빛에 닿으면 도망가는 기믹은 일단 제거해줌
        ////빛에 닿았을 시
        //if(isLighting)
        //{
        //    enemyMoveDir = -(player.transform.position - transform.position).normalized;

        //    transform.Translate(enemyMoveDir * enemyRunSpeed * Time.deltaTime);
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

        //    // 시간을 더해줌
        //    moveTime += Time.deltaTime;
        //}

        //추적중이 아니면
        if (!isTrace && !isDie && !isEnemyHit)
        {
            //스프라이트 때문에 이걸 사용해줌
            //EnemyNormalTurn2();

            //에니메이션, 추적 false로 바꾸어줌
            //anim.SetBool("isTrace", false);

            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);

            // 시간을 더해줌
            moveTime += Time.deltaTime;
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

                EnemyHit();

            }

            if (collision.gameObject.CompareTag("Light"))
            {
                //빛에 닿으면 true로 해주고 도망가게 해줌
                isLighting = true;
            }

            // 충돌 시 데미지를 부여
            if (collision.gameObject.CompareTag("Player"))
            {
                player.DamagedHP(enemyDamage);
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


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Light"))
            {
                if(!isAction)
                {
                    //빛에서 벗어나면 3초 뒹 빛이 꺼지게 해줌
                    StartCoroutine(Delay());
                }
            }
        }
    }

    IEnumerator Delay()
    {
        isAction = true;

        yield return new WaitForSeconds(3f);

        isLighting = false;
        isAction = false;
    }
}
