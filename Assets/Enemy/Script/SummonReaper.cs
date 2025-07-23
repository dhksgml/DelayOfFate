using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonReaper : MonoBehaviour
{
    [Header("����")]
    [SerializeField] QuickSlotUI time;
    [SerializeField] RoomRandomPlacement roomTransform;
    [SerializeField] GameObject reaper;

    [Header("Ʈ����")]
    [SerializeField] bool isSpawn;

    [Header("��ȯ �ð�")]
    [SerializeField] float spawnTime;

    int randomNum;

    void Start()
    {
        // ������ ���� ����
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
