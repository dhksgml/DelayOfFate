using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instace { get; private set; }

    public GameObject[] mapPrefabs;
    public Transform mapParent;
    private GameObject currentMap;

    private void Awake()
    {
        Instace = this;

        LoadMap(0, "Spawn");
    }

    public void LoadMap(int index, string entranceName)
    {
        if (currentMap != null)
            Destroy(currentMap);

        currentMap = Instantiate(mapPrefabs[index], mapParent.transform);
        currentMap.SetActive(true);

        // 플레이어를 지정한 입구 위치로 이동
        Transform entrance = currentMap.transform.Find(entranceName);
        if (entrance != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = entrance.position;
        }

        // 적에게 새로운 순찰 포인트 전달
        //EnemyAI enemy = FindObjectOfType<EnemyAI>();
        //PatrolPointsContainer patrols = currentMap.GetComponentInChildren<PatrolPointsContainer>();
        //if (enemy && patrols != null)
        //{
        //    enemy.SetPatrolPoints(patrols.GetPatrolPoints());
        //}
    }

}
