using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanGhostRemake : Enemy
{

    Vector3 invisibleTrans;
    [SerializeField] bool isStop;
    //[SerializeField] bool isPlayerSee;

    //bool isAction = false;
    public bool isAttack;
    bool isWomanTrace = false;
    bool isCloaking = false;


    [SerializeField] float seeTime;
    [SerializeField] float dontSeeTime;

    [SerializeField] private float maxStunTime = 3f; 
    [SerializeField] private float minStunTime = 1f;
    [SerializeField] int attackDamage;
   

    PlayerController player;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        // 초기화
        EnemyInt();

        // 이동 방향 설정
        ChooseNewDirection();
        StartCoroutine(ChangeDirectionRoutine());
    }
    void Update()
    {
        HpBarUpdate();

        if (isTrace) { isWomanTrace = true; }

        // 사망처리
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }

        if (isAttack && !isCloaking) 
        {
            isCloaking = true;
            StartCoroutine(Cloaking()); 
        }

        EnemyMove();
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

            // 빛에 충돌시 은신 풀림
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
            if (collision.gameObject.CompareTag("Sight"))
            {
                //isPlayerSee = false;
            }

            // 빛에서 벗어날시 은신
            if (collision.gameObject.CompareTag("Light"))
            {
                EnemyCloaking();
            }
        }
    }

    public override void EnemyMove()
    {
        // 추격 목표
        enemyTargetDir = (player.transform.position - transform.position).normalized;


        // 추적시
        if (!iscloaking)
        {
            EnemyTraceTurn2();

            anim.SetBool("isTrace", true);

            transform.Translate(enemyTargetDir * enemyMoveSpeed * 3 * Time.deltaTime);

        }

        // 통상
        else if (!isTrace && !isDie && !isEnemyHit && !isStop && !isWomanTrace)
        {
            EnemyNormalTurn2();

            anim.SetBool("isTrace", false);

            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }

    public void WomanGhostAttack()
    {
        // 체력 비율
        float hpRatio = enemyHp / enemyMaxHp;

        // hpRatio가 1일 때 maxStunTime, hpRatio가 0일 때 minStunTime
        float stunTime = Mathf.Lerp(minStunTime, maxStunTime, hpRatio);

        // 트리거 활성화 및 값 전달
        player.isFreeze = true;
        player.freezeTime = stunTime;
        isAttack = true;

        // 데미지
        player.DamagedHP(attackDamage * cloakingDamage);
    }

    IEnumerator Cloaking()
    {
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

        // 소멸
        Destroy(transform.parent.gameObject);
    }
}
