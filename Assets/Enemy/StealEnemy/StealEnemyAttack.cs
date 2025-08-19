using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEnemyAttack : EnemyAttack
{
    [SerializeField] StealEnemy stealEnemy;
    PlayerController player;

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

        //훔치지 않았으면 공격
        if (!stealEnemy.isStealGold) time += Time.deltaTime;

        rotationColl();


        if (stealEnemy.isStealGold)
        {
            stealEnemy.anim.SetBool("isAttack", false);
        }

        //공격 시간이 됐고, 목표와의 거리가 3f 보다 작거나 같으면 공격
        else if (time >= enemyAttackSpeed && !stealEnemy.isStealGold
            && distance <= 3f)
        {
            //에니메이션 활성화
            stealEnemy.anim.SetBool("isAttack", true);

            //공격 콜라이더 활성화 해줌
            enemyAttackCollider.enabled = true;
            enemy.isEnemyAttack = false;
            time = 0;
            Invoke("Delay", 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !stealEnemy.isAttack)
        {

            // 에니메이션 적용
            stealEnemy.anim.SetBool("isSteal", true);

            // 스프라이트 적용
            stealEnemy.sp.sprite = stealEnemy.stealSprite;

            stealEnemy.isStealGold = true;
            stealEnemy.isAttack = true;

            // 공격
            player.DamagedHP(enemyDamage);
            // 기력 데미지. -50% 이기에 최대 스테미너 / 2 를 해주었음
            player.DamagedSP(player.maxSp / 2);
        }
    }

    public void Delay()
    {
        enemyAttackCollider.enabled = false;
        stealEnemy.isAttack = false;
        //stealEnemy.anim.SetBool("isAttack", false);
    }
}
