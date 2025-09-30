using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boon_yel_gwi_Attack : EnemyAttack
{
    [SerializeField] Boon_yeol_gwi_Remake boonyeolgwi;
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
            Debug.Log(1);
            time += Time.deltaTime;

            boonyeolgwi.isFind = true;

            if (time >= enemyAttackSpeed)
            {
                Debug.Log(2);
                // 투사체 발사
                GameObject bullet = Instantiate(this.bullet, transform.position, Quaternion.identity);

                Boon_yeol_gwi_bullet component = bullet.GetComponent<Boon_yeol_gwi_bullet>();
                if (boonyeolgwi.iscloaking == true) { component.isHide = true; }
                time = 0;

            }
        }
        else if (distance >= enemyAttackRange)
        {
            boonyeolgwi.isFind = false;
        }
    }
}
