using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoar_Attack : EnemyAttack
{
    [SerializeField] WildBoar wildBoar;
    PlayerController player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        transform.position = enemy.transform.position;

        time += Time.deltaTime;

        rotationColl();

        if (time >= enemyAttackSpeed && distance <= 3f && !wildBoar.isStop)
        {

            // 에니메이션
            wildBoar.anim.SetTrigger("isAttack");

            enemyAttackCollider.enabled = true;
            enemy.isEnemyAttack = false;
            time = 0;
            Invoke("Delay", 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            player.DamagedHP(enemyDamage * enemy.cloakingDamage);
        }
    }

    public void Delay()
    {
        enemyAttackCollider.enabled = false;
    }
}
