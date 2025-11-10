using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geuseundae_Attack : EnemyAttack
{
    [SerializeField] Geuseundae geuseundae;
    [HideInInspector] PlayerController player;
    [SerializeField] GameObject bullet;


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

        rotationColl();

        if (distance <= enemyAttackRange)
        {
            time += Time.deltaTime;

            geuseundae.isAttack = true;

            if (time >= enemyAttackSpeed)
            {
                // 투사체 발사
                GameObject bullet = Instantiate(this.bullet, transform.position, Quaternion.identity);

                Geuseundae_Bullet component = bullet.GetComponentInChildren<Geuseundae_Bullet>();
                time = 0;

            }
        }
        else if (distance >= enemyAttackRange)
        {
            geuseundae.isAttack = false;
        }
    }
}
