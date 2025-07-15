using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mumyeon_Gwi : Enemy
{
    [Header("무면귀")]
    PlayerController player;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EnemyInt(); // 메서드 안보임 Enemy 스크립트 확인 0603
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
        EnemyMove();
    }

    public override void EnemyMove()
    {
        // 추적 중이면
        if(isTrace)
        {
            // 플레이어의 위치와 자기 자신의 위치를 뺴준 후 정규화를 해줌
            enemyTargetDir = (player.transform.position - transform.position).normalized;

            // 그리고 추적을 해준다.
            transform.Translate(enemyTargetDir * enemyRunSpeed * Time.deltaTime);
        }

        else
        {
            transform.Translate(moveDirection * enemyMoveSpeed * Time.deltaTime);
        }
        
    }
}
