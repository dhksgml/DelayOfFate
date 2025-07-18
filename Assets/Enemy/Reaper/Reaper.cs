using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper : Enemy
{
    PlayerController player; //플레이어

    //강해지는데 걸리는 시간
    [SerializeField] float powerUpTime;
    //이동속도 상승치
    [SerializeField] float speedUpValue;
    //크기 상승치
    [SerializeField] Vector3 scaleUpValue;
    //콜라이더 상승치
    [SerializeField] float colliderUpValue;
    //콜라이더
    [SerializeField] CircleCollider2D circleCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyInt();
    }

    void Update()
    {
        EnemyMove();
    }

    //활성화시 실행
    void OnEnable()
    {
        StartCoroutine(ScaleAndSpeedUp());
    }

    //플레이어가 닿았을 시
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision != null)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                // 데미지 처리 부분
                player.DamagedHP(444444);
            }
        }

        //적 피격 부분
        //이부분은 아마 적 공동 코드로 사용할 것 같다.
        Attack_sc attack = collision.GetComponent<Attack_sc>();

        if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
        {
            // 사신은 약점이 없으므로 주석처리
            // 타입이 일치하면 즉사
            //if (attack.attackType.ToString() == enemyWeakness.ToString())
            //{
            //    //이부분 없다 나와서 일단 주석 처리 해주었음.
            //    //attack.CheckWeakness();
            //    enemyHp = 0f;
            //}
            //else
            //{
            //    enemyHp -= attack.damage;
            //}

            // 데미지 처리
            enemyHp -= attack.damage;

            EnemyHit();
            Invoke("EnemyHitRegen", enemyHitTime);
        }
    }

    public override void EnemyMove()
    {
        //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        //EnemyTraceTurn2();

        //에니메이션, 추적 true 바꾸어줌
        //anim.SetBool("isTrace", true);

        rigid.MovePosition(transform.position + enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
    }

    //40초마다 커지게 하기 위함
    IEnumerator ScaleAndSpeedUp()
    {
        while (true)
        {
            //지정한 시간만큼 딜레이를 줌
            yield return new WaitForSeconds(powerUpTime);

            // 크기 증가
            transform.localScale += scaleUpValue;

            // 이동속도 증가
            enemyMoveSpeed += speedUpValue;

            // 콜라이더 크기 증가
            circleCollider.radius += colliderUpValue;
        }
    }
}
