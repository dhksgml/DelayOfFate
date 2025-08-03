using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Somyeon_gwi : Enemy
{
    [Header("소면귀")]
    PlayerController player;
    public GameObject[] randomItem; //내뱉을 아이템
    [SerializeField] int throwItemCount; //아이템을 내뱉는 횟수
    [SerializeField] float eatDelay; //먹을떄 딜레이
    public bool isHit; //피격시
    public bool isFindItem; //아이템을 찾았을 시
    public Vector3 findItemVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        EnemyInt();
    }

    float eatTime;

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
            eatTime += Time.deltaTime;
            EnemyMove();
        }
    }

    public override void EnemyMove()
    {
        //피격시
        if (isHit)
        {
            //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            //EnemyTraceTurn2();

            //에니메이션, 추적 true 바꾸어줌
            //anim.SetBool("isTrace", true);

            rigid.MovePosition(transform.position + enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }
        //아이템을 발견하고, 횟수가 남아있으며 먹는 시간이 다 되면
        else if (isFindItem && throwItemCount > 0 && eatTime >= eatDelay)
        {
            enemyTargetDir = (findItemVec - transform.position).normalized;
            rigid.MovePosition(transform.position + enemyTargetDir * enemyMoveSpeed * Time.deltaTime);
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

            //최대 3번까지
            if (collision.gameObject.CompareTag("Item") && throwItemCount > 0 &&
                eatTime >= eatDelay)
            {
                //위치를 저장함
                Vector3 itemPos = collision.transform.position;

                //닿은 아이템을 소멸시킨 후
                Destroy(collision.gameObject);

                //랜덤한 위치에 스폰되게 해줌
                float randomPosX = itemPos.x + Random.Range(-4f, 4f);
                float randomPosY = itemPos.y + Random.Range(-4f, 4f);

                //넣어둔 아이템중 랜덤으로 스폰되게 해주었음
                Instantiate(randomItem[Random.Range(0, randomItem.Length)],
                    new Vector3(randomPosX, randomPosY, itemPos.z), 
                    Quaternion.identity);

                //트리거 초기화 해줌
                isFindItem = false;
                //횟수를 빼준다.
                throwItemCount--;
                eatTime = 0;
            }

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

                isHit = true;

                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }
        }
    }
}
