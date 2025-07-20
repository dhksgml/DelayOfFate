using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Color = UnityEngine.Color;

public class WomanGhost : Enemy
{

    //사라질때 위치
    Vector3 invisibleTrans;
    //사라지면 멈추기 위해
    [SerializeField] bool isStop;
    //플레이어가 보고있는지 확인
    [SerializeField] bool isPlayerSee;
    bool isinvisible;
    bool isAction = false;
    public bool isAttack;
    bool isWomanTrace = false;
    

    [SerializeField] float seeTime;
    [SerializeField] float dontSeeTime;

    PlayerController player; //플레이어
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyInt();
    }

    void Start()
    {
        // 처음에 랜덤한 방향 설정
        ChooseNewDirection();

        // 주기적으로 방향 전환
        StartCoroutine(ChangeDirectionRoutine());
    }
    void Update()
    {
        if (isTrace) { isWomanTrace = true; }
        
        if (!isPlayerSee && isinvisible) { dontSeeTime += Time.deltaTime; }
       
        //안본지 5초가 지나면 다시 나와줌
        if (dontSeeTime >= 5f && !isPlayerSee && !isAttack && !isAction)
        {
            StartCoroutine(PlayerDontSee());
            dontSeeTime = 0f;
        }

        //적의 체력이 0이하일시.
        if (enemyHp <= 0 && !isDie)
        {
            isDie = true;
            StartCoroutine(EnemyDie());
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
                Debug.Log(1);
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

            //플레이어의 시야가 닿을시
            if (collision.gameObject.CompareTag("Sight"))
            {
                isPlayerSee = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //만약  플레이어가 이 몬스터를 지켜보고 있다면
        if (collision.gameObject.CompareTag("Sight") && isPlayerSee)
        {
            seeTime += Time.deltaTime;

            if (seeTime >= 3f && isPlayerSee && !isStop && !isAttack && !isAction)
            {
                StartCoroutine(PlayerSee());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Sight"))
        {
            isPlayerSee = false;
        }
    }

    public override void EnemyMove()
    {
        //추적하는 타겟의 위치 - 자신의 위치를 구한 후 정규화를 해준다
        enemyTargetDir = (player.transform.position - transform.position).normalized;

        //플레이어가 지켜볼때
        if (isPlayerSee && !isDie && !isEnemyHit && !isStop)
        {
            EnemyTraceTurn();

            anim.SetBool("isTrace", false);

            // 한번 보지않으면 끝까지 쫒아옴
            isWomanTrace = false;

            isTrace = false;

            //반대 방향으로 도망가줌
            rigid.MovePosition(transform.position + -enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        //추적중일때
        else if (isWomanTrace && !isDie && !isEnemyHit && !isStop && !isPlayerSee)
        {
            EnemyTraceTurn2();
   
            //에니메이션, 추적 true 바꾸어줌
            anim.SetBool("isTrace", true);

            // 목표를 향해 이동
            transform.Translate(enemyTargetDir * enemyMoveSpeed * 3 * Time.deltaTime);

        }

        //추적중이 아니면
        else if (!isTrace && !isDie && !isEnemyHit && !isStop && !isWomanTrace)
        {
            //스프라이트 때문에 이걸 사용해줌
            EnemyNormalTurn2();

            //에니메이션, 추적 false로 바꾸어줌
            anim.SetBool("isTrace", false);

            // 현재 방향으로 이동
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
    }

    IEnumerator PlayerSee()
    {
        isStop = true;
        Color color = sp.color;

        //현재 위치값을 저장해준 후
        invisibleTrans = transform.position;

        for (float i = 1.0f; i >= 0.0f; i -= 0.01f)
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }

        seeTime = 0f;
        isinvisible = true;

    }

    //5초?7초 뒤에는 코루틴 실행전에 Time.delta을 더해주는걸로 
    IEnumerator PlayerDontSee()
    {

        Color color = sp.color;

        for (float i = 0.0f; i <= 1.0f; i += 0.01f)
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }

        isinvisible = false;
        isStop = false;

    }

    public IEnumerator PlayerPossession()
    {
        anim.SetBool("isAttack", true);
        Color color = sp.color;
        //각각 -3 ~ -5, -8 ~ -12, -10 ~ -11로 해주었다
        int randomHpDamage = Random.Range(3, 6);
        int randomMpDamage = Random.Range(8, 13);
        int randomSpDamage = Random.Range(10, 12);

        //플레이어가 이동 못하게 해줌. 이부분은 스크립트 가져오는걸로
        player.isFreeze = true;
        isStop = true;
        isinvisible = true;
        isAttack = true;

        for (float i = 1.0f; i >= 0.0f; i -= 0.01f)
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }

        while (player.isFreeze)
        {

            //채력, 정신력, 기력이 1초마다 감소됨
            player.DamagedHP(randomHpDamage);
            player.DamagedMP(randomMpDamage);
            player.DamagedSP(randomSpDamage);

            yield return new WaitForSeconds(1f);
        }

        for (float i = 0.0f; i <= 1.0f; i += 0.01f)
        {
            color.a = i;
            sp.color = color;
            //딜레이를 위해 코루틴을 사용해줌
            yield return new WaitForSeconds(0.01f);
        }

        player.isFreeze = false;
        isAttack = false;
        isinvisible = false;
        isStop = false;
        //10번을 다채우면 다시 나타나게 해줌
        anim.SetBool("isAttack", false);
    }
}
