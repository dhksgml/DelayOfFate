using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static EnemyTrace;
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
    [SerializeField] Sprite hideSprite;
    [SerializeField] Sprite OutSprite;


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
        HpBarUpdate();

        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }

        // 잠복
        if (isIn)
        {
            sp.sprite = hideSprite;
        }
        // 돌출
        else if (isOut)
        {
            sp.sprite = OutSprite;
        }


        //잠복시 회복 시간
        healTime += Time.deltaTime;


        //잠복과 돌출시 하는 행동
        LandSharkStat();

        Debug.Log(isStop);
        EnemyMove();
        //if(!isStop) { EnemyMove(); }
    }


    bool isRush;

    void PlayerTrsFind()
    {
        enemyTargetDir = (player.transform.position - transform.position).normalized;
    }

    public override void EnemyMove()
    {
        

        if (isRush)
        {
            // 에니메이션
            EnemyTraceTurn();

            // 이동
            transform.Translate(enemyTargetDir * landSharkAttackSpeed * Time.deltaTime);
        }

        else if (isStop)
        {
            return;
        }

        //최대 채력이 아니면 도망
        else if(isEnemyRun)
        {
            enemyTargetDir = (enemyTrace.targetPos - transform.position).normalized;
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
            EnemyTraceTurn();
        }

        else
        {
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
            // 스프라이트 회전
            EnemyNormalTurn();
        }
    }

    //잠복으로 변환 하는 메서드
    public void IsHide()
    {
        isOut = false;
        isIn = true;

        // 에니메이션 실행
        anim.SetBool("isHide", true);

        // 이동
        isStop = false;
    }
    //돌출로 변환 하는 메서드
    public void isHideOut()
    {
        isIn = false;
        isOut = true;

        // 에니메이션 실행
        anim.SetBool("isHide", false);

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
        // 에니메이션 실행 11.03
        anim.SetBool("isAttackReady", true);
        PlayerTrsFind();

        landSharkAttack.enemyDamage = landSharkAttack.landSharkJumpAttackDamage;

        // 도약 전 0.5초 대기 11.03
        yield return new WaitForSeconds(0.5f);

        // 에니메이션 실행 11.03
        anim.SetBool("isAttack", true);

        // 한번 히트시에만 켜줌
        if (!landSharkAttack.isRushHit) { landSharkAttack.enemyAttackCollider.enabled = true; }
        isRush = true;

        //0.7초. 이건 행동 보고 수정해줘야 할 듯
        yield return new WaitForSeconds(1f);

        isRush = false;

        //공격 트리거 초기화
        isAttackReady = false;
        enemyTrace.landSharkAttackTime = 0;

        //콜라이더 비활성화
        landSharkAttack.enemyAttackCollider.enabled = false;
        
        //비활성화 된 simulated를 활성화 시켜준다.
        rigid.simulated = true;

        //데미지를 원래 데미지로 돌려놔줌
        landSharkAttack.enemyDamage = landSharkAttack.currentDamage;

        //잠복이 끝나고 돌출로 바꿔줌
        isHideOut();

        anim.SetBool("isAttack", false);
        anim.SetBool("isAttackReady", false);

        yield return new WaitForSeconds(2f);

        // 트리거 초기화
        landSharkAttack.isRushHit = false;

    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                WallCollOrigin();
            }
            // 빛에 충돌시 은신 풀림
            if (collision.gameObject.CompareTag("Light"))
            {
                EnemyCloaking();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // 플레이어 공격시 체크
                if (!attack.CheckWeaknessPassive() && attack.attackType.ToString() == enemyWeakness.ToString())
                {
                    // 약점공격
                    Enemy_Weakness_Hit(attack.damage, attack.attackType.ToString(), enemyHp);
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                EnemyHit(attack.damage);
            }

            if (collision.gameObject.CompareTag("Wall"))
            {
                WallNotCross();
            }

            // 빛에 충돌시 은신 풀림
            if (collision.gameObject.CompareTag("Light"))
            {
                EnemyLightHit();
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
