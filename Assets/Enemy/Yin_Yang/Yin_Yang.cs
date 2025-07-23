using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

//각각 음과 양
public enum Yin_Yang_Type
{
    Yin,
    Yang
}

public class Yin_Yang : Enemy
{
    [Header("Yin_Yang")]
    //배회하는 적, 이방식으로 할려면 콜라이더를 두개 넣어줘야 한다.
    //추후에 피격 콜라이더를 나누는게 좋을지도
    [SerializeField] public Yin_Yang_Type type;
    [SerializeField] Yin_YangTrace yin_YangTrace;
    [SerializeField] GameObject summonReaper;
    public bool isFind;
    bool isSpawn = false;
    [SerializeField] GameObject yinObj;

    PlayerController player; //플레이어

    // 바로 합체되는걸 막아주기 위함
    float delay;
    float fusionTime = 5.0f;

    // static으로 두 오브젝트가 공유하는 충돌 여부
    static bool hasFusion = false; 

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();

        if (type == Yin_Yang_Type.Yang)
        {
            // 처음에 랜덤한 방향 설정
            ChooseNewDirection();

            // 주기적으로 방향 전환
            StartCoroutine(ChangeDirectionRoutine());
        }
        Debug.Log(gameObject);
    }

    void Update()
    {
        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
        }

        if (!isSpawn)
        {
            if (type == Yin_Yang_Type.Yang)
            {
                isSpawn = true;
                // 음 0,0,0에 소환
                GameObject test = Instantiate(yinObj, new Vector3(0, 0, 0), Quaternion.identity);
            }
        }

        delay += Time.deltaTime;

        EnemyMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && !collision.gameObject.CompareTag("Enemy"))
        {
            Attack_sc attack = collision.GetComponent<Attack_sc>();

            if (collision.gameObject.CompareTag("Attack") && !isEnemyHit && attack != null)
            {
                Debug.Log(2);
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
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            //Yin_Yang 스크립트를 가져온 후
            Yin_Yang yinYang = collision.gameObject.GetComponent<Yin_Yang>();

            if(yinYang != null)
            {
                if(delay >= fusionTime)
                {
                    if (!hasFusion)
                    {
                        SummonReaper();
                        hasFusion = true;
                    }
                    Destroy(transform.parent.gameObject);
                }
            }
        }

        //충돌시 방향전환
        ChooseNewDirection();
    }

    void OnDestroy()
    {
        ResetCollision();
    }

    public override void EnemyMove()
    {
        if(isFind)
        {
            moveDirection = (yin_YangTrace.target - transform.position).normalized;
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
        else if(type == Yin_Yang_Type.Yang)
        {
            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }


    //사신 소환 메서드
    void SummonReaper()
    {
        Debug.Log("음양합체");
        Instantiate(summonReaper, transform.position, Quaternion.identity);
    }

    //static 변수 초기화 메서드
    public static void ResetCollision()
    {
        hasFusion = false;
    }
}
