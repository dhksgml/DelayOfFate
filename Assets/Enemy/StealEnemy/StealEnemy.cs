using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEnemy : Enemy
{
    PlayerController player;
    public bool isAttack;
    public bool isSteal;
    bool isChasing = false;
    bool hasStolen = false; // 돈 훔친 후 최초 1회만 차감하기 위한 변수

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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

        EnemyMove();

    }

    public override void EnemyMove()
    {
        //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;

        if(isSteal && !hasStolen)
        {
            GameManager.Instance.Sub_Gold(enemyData.CoinDeviation);
            hasStolen = true;
        }

        //물건을 훔쳤을때
        if (isSteal)
        {
            EnemyTraceTurn();
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }



        //추적중일때 또는 한번 인식을 했을때
        else if (isTrace && !isDie && !isEnemyHit || isChasing)
        {
            isChasing = true;
            EnemyTraceTurn2();

            //한번 추적중이면 끝까지 따라옴
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            //에니메이션, 추적 true 바꾸어줌
            //anim.SetBool("isTrace", true);

            rigid.MovePosition(transform.position + enemyTargetDir * enemyMoveSpeed * Time.deltaTime);

            if (enemyMovePoint.Length > 0)
            {
                enemyMoveDir = (enemyMovePoint[enemyCurrentMove].position - transform.position).normalized;
            }  
        }

        //추적중이 아니면
        else if (!isTrace && !isDie && !isEnemyHit)
        {
            //스프라이트 때문에 이걸 사용해줌
            EnemyNormalTurn2();

            //에니메이션, 추적 false로 바꾸어줌
            //anim.SetBool("isTrace", false);

            //MovePostion을 이용해 이동한다.
            if (enemyMovePoint.Length > 0)
            {
                rigid.MovePosition(transform.position + enemyMoveDir * enemyMoveSpeed * Time.deltaTime);
            }
            
            EnemyMoveTarget();
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

                EnemyHit();
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
}
