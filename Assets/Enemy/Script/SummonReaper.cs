using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonReaper : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] QuickSlotUI time;
    [SerializeField] RoomRandomPlacement roomTransform;
    [SerializeField] GameObject reaper;

    [Header("트리거")]
    [SerializeField] bool isSpawn;

    [Header("소환 시간")]
    [SerializeField] float spawnTime;

    int randomNum;

    void Start()
    {
        // 랜덤한 방을 위함
        randomNum = Random.Range(0, roomTransform.randomPlace.Count + 1);
    }

    void Update()
    {
        if (time.angleUnit >= spawnTime && !isSpawn)
        {

            Instantiate(reaper, roomTransform.randomPlace[randomNum], Quaternion.identity);

            isSpawn = true;
        }
    }
}
