using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSharkAttack : EnemyAttack
{
    [Header("땅상어")]
    [SerializeField] LandShark landShark;
    [SerializeField] BoxCollider2D hideOutAttackColl;
    bool isOutAttackReady; //돌출시 공격 확인용
    PlayerController player;
    public int landSharkJumpAttackDamage;
    public int currentDamage;

    void Awake()
    {
        //플레이어를 찾아서 저장해준 후
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        currentDamage = enemyDamage;
    }


    void Update()
    {
        transform.position = enemy.transform.position;
        //거리를 계산하고
        float distance = Vector3.Distance(transform.position, player.transform.position);

        //돌출 상태이고, 잠복 상태가 아니며, 추적중일시
        if (landShark.isOut && !landShark.isIn && landShark.isTrace)
        {
            transform.position = enemy.transform.position;
            //공격 준비가 안될 시에만
            if (!isOutAttackReady) time += Time.deltaTime;

            rotationColl();

            //공격 시간이 됐고, 목표와의 거리가 1f 보다 작거나 같으면 공격
            if (time >= enemyAttackSpeed && !isOutAttackReady
                && distance <= 5f)
            {
                //공격 준비 활성화
                isOutAttackReady = true;
                //에니메이션 활성화
                Debug.Log(4);
                //공격 콜라이더 활성화 해줌
                hideOutAttackColl.enabled = true;
                //시간 초기화
                time = 0;
                //0.5초후 공격 콜라이더 비활성화
                Invoke("AttackDelay", 0.5f);
            }
        }

    }

    public void AttackDelay()
    {
        Debug.Log(5);
        hideOutAttackColl.enabled = false;
        isOutAttackReady = false;
        //에니메이션
    }

    public void JumpAttack()
    {
        enemyDamage = 0;

        if (enemyAttackCollider is CircleCollider2D circleColl) { circleColl.radius = 3.0f; }
        //콜라이더 활성화를 해줌
        enemyAttackCollider.enabled = true;
        landShark.isStop = true;

        StartCoroutine(landShark.LandSharkJumpAttackMove());
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                //충돌시 데미지를 줌
                player.currentHp -= enemyDamage;
            }
        }
    }
}
