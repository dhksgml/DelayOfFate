using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yin_YangTrace : EnemyAttack
{
    [Header("Yin_YangTrace")]
    [SerializeField] Yin_Yang yinyang;
    [SerializeField] GameObject follow;
    [SerializeField] float attackDelay;
    public Vector3 target;

    void Update()
    {
        transform.position = follow.transform.position;
        attackTime += Time.deltaTime;
    }

    float attackTime;

    void OnTriggerStay2D(Collider2D collision)
    {
        //적 태그이면
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Yin_Yang 스크립트를 가져온 후
            Yin_Yang yinYang = collision.GetComponent<Yin_Yang>();

            if (yinyang.type == Yin_Yang_Type.Yin && yinYang.type == Yin_Yang_Type.Yang)
            {
                //타겟의 좌표를 가져오고 bool값을 true로 해준다.
                target = collision.transform.position;
                yinyang.isFind = true;
            }

            else if (yinyang.type == Yin_Yang_Type.Yang && yinYang.type == Yin_Yang_Type.Yin)
            {
                //타겟의 좌표를 가져오고 bool값을 true로 해준다.
                target = collision.transform.position;
                yinyang.isFind = true;
            }
        }
        //공격 시간은 1초로 해주었음. 필요시 수정
        if (collision.gameObject.CompareTag("Player") && attackTime >= attackDelay)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            switch(yinyang.type)
            {
                case Yin_Yang_Type.Yin:
                    player.DamagedHP(enemyDamage); //데미지만큼 빼줌. 근데 -3으로 해놨음
                    attackTime = 0;
                    break;

                case Yin_Yang_Type.Yang:
                    player.currentMp += enemyDamage; //정신 회복

                    // 정신이 최대치를 넘어가면 최대값에 고정시켜줌
                    if (player.currentMp >= player.maxMp)
                    {
                        player.currentMp = player.maxMp;
                    }

                    attackTime = 0;
                    break;
            }
        }
    }
}
