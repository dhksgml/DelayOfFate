using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place_Player_Find : MonoBehaviour
{
    public GameObject minimapPlace;   // MapSpawn에서 복제한 미니맵 장소 연결

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && minimapPlace != null)
        {
            minimapPlace.SetActive(true);
        }
    }
}
