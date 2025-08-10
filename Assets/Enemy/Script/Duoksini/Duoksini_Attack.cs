using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;

public class Duoksini_Attack : EnemyAttack
{
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] Duoksini duoksini;
    [HideInInspector] PlayerController player;
    [SerializeField] GameObject attackRange;
    [HideInInspector] public bool isAttack = false;
    [HideInInspector] public bool isTrsPass = false;

    // 장승이 공격 범위를 보여주고 몇 초 뒤에 공격할지 정함
    [Tooltip("공격 범위가 보인 후 공격까지 걸리는 시간")]
    [SerializeField] float duoksiniAttackDelay;
    float duoksiniAttackTime;

    void Awake()
    {
        //플레이어를 찾아서 저장해준 후
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    bool isAttackReady = false;
    private void Update()
    {
        // 공격 중엔 시간이 안오름
        if (!isAttack) time += Time.deltaTime;
        if (isAttackReady) duoksiniAttackTime += Time.deltaTime;

        // 공격 준비가 다 되었음면 공격
        if (time >= enemyAttackSpeed && duoksini.isAttackReady && isTrsPass)
        {
            Debug.Log("범위 표시");
            isAttack = true;

            // 위치를 공격 범위 쪽으로 옳겨줌
            transform.position = duoksini.attackTargetTrs;

            // 사거리 표시
            attackRange.SetActive(true);

            isTrsPass = false;
            isAttackReady = true;
        }

        if (duoksiniAttackTime >= duoksiniAttackDelay)
        {

            Debug.Log("공격");
            // 파괴
            DestroyItem();

            // 사거리 비활성화
            attackRange.SetActive(false);

            // 콜라이더를 활성화 해줌
            enemyAttackCollider.enabled = true;

            // 초기화
            duoksiniAttackTime = 0;
            isAttack = false;
            duoksini.isAttackReady = false;
            isAttackReady = false;
            time = 0;


            // 콜라이더 비활성화
            Invoke("Dealy", 0.1f);
        }
    }


    // 플레이어 공격
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 공격
        if (collision.gameObject.CompareTag("Player") && !isAttack)
        {
            // 공격
            player.DamagedHP(enemyDamage);
        }
    }


    void DestroyItem()
    {
        Vector2 center = enemyAttackCollider.bounds.center;
        float radius = ((CircleCollider2D)enemyAttackCollider).radius * enemyAttackCollider.transform.lossyScale.x;

        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(center, radius, itemLayer);

        foreach (Collider2D collider in itemColliders)
        {
            ItemObject itemObject = collider.GetComponent<ItemObject>();

            if (itemObject != null)
            {
                Debug.Log("파괴: " + itemObject.name);
                Destroy(itemObject.gameObject);
            }
            else
            {
                Debug.Log("ItemObject 컴포넌트 없음: " + collider.name);
            }
        }
    }

    void Dealy()
    {
        enemyAttackCollider.enabled = false;
    }
}
