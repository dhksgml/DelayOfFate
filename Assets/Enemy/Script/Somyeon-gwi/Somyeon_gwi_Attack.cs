using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Somyeon_gwi_Attack : EnemyAttack
{
    [Header("소면귀 공격")]
    [SerializeField] int somyeon_Gwi_Mind_Damage;
    public Somyeon_gwi somyeon_Gwi;
    [HideInInspector] PlayerController player;

    void Awake()
    {
        //플레이어를 찾아서 저장해준 후
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    float attackTime;
    void Update()
    {
        transform.position = enemy.transform.position;
        attackTime += Time.deltaTime;
    }


    //공격시
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                
                if (somyeon_Gwi.isHit)
                {
                    if (attackTime >= enemyAttackSpeed)
                    {
                        //충돌시 데미지를 줌
                        player.DamagedHP(enemyDamage);
                        player.DamagedMP(somyeon_Gwi_Mind_Damage);
                        attackTime = 0;
                    }
                }
            }
        }
    }
}
