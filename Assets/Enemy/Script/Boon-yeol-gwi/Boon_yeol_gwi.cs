using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boon_yeol_gwi_Type
{
    Entity,
    Copy
}

public class Boon_yeol_gwi : Enemy
{
    [Header("분열귀")]
    [SerializeField] Boon_yeol_gwi_Type type;
    [SerializeField] GameObject copyObj; //분열체 오브젝트
    [SerializeField] Vector3 entityTrs; //본체 위치
    [SerializeField] int explosionSelfvalue; //가치가 이거 이상이면 터짐

    [SerializeField] public bool isItemFind = false; //분열체가 아이템을 찾았을때
    [SerializeField] public bool isItemEat = false; //분열체가 아이템을 먹었을때
    [SerializeField] public bool isEntityFind = false; //분열체가 본체를 찾았을떄
    [SerializeField] bool isSpawn = false; //스폰하는 트리거

    [SerializeField] int currentIndex = 0; //현재 인덱스
    int maxIndex = 2; //최대 3마리로 지정

    public List<GameObject> copyObjList; //생성된 적을 보관할 리스트

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        EnemyInt();

        //분열체일떄
        if (type == Boon_yeol_gwi_Type.Copy)
        {
            //시작하면 위치를 지정해줌
            entityTrs = transform.position;
            // 처음에 랜덤한 방향 설정
            ChooseNewDirection();

            // 주기적으로 방향 전환
            StartCoroutine(ChangeDirectionRoutine());
        }
        //본채일떄
        else if (type == Boon_yeol_gwi_Type.Entity)
        {
            GameObject copyEnemy = Instantiate(copyObj, transform.position, Quaternion.identity);
            //리스트에 추가해줌
            copyObjList.Add(copyEnemy);
            currentIndex++; //1마리 증가했으므로 ++
        }
    }

    void Update()
    {
        //본체일때 일정 가격이 되면
        if (enemyPrice >= explosionSelfvalue && type == Boon_yeol_gwi_Type.Entity)
        {
            //일단 삭제 처리해줌
            Destroy(transform.parent.gameObject);

            //분열체도 삭제처리해줌
            foreach (GameObject obj in copyObjList)
            {
                Destroy(obj);
            }
        }

        //스폰 준비가 되면
        else if (type == Boon_yeol_gwi_Type.Entity && isSpawn)
        {
            if (currentIndex <= maxIndex)
            {
                //두마리 소환
                GameObject copyEnemy = Instantiate(copyObj, transform.position, Quaternion.identity);
                copyObjList.Add(copyEnemy);

                currentIndex++;

                if (currentIndex <= maxIndex)
                {
                    GameObject copyEnemy2 = Instantiate(copyObj, transform.position, Quaternion.identity);
                    copyObjList.Add(copyEnemy2);
                    currentIndex++;
                }

                isSpawn = false;
            }
        }

        EnemyMove();
    }

    [HideInInspector] public Vector3 itemTrs; //아이템 위치
    [HideInInspector] public Vector3 targetTrs; //타겟 위치

    public override void EnemyMove()
    {
        // 본체 발견
        if (isEntityFind)
        {
            Vector3 target = (targetTrs - transform.position).normalized;
            transform.Translate(target * enemyMoveSpeed * Time.deltaTime);
        }

        // 아이템 가치 소멸 후 복귀
        else if (isItemFind && isItemEat)
        {
            Vector3 entity = (entityTrs - transform.position).normalized;
            transform.Translate(entity * enemyMoveSpeed * Time.deltaTime);
        }

        // 아이템 발견
        else if (isItemFind)
        {
            Vector3 itemPos = (itemTrs - transform.position).normalized;
            transform.Translate(itemPos * enemyMoveSpeed * Time.deltaTime);
        }
        // 그 외
        else if (!isItemEat && !isItemFind && !isEntityFind)
        {
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            //피격 효과
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

            //item을 가져와준 후
            ItemObject item = collision.gameObject.GetComponent<ItemObject>();

            if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0 && !isItemEat)
            {
                //가치를 올려준 후
                enemyPrice += item.itemData.Coin;

                //가치를 제거해줌
                item.itemData.Coin = 0;

                isItemEat = true;
            }


            if (type == Boon_yeol_gwi_Type.Copy)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    if (entity != null)
                    {
                        //충돌한게 본체일때
                        if (entity.type == Boon_yeol_gwi_Type.Entity &&
                            isItemEat && isItemEat)
                        {
                            //가치를 올려줌
                            entity.enemyPrice += enemyPrice;

                            //자기 자신을 리스트에서 제거
                            if (entity.copyObjList.Contains(gameObject))
                            {
                                entity.copyObjList.Remove(gameObject);
                            }
                            //리스트 정리
                            for (int i = entity.copyObjList.Count - 1; i >= 0; i--)
                            {
                                if (entity.copyObjList[i] == null)
                                {
                                    entity.copyObjList.RemoveAt(i);
                                }
                            }

                            //그리고 스스로 사라짐
                            Destroy(transform.parent.gameObject);

                            //현재 인덱스값 뺴줌
                            entity.currentIndex--;

                            //그리고 스폰하는 트리거 활성화
                            entity.isSpawn = true;
                        }
                    }
                }
            }
        }
    }
}
