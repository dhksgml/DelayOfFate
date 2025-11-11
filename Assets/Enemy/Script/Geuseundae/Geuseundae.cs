using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geuseundae : Enemy
{
    [Header("그슨대")]
    [SerializeField] bool isStop;
    [HideInInspector] public bool isAttack;
    [SerializeField] bool isNoDamage;
    [SerializeField] Material notSeeMaterial;
    [SerializeField] Material seeMaterial;  
    PlayerController player;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();

        GeuseundaeCloaking();

        // 처음에 랜덤한 방향 설정
        ChooseNewDirection();

        // 주기적으로 방향 전환
        StartCoroutine(ChangeDirectionRoutine());
    }


    void Update()
    {
        HpBarUpdate();

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
    }

    public override void EnemyMove()
    {
        enemyTargetDir = (player.transform.position - transform.position).normalized;
        EnemyNormalTurn2();

        // 공격 범위 내에 들어오면
        if (isAttack)
        {
            // 스프라이트 때문에 이걸 사용해줌


            return;
        }

        //추적중이 아니면
        else
        {
            // 에니메이션
            anim.SetBool("isMove", true);


            //에니메이션, 추적 false로 바꾸어줌
            //anim.SetBool("isTrace", false);

            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                // 무적이 아니면
                if (!isNoDamage)
                {
                    // 타입이 일치하면 즉사
                    if (!attack.CheckWeaknessPassive() && attack.attackType.ToString() == enemyWeakness.ToString())
                    {
                        attack.CheckWeakness();
                        Enemy_Weakness_Hit(attack.damage, attack.attackType.ToString(), enemyHp);
                        enemyHp = 0f;
                    }
                    else
                    {
                        enemyHp -= attack.damage;
                    }

                    EnemyHit(attack.damage);
                }
            }

            // 빛에 충돌시 은신 풀림
            if (collision.gameObject.CompareTag("Light"))
            {
                GeuseundaeLightHit();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            // 빛에서 벗어날시 은신
            if (collision.gameObject.CompareTag("Light"))
            {
                GeuseundaeCloaking();
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
                WallKnuckBack();
            }
        }
    }

    // 그슨대 은신해제
    public void GeuseundaeLightHit()
    {
        // 클로킹 중이 아님
        iscloaking = false;

        isNoDamage = true;

        // 이동속도 반영
        enemyMoveSpeed = enemyMoveSpeed * cloakingSpeed;
        enemyRunSpeed = enemyRunSpeed * cloakingSpeed;

        // 은신
        Color c = sp.color;
        c.a = 0.0f;
        sp.color = c;

        // 쉐이더 변경
        sp.material.shader = notSeeMaterial.shader;

    }

    // 그슨대 은신
    public void GeuseundaeCloaking()
    {
        // 이동속도, 공격력 1배
        cloakingSpeed = 1.0f;
        cloakingDamage = 1.0f;

        // 클로킹 중
        iscloaking = true;

        isNoDamage = false;

        // 은신 해제
        Color c = sp.color;
        c.a = 1.0f;
        sp.color = c;

        // 쉐이더 변경
        sp.material.shader = seeMaterial.shader;

        // 체력바를 비활성화 해줌
        enemyHpBar.hpObj.SetActive(false);
    }

}
