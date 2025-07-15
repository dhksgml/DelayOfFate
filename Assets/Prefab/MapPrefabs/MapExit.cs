using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExit : MonoBehaviour
{
    [SerializeField] private int nextMapIndex;
    [SerializeField] private string spawnPointName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MapManager.Instace?.LoadMap(nextMapIndex, spawnPointName);
        }
    }
}
