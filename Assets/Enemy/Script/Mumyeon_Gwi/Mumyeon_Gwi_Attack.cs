using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mumyeon_Gwi_Attack : EnemyAttack
{
    [Header("무면귀")]
    [SerializeField] Mumyeon_Gwi mumyeon_Gwi;
    PlayerController player;


    void Awake()
    {
        //플레이어를 찾아서 저장해준 후
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {


        transform.position = enemy.transform.position;
        //거리를 계산하고
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (mumyeon_Gwi.isTrace)
        {
            // 공격 시간을 체크
            time += Time.deltaTime;

            // 공격 콜라이더 회전
            rotationColl();

            //공격 시간이 됐고, 목표와의 거리가 1f 보다 작거나 같으면 공격
            if (time >= enemyAttackSpeed && distance <= 7f)
            {
                //에니메이션 활성화

                //공격 콜라이더 활성화 해줌
                enemyAttackCollider.enabled = true;

                // 시간을 초기화해줌
                time = 0;

                //0.5초후 공격 콜라이더 비활성화
                Invoke("AttackDelay", 0.5f);
            }
        }
    }

    public void AttackDelay()
    {
        enemyAttackCollider.enabled = false;
        //에니메이션
    }
}
