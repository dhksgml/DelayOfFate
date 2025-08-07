using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
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
    public Boon_yeol_gwi entityObj; // 본체
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

            Boon_yeol_gwi copyBoon = copyEnemy.GetComponentInChildren<Boon_yeol_gwi>();
            
            if (copyBoon == null) { Debug.LogWarning("자식없음"); }
            copyBoon.entityObj = gameObject.GetComponentInChildren<Boon_yeol_gwi>();

            //리스트에 추가해줌
            copyObjList.Add(copyEnemy);
            currentIndex++; //1마리 증가했으므로 ++
        }
    }
    float listTime = 0;
    void Update()
    {
        // 리스트 관련
        if (!isDie) { listTime += Time.deltaTime; }

        if (listTime >= 0.5f)
        {
            listTime = 0;
            RemoveList();
        }
        // 분열체 관련 삭제
        if (enemyHp <= 0 && type == Boon_yeol_gwi_Type.Copy)
        {
            entityObj.copyObjList.Remove(gameObject);
            entityObj.RemoveList();
        }

        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;

            foreach (var boon_yeol_che in copyObjList)
            {
                Destroy(boon_yeol_che);
            }

            // 본체 사망처리
            if (type == Boon_yeol_gwi_Type.Entity)
            {
                StartCoroutine(EnemyDie());
            }
            // 분열체 사망 처리
            else if (type == Boon_yeol_gwi_Type.Copy)
            {
                // 그냥 소멸처리
                StartCoroutine(CopyDie());
            }
            
        }

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
            if (copyObjList.Count <= maxIndex)
            {
                //두마리 소환
                GameObject copyEnemy = Instantiate(copyObj, transform.position, Quaternion.identity);

                Boon_yeol_gwi copyBoon = copyEnemy.GetComponentInChildren<Boon_yeol_gwi>();

                if (copyBoon == null) { Debug.LogWarning("자식없음"); }
                copyBoon.entityObj = gameObject.GetComponentInChildren<Boon_yeol_gwi>();

                copyObjList.Add(copyEnemy);



                currentIndex++;

                if (copyObjList.Count <= maxIndex)
                {
                    GameObject copyEnemy2 = Instantiate(copyObj, transform.position, Quaternion.identity);

                    Boon_yeol_gwi copyBoon2 = copyEnemy2.GetComponentInChildren<Boon_yeol_gwi>();

                    if (copyBoon2 == null) { Debug.LogWarning("자식없음"); }
                    copyBoon2.entityObj = gameObject.GetComponentInChildren<Boon_yeol_gwi>();

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
        if (type == Boon_yeol_gwi_Type.Copy)
        {
            // 본체 발견
            if (isEntityFind)
            {
                Vector3 target = (targetTrs - transform.position).normalized;
                transform.Translate(target * enemyMoveSpeed * Time.deltaTime);
            }

            // 아이템 가치 소멸 후 복귀
            else if (isItemEat)
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
                    enemyHp = 0f;
                }
                else
                {
                    enemyHp -= attack.damage;
                }

                EnemyHit(attack.damage);
                Invoke("EnemyHitRegen", enemyHitTime);
            }

            //item을 가져와준 후
            ItemObject item = collision.gameObject.GetComponent<ItemObject>();

            if (collision.gameObject.CompareTag("Item") && item.itemData.Coin != 0 && !isItemEat)
            {
                //가치를 올려준 후
                enemyPrice += item.itemData.Coin;

                // 아이템을 제거해줌
                Destroy(item.gameObject);

                isItemEat = true;
            }


            if (type == Boon_yeol_gwi_Type.Copy)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    Boon_yeol_gwi entity = collision.GetComponent<Boon_yeol_gwi>();

                    //충돌한게 본체일때
                    if (entity == entityObj)
                    {
                        if (entity.type == Boon_yeol_gwi_Type.Entity && isItemEat && isItemEat)
                        {
                            //가치를 올려줌
                            entity.enemyPrice += enemyPrice;

                            entity.copyObjList.Remove(gameObject);

                            RemoveList();

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
    // 분열체 전용 사망처리
    IEnumerator CopyDie()
    {
        Debug.Log("테스트");

        // 사망시 이펙트 
        Instantiate(enemyDeathEffect, transform.position, Quaternion.identity);

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
       
        Destroy(transform.parent.gameObject);
    }

    public void RemoveList()
    {
        //리스트 정리
        copyObjList.RemoveAll(obj => obj == null);

        // 본체가 분열체가 남아있는지 확인하기 위함
        if (copyObjList.Count <= 0 && type == Boon_yeol_gwi_Type.Entity)
        {
            StartCoroutine(EnemyDie());
        }
    }
}
