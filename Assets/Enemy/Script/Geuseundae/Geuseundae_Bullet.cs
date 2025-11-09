using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geuseundae_Bullet : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float damage = 10f;
    [HideInInspector] public bool isHide;
    [SerializeField] float playerMinusLightGage;

    Rigidbody2D rb;
    PlayerController player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // 플레이어 방향 계산
        Vector2 dir = (player.transform.position - transform.position).normalized;

        // 속도 적용
        rb.velocity = dir * speed;

        // 일정 시간 후 총알 삭제
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.DamagedHP(damage);
            player.currentRadius -= playerMinusLightGage;
            Destroy(gameObject);
        }
    }
}
