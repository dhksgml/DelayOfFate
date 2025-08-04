using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seokdeungnyeong_Trace : MonoBehaviour
{
    [SerializeField] Seokdeungnyeong seokdeungnyeong;
    [SerializeField] GameObject followObj;


    void Update()
    {
        transform.position = followObj.transform.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                seokdeungnyeong.isPlayer = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                seokdeungnyeong.isPlayer = false;
            }
        }
    }
}
