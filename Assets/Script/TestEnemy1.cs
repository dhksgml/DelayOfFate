using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy1 : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.transform.GetComponent<PlayerController>();
        player.SpendMp(10);
    }
}
