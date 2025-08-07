using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

            if (collision.gameObject.CompareTag("Light"))
            {
                Light2D light = collision.gameObject.GetComponent<Light2D>();
                light.enabled = false;
                seokdeungnyeong.isPlayer = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {

            if (collision.gameObject.CompareTag("Light"))
            {
                Light2D light = collision.gameObject.GetComponent<Light2D>();
                light.enabled = true;
                seokdeungnyeong.isPlayer = false;
            }
        }
    }
}
