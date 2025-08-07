using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class LandShark : Enemy
{
    [Header("땅상어")]
    public bool isOut; //돌출
    public bool isIn; //잠복
    public bool isAttackReady; //공격 준비
    public bool isStop; //멈춤
    public float landSharkAttackSpeed;
    [SerializeField] LandSharkAttack landSharkAttack;


    PlayerController player; //플레이어

    float enemyOriginHP;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }


    void Start()
    {
        EnemyInt();

        enemyOriginHP = enemyHp;

        // 처음에 랜덤한 방향 설정
        ChooseNewDirection();

        // 주기적으로 방향 전환
        StartCoroutine(ChangeDirectionRoutine());
    }

    float healTime;

    void Update()
    {
        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }
        //잠복시 회복 시간
        healTime += Time.deltaTime;


        //잠복과 돌출시 하는 행동
        LandSharkStat();

        if(!isStop) { EnemyMove(); }
    }

    public override void EnemyMove()
    {
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;

        //돌출 시
        if (isOut && !isIn && isTrace)
        {
            rigid.MovePosition(transform.position + enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        //최대 채력이 아니면 도망
        else if(isEnemyRun)
        {
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        else
        {
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }

    //잠복으로 변환 하는 메서드
    public void IsHide()
    {
        isIn = true;
        isOut = false;
    }
    //돌출로 변환 하는 메서드
    public void isHideOut()
    {
        isIn = false;
        isOut = true;
    }

    void LandSharkStat()
    {
        if (isIn) //잠복시
        {
            healTime += Time.deltaTime;

            //적의 체력이 최대 체력이 아니라면
            if (enemyHp < enemyOriginHP) { isEnemyRun = true; }
            //최대체력이라면
            else if (enemyHp == enemyOriginHP)
            {
                isAttackReady = true;
            }

            if (healTime >= 1f)
            {
                enemyHp += 5;
                if (enemyHp >= enemyOriginHP) { enemyHp = enemyOriginHP; }

                healTime = 0f;
            }
        }

        else if (isOut) //돌출시
        {
            isAttackReady = false;
        }
    }

    public IEnumerator LandSharkJumpAttackMove()
    {
        float stopDistance = 0f;

        float distanceToTarget = Vector3.Distance(transform.position, enemyTrace.targetPos);
        enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;
        Vector3 targetPosition = enemyTrace.targetPos + (enemyTargetDir * stopDistance);
        
        //거리가 멈추는 거리보다 크면 근데 사실상 true라 초에 맞춰서 멈추게 해주었음
        if (distanceToTarget > stopDistance)
        {
            //데미지를 점프 어택 데미지만큼 할당시켜줌
            landSharkAttack.enemyDamage = landSharkAttack.landSharkJumpAttackDamage;
            //플레이어를 뚫고 가야 하기 때문에 잠시 꺼준 후
            rigid.simulated = false;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition - Vector3.right,
                landSharkAttackSpeed * Time.deltaTime
            );
            //0.7초. 이건 행동 보고 수정해줘야 할 듯
            yield return new WaitForSeconds(0.7f);
        }

        //공격 트리거 초기화
        isAttackReady = false;
        isStop = false;
        enemyTrace.landSharkAttackTime = 0;

        //콜라이더 비활성화
        landSharkAttack.enemyAttackCollider.enabled = false;
        
        //비활성화 된 simulated를 활성화 시켜준다.
        rigid.simulated = true;

        //데미지를 원래 데미지로 돌려놔줌
        landSharkAttack.enemyDamage = landSharkAttack.currentDamage;

        //잠복이 끝나고 돌출로 바꿔줌
        isHideOut();

        yield return null;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                WallNotCross();
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



}
