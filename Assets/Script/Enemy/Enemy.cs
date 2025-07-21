using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using static Enemy_data;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
//스프라이트 들어오면 회전이나 에니메이션 처리도 해줘야 함

//적이 약점 임시로 5개로 지정하였다.
//검, 방망이, 부적, 스크롤, 호리병
public enum EnemyWeakness 
{
    Sword,
    Bat,
    Paper,
    Scroll,
    Bottle,
    None
}

// 적의 등급 노말, 중간보스, 보스로 구성
public enum EnemyMobType
{
    Normal,
    MiddleBoss,
    Boss
}

public abstract class Enemy     : MonoBehaviour
{
    [Header("적 데이터")]
    public Enemy_data enemyData;

    //적 스크립트에 콜라이더 두 종류 넣어줘야 할 것 같음, 피격과 충돌을 위함
    [Header("Enemy Stat")]
    public string        enemyName; //적의 이름
    public float         enemyHp;  //적의 체력
    public float         enemyMoveSpeed; //적의 이동속도
    public float         enemyRunSpeed; //도망치는 적의 이동속도
    
    [Space(20f)]
    //적의 가격 최소치와 최대치
    public int           enemyPriceMin;
    public int           enemyPriceMax;
    public int           enemyPrice; //적의 가격

    [Space(20f)]
    //적의 무게 최소치와 최대치
    public int           enemyHeightMin;
    public int           enemyHeightMax;
    public int           enemyHeight;

    [Space(20f)]
    // 보스인지 확인하기 위함
    public EnemyMobType enemyMobType;

    [Space(20f)]
    [Range(0f, 10f)]
    public float         enemyHitTime; //적이 피격당했을때 무적 시간
    [Space(20f)]
    public EnemyWeakness enemyWeakness; //적의 약점
    [Space(20f)]
    public Classification enemyType; //적의 타입

    [Header("Enemy Move Point")]
    public int           enemyCurrentMove; //적이 현재 이동한 횟수
    public Transform[]   enemyMovePoint; //적이 지정된 장소를 배회하게 만들어줌

    [Header("Trigger")]
    public bool          isTrace; //플레이를 추적 중인지 확인하는 bool
    public bool          isEnemyRun; //적의 패턴중 도망가는 패턴
    public bool          isEnemyAttack; //적이 공격을 하냐 확인하는 bool
    public bool          isEnemyHit; //피격했나 확인하는 bool
    public bool          isEnemyChase; //보이지 않아도 적이 쫒아가는지 확인하는 bool
    public bool          isArrive; //웨이포인트를 지닌 적이 한 바퀴를 다 돌았을때 확인하는 bool

    //기본적으로 적이 죽었을떄와 적이 피격당했을때 움직이지 않게 설정해놨습니다.

    [Header("Reference")]
    
    public EnemyTrace           enemyTrace;
    public EnemyAttack          enemyAttack;    
    public SpriteRenderer       sp;
    public Animator             anim;
    public GameObject           enemyCorpse; //적 시체
    public GameObject           enemySelf;
    public Collider2D           enemyColl;

    [HideInInspector] public Vector3       enemyTargetDir; //적의 타겟 방향

    [HideInInspector] public bool                 isDie = false;
    [HideInInspector] public Rigidbody2D          rigid;
    [HideInInspector] public Vector3              enemyMoveDir; //적의 원래 이동 방향
    public Vector3              moveDirection;

    //시작시 초기화 해주는 함수
    public void EnemyInt()
    {
        enemyName = enemyData.Name;
        enemyHeight = enemyData.Weight;
        //이거 수정해주었음 enemyData부분
        enemyWeakness = enemyData.weakness;
        enemyType = enemyData.classification;

        // 노말
        if (enemyMobType == EnemyMobType.Normal)
        {
            //체력도 랜덤 값에서 - 랜덤값 + 랜덤값에서 나온 값으로 할당해줌
            enemyHp = Random.Range(enemyData.Hp - enemyData.HpDeviation,
                                   enemyData.Hp + enemyData.HpDeviation + 1);
            //일단 랜덤 값 설정에서 기본 코인값 - 랜덤값 ~~ 코인값 + 랜덤값이렇게 해주었음
            enemyPrice = Random.Range(enemyData.Coin - enemyData.CoinDeviation,
                                      enemyData.Coin + enemyData.CoinDeviation + 1);
        }
        // 중간 보스
        else if (enemyMobType == EnemyMobType.MiddleBoss)
        {
            // 중간 보스는 2배
            //체력도 랜덤 값에서 - 랜덤값 + 랜덤값에서 나온 값으로 할당해줌
            enemyHp = Random.Range((enemyData.Hp - enemyData.HpDeviation) * 2,
                                   (enemyData.Hp + enemyData.HpDeviation * 2) + 1);
            //일단 랜덤 값 설정에서 기본 코인값 - 랜덤값 ~~ 코인값 + 랜덤값이렇게 해주었음
            enemyPrice = Random.Range(enemyData.Coin - enemyData.CoinDeviation * 2,
                                      (enemyData.Coin + enemyData.CoinDeviation * 2 ) + 1);

            // 크기는 두배로
            transform.localScale = new Vector3(2, 2, 2);
        }

        // 보스
        else if (enemyMobType == EnemyMobType.Boss)
        {

        }


    }

    //이젠 안씀
    public void EnemyPrice()
    {
        //적의 가격을 정해줌. int형이기에 최대값에 +1을 더해주었음
        enemyPrice = Random.Range(enemyPriceMin, enemyPriceMax + 1);
    }
    //이젠 안씀
    public void EnemyHeight()
    {
        enemyHeight = Random.Range(enemyHeightMin, enemyHeightMax + 1);
    }

    public abstract void EnemyMove();

    #region 목표 이동
    //적의 이동을 하는 메서드
    public void EnemyMoveTarget()
    {
        if (enemyMovePoint.Length > 0)
        {
            enemyMoveDir = (enemyMovePoint[enemyCurrentMove].position - transform.position).normalized;

            //만약 적과 현재 이동 포인트의 거리가 0.02f * enemyMoveSpeed보다 작으면
            if (Vector3.Distance(transform.position, enemyMovePoint[enemyCurrentMove].position) < 0.02f * enemyMoveSpeed)
            {
                //이동 포인트 변경
                EnemyNextMove();
            }
        }
    }

    //적의 다음 이동 방향을 정해주는 메서드
    public void EnemyNextMove()
    {
        //적의 이동 횟수가 적의 이동 포인트 길이보다 작으면
        if (enemyCurrentMove < enemyMovePoint.Length - 1 )
        {
            //이동 횟수를 ++ 해준 후
            enemyCurrentMove++;
            isArrive = false;
        }
        //만약 마지막 이동포인트면
        else
        {
            //현재 이동 횟수를 0으로 해준 후
            enemyCurrentMove = 0;
            isArrive = true;
        }
        //이동 포인트의 위치와 자기 자신의 거리를 빼준 후 정규화를 하고 Vector3 변수에 저장해준다.
        enemyMoveDir = (enemyMovePoint[enemyCurrentMove].position - transform.position).normalized;
    }
    #endregion

    #region 무작위 이동
    //360도기준으로 이동
    public void ChooseNewDirection()
    {
        // 0~360도 중 랜덤한 각도로 이동 방향 설정
        float angle = Random.Range(0f, 360f);
        float radian = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }

    public IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f)); // 주기적으로 방향 전환 랜덤으로 해주었음
            ChooseNewDirection();
        }
    }
    #endregion

    #region 피격
    //적이 플레이어에게 피격당했을떄. 
    public void EnemyHit()
    {
        isEnemyHit = true;

        //피격시 못움직이게 모두 동결해줌
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;

        //스프라이트 추가시 사용 0603
        //Color color = sp.color;
        //color.a = 0.5f;
        //sp.color = color;

        Invoke("EnemyHitRegen", enemyHitTime);
    }

    //적이 피격당한후 색이 돌아올때
    public void EnemyHitRegen()
    {
        //스프라이트 추가시 사용 0603
        //Color color = sp.color;
        //color.a = 1f;
        //sp.color = color;

        isEnemyHit = false;
        //동결해둔걸 다시 풀어줌
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    #endregion

    #region 사망처리
    //적이 죽을떄 실행됨
    public IEnumerator EnemyDie()
    {
        Color color = sp.color;

        //먼저 추적 범위와 공격 범위를 지워줌.
        Destroy(enemyTrace);
        Destroy(enemyAttack);


        //투명도 값을 1.0에서 0.01씩 뺴주면서 천천히 투명하게 해줌
        for (float i = 1.0f; i >= 0.0f; i -= 0.01f )
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(transform.parent.gameObject);
        //EnemyCorpseSummon();
    }

    //적이 죽을떄 시체를 소환하는 메서드
    void EnemyCorpseSummon()
    {
        //생성한 적 시체 게임오브젝트를 가져와 저장해줌
        GameObject corpse = Instantiate(this.enemyCorpse, transform.position, transform.rotation);
        //그 후 EnemyCorpse 컴포넌트를 가져와줌
        EnemyCorpse enemyCorpse = corpse.GetComponent<EnemyCorpse>();
        //이름 정보와 가격을 전달해줌
        enemyCorpse.corpseName = $"{enemyName}의 영혼";
        enemyCorpse.corpseGold = enemyPrice;
        enemyCorpse.corpseHeight = enemyHeight;

        //마지막으로 자기 자신을 지워줌
        //부모 오브젝트도 통으로 지워줌
        Destroy(transform.parent.gameObject);
    }

    #endregion

    #region 회전 처리

    //적의 스프라이트 회전을 위함
    public void EnemyNormalTurn()
    {
        //적이 돌아다닐때
        if (moveDirection.x < 0) { sp.flipX = true; }
        else if (moveDirection.x > 0) { sp.flipX = false; }

    }

    //스프라이트 때문에 두개 만들어줌
    public void EnemyNormalTurn2()
    {
        //적이 돌아다닐때
        if (moveDirection.x < 0) { sp.flipX = false; }
        else if (moveDirection.x > 0) { sp.flipX = true; }

    }

    //적이 쫒을때
    public void EnemyTraceTurn()
    {
        if (enemyTargetDir.x < 0) { sp.flipX = true; }
        else if (enemyTargetDir.x > 0) { sp.flipX = false; }
    }

    //적이 훔치고 도망갈때
    public void EnemyTraceTurn2()
    {
        if (enemyTargetDir.x < 0) { sp.flipX = false; }
        else if (enemyTargetDir.x > 0) { sp.flipX = true; }
    }
    #endregion

    #region 벽 충돌 처리

    //  벽을 못넘게 해주는 메서드
    public void WallNotCross()
    {
        enemyColl.isTrigger = false;
    }

    // 다시 원래대로 해주는 메서드
    public void WallCollOrigin()
    {
        enemyColl.isTrigger = true;
    }

    #endregion



    //수정용
    //void EnemyCorpseSummon()
    //{
    //    //생성한 적 시체 게임오브젝트를 가져와 저장해줌
    //    GameObject corpse = Instantiate(this.enemyCorpse, transform.position, transform.rotation);
    //    //그 후 EnemyCorpse 컴포넌트를 가져와줌
    //    Item enemyCorpse = corpse.GetComponent<Item>();//itemData
    //    //이름 정보와 가격을 전달해줌
    //    enemyCorpse.itemName = $"{enemyName}의 영혼";
    //    enemyCorpse.Coin = enemyPrice;
    //    enemyCorpse.Weight = enemyHeight;
    //}
}
