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

                if (!player.isHealBan)
                {
                    // 회복
                    player.Hp_add(healing);
                }

                // 삭제
                Destroy(gameObject);
            }
        }
    }
}
