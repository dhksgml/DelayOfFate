using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Place_Player_Find : MonoBehaviour
{
    public GameObject minimapPlace;   // MapSpawn에서 복제한 미니맵 장소 연결
    Minimap_Blind minimap;

    private void Start()
    {
        // 11.17 추가
        minimap = FindObjectOfType<Minimap_Blind>();
    }

    void Update()
    {
        if (minimap != null)
        {
            if (minimap.isUseEyeReference)
            {
                minimapPlace.SetActive(true);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && minimapPlace != null)
        {
            minimapPlace.SetActive(true);
        }
    }
}
