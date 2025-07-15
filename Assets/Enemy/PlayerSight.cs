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

    void TurnSight()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDir = (transform.position - mousePosition).normalized;
        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
