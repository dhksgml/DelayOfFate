using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geuseundae_Bullet : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float damage = 10f;
    [HideInInspector] public bool isHide;
    [SerializeField] float playerMinusLightGage;
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer sp;

    Rigidbody2D rb;
    PlayerController player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // 플레이어 방향 계산
        Vector2 dir = (player.transform.position - transform.position).normalized;

        // 총알 회전 (오른쪽이 기본 전방일 경우)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;


        // 속도 적용
        rb.velocity = dir * speed;

        // 일정 시간 후 총알 삭제
        Destroy(gameObject, 5f);
    }

    bool isAnim;
    private void Update()
    {
        if (!isAnim)
        {
            anim.SetTrigger("isChange");
            isAnim = true;
        }
        

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
