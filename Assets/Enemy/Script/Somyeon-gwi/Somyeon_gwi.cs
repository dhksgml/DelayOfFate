using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Somyeon_gwi : Enemy
{
    [Header("소면귀")]
    PlayerController player;
    public Vector3 findItemVec;
    public int rageCount = 1; //분노 게이지 1, 2, 3까지

    [SerializeField] Sprite rageSp1;
    [SerializeField] Sprite rageSp2;
    [SerializeField] Sprite rageSp3;

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
            EnemyMove();
        }
        // 스프라이트 형식, 추후 애니메이션이 도입되면 변경해야함
        switch (rageCount)
        {
            case 1:
                sp.sprite = rageSp1;
                break;

            case 2:
                sp.sprite = rageSp2;
                break;

            case 3:
                sp.sprite = rageSp3;
                break;
        }
    }

    public override void EnemyMove()
    {
        //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        // 분노가 3이면
        if (rageCount == 3)
        {
            transform.Translate(enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
        }

        //추적중이 아니면
        else 
        {
            //스프라이트 때문에 이걸 사용해줌
            //EnemyNormalTurn2();

            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }

    }



    //플레이어가 닿았을 시
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            }


            //적 피격 부분
            //이부분은 아마 적 공동 코드로 사용할 것 같다.
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // 타입이 일치하면 즉사
                if (!attack.CheckWeaknessPassive() && attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    //이부분 없다 나와서 일단 주석 처리 해주었음.
                    attack.CheckWeakness();
                    Enemy_Weakness_Hit(attack.damage, attack.attackType.ToString(), enemyHp);
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                // 분노 게이지 바로 3
                rageCount = 3;

                EnemyHit(attack.damage);
            }
        }
    }
}
