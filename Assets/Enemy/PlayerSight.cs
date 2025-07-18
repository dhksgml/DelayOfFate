using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    private Vector3 mousePosition;
    private Vector3 aimDir;
    private float angle;

    void Update()
    {
        TurnSight();
    }

    // 마우스 방향으로 보고 있음. 방향이 맞지 않으면 추후 의논
    void TurnSight()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // z축 고정
        aimDir = (mousePosition - transform.position).normalized;
        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // ← 여기에 -90도 보정
    }
}
