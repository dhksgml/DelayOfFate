using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanGhostAttack : EnemyAttack
{
    [SerializeField] WomanGhost womanGhost;
    [HideInInspector] PlayerController player;


     void Awake()
    {
        //플레이어를 찾아서 저장해준 후
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();   
    }


    void Update()
    {
        //거리를 계산하고
        float distance = Vector3.Distance(transform.position, player.transform.position);

        transform.position = enemy.transform.position;
        if(!womanGhost.isAttack) time += Time.deltaTime;

        rotationColl();

        //공격 시간이 됐고, 목표와의 거리가 1f 보다 작거나 같으면 공격
        if (time >= enemyAttackSpeed && !womanGhost.isAttack 
            && distance <= 1f)
        {
            //에니메이션 활성화
            womanGhost.anim.SetBool("isAttack", true);

            //공격 콜라이더 활성화 해줌
            enemyAttackCollider.enabled = true;
            enemy.isEnemyAttack = false;
            time = 0;
            Invoke("Delay", 0.5f);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !womanGhost.isAttack)
        {
            //처녀귀신 스크립트에 작성해두었던 코루틴 실행
            StartCoroutine(womanGhost.PlayerPossession());
        }
    }

    public void Delay()
    {
        enemyAttackCollider.enabled = false;
        womanGhost.anim.SetBool("isAttack", false);
    }
}
