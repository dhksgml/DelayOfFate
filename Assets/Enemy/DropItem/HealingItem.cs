using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [SerializeField] float healing;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                // 회복
                player.currentHp += healing;

                // 최대 채력 넘어가면 방지
                if (player.currentHp >= player.maxHp)
                {
                    player.currentHp = player.maxHp;
                }

                // 삭제
                Destroy(gameObject);
            }
        }
    }
}
