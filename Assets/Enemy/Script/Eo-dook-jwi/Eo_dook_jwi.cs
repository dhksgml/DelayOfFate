using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eo_dook_jwi : Enemy
{
    [Header("Eo_dook_jwi")]
    [SerializeField] float waitTime = 0f;
    [SerializeField] bool isLighting;
    [SerializeField] bool isAction;
    [SerializeField] bool isStop;
    PlayerController player;

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
        //도착하지 않을 시
        if (!isArrive) 
        {
            EnemyMove();
        }
        //빛에 닿았을 시
        else if (isLighting)
        {
            EnemyMove();
        }

        //도착 했을시
        else if(isArrive)
        {
            if (!isStop) { EnemyMove(); }
            //거리를 계산해서
            float distance = Vector3.Distance(transform.position, enemyMovePoint[0].position);
            // 1f이하면
            if (distance < 1f)
            {
                //멈춰준 후
                isStop = true;

                //시간을 더해주고
                waitTime += Time.deltaTime;
                //10초가 지나면 false로 바꾸어 줌
                if (waitTime >= 10f)
                {
                    isArrive = false;
                    isStop = false;
                    waitTime = 0f;
                }
            }

        }
    }

    public override void EnemyMove()
    {
        //빛에 닿았을 시
        if(isLighting)
        {
            enemyMoveDir = -(player.transform.position - transform.position).normalized;
            rigid.MovePosition(transform.position + enemyMoveDir * enemyRunSpeed * Time.deltaTime);
        }

        //추적중이 아니면
        else if (!isTrace && !isDie && !isEnemyHit)
        {
            //스프라이트 때문에 이걸 사용해줌
            //EnemyNormalTurn2();

            //에니메이션, 추적 false로 바꾸어줌
            //anim.SetBool("isTrace", false);

            //MovePostion을 이용해 이동한다.
            rigid.MovePosition(transform.position + enemyMoveDir * enemyMoveSpeed * Time.deltaTime);
            EnemyMoveTarget();
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
