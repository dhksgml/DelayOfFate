using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] float speed;

    [Header("Reference")]
    public GameObject playerSight;

    Vector2 inputVec;
    Rigidbody2D rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //플레이어의 입력 값을 벡터로 저장해줌
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        PlayerMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if(collision.gameObject.CompareTag("EnemyAttack"))
            {
                Debug.Log("PlayerHit");
            }
        }
    }


    void PlayerMove()
    {
        //movePosition 사용
        Vector2 nextVector = inputVec.normalized * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + nextVector);

        if (inputVec.x != 0 || inputVec.y != 0)
        {
            float angle = Mathf.Atan2(nextVector.y, nextVector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
}
