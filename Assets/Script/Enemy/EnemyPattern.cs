using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//적의 트리거 본다, 안본다, 플레이어 주변, 플레이어 움직임, 죽는다로 구성
public enum EnemyTrigger
{
    See,
    Not_See,
    Around_Player,
    Moving_Player,
    Die
}
//적의 행동 공격, 도망, 죽는다, 훔친다로 구성
public enum EnemyAction
{
    Attack,
    Run,
    Die,
    Steal
}

public class EnemyPattern : MonoBehaviour
{
    //적의 트리거
    public EnemyTrigger enemyTrigger;
    //적의 행동
    public EnemyAction enemyAction;

    [Header("Reference")]
    [SerializeField]
    Enemy              enemy;

    //만약 보이지 않아도 플레이어를 쫒아가기 위함임
    public GameObject      player;

    void Awake()
    {
        //플레이어가 보이지 않아도 쫒아가는 몬스터를 위함임
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //보이지 않아도 && 쫒아가 돈을 훔침
        if (enemyTrigger == EnemyTrigger.Not_See && enemyAction == EnemyAction.Steal && !enemy.isEnemyChase)
        {
            enemy.isEnemyChase = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                if (enemyTrigger == EnemyTrigger.Not_See && enemyAction == EnemyAction.Steal)
                {
                    //enemy.isSteal = true;
                    Debug.Log("플레이어의 골드가 감소하였습니다");
                    //추후 이곳에서 충돌한 콜라이더에게서 플레이어를 가져오거나
                    //싱글톤으로 선언한 게임메니저에서 돈을 감소시키게 해주면 될 것 같음
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            //플레이어가 범위 안에 들어오면 쫒아가줌
            if (collision.gameObject.CompareTag("Player"))
            {
                //보고 && 도망감
                if (enemyTrigger == EnemyTrigger.See && enemyAction == EnemyAction.Run)
                {
                    enemy.isEnemyRun = true;
                }

                //보고 && 공격하러옴
                else if (enemyTrigger == EnemyTrigger.See && enemyAction == EnemyAction.Attack)
                {
                    enemy.isEnemyAttack = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            //플레이어가 범위 안에 들어오면 쫒아가줌
            if (collision.gameObject.CompareTag("Player"))
            {
                if (enemyTrigger == EnemyTrigger.See && enemyAction == EnemyAction.Run)
                {
                    Invoke("Delay", 1f);
                }
            }
        }
    }

    void Delay()
    {
        enemy.isEnemyRun = false;
    }

}
