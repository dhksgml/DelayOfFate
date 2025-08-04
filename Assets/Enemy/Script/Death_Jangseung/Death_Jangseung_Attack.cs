using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Jangseung_Attack : EnemyAttack
{
    [Header("죽음장승")]
    [SerializeField] Death_Jangseung death_Jangseung;
    [HideInInspector] PlayerController player;
    [SerializeField] GameObject attackRange;
    public GameObject attackSprite;
    [HideInInspector] public bool isAttack = false;
    
        
    // 장승이 공격 범위를 보여주고 몇 초 뒤에 공격할지 정함
    [SerializeField] float jangseungAttackDelay;
    float jangseungAttackTime;

    void Awake()
    {
        //플레이어를 찾아서 저장해준 후
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (death_Jangseung.isSpawnReaper) 
        { 
            attackRange.SetActive(false);
            enemyAttackCollider.enabled = false;
            return; 
        }

        // 공격 중엔 시간이 안오름
        if (!isAttack) time += Time.deltaTime;

        // 공격 준비가 다 되었음면 공격
        if (time >= enemyAttackSpeed && death_Jangseung.isAttackReady)
        {
            Debug.Log("사거리 표시");
            isAttack = true;

            // 위치를 공격 범위 쪽으로 옳겨줌
            transform.position = death_Jangseung.attackTargetTrs;

            // 사거리 표시
            attackRange.SetActive(true);

            // 범위가 표시되고 지정한 초 뒤에 공격
            jangseungAttackTime += Time.deltaTime;


            if(jangseungAttackTime >= jangseungAttackDelay)
            {
                Debug.Log("공격");
                attackSprite.SetActive(true);

                // 사거리 비활성화
                attackRange.SetActive(false);

                // 콜라이더를 활성화 해줌
                enemyAttackCollider.enabled = true;

                // 초기화
                jangseungAttackTime = 0;
                isAttack = false;
                death_Jangseung.isAttackReady = false;
                time = 0;

                // 콜라이더 비활성화
                Invoke("Dealy", 0.1f);
            }
        }
    }


    // 플레이어 공격
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 공격
        if (collision.gameObject.CompareTag("Player") && !isAttack)
        {
            // 공격
            player.DamagedHP(enemyDamage);
        }
    }

    void Dealy()
    {
        attackSprite.SetActive(false);
        enemyAttackCollider.enabled = false;
    }
}
