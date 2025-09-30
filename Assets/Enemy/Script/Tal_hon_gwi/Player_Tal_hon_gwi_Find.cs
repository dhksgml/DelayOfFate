using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Tal_hon_gwi_Find : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Enemy"))
            {
                Tal_hon_gwi talhongwi = collision.gameObject.GetComponent<Tal_hon_gwi>();

                if (talhongwi != null)
                {
                    // 줍기 Z 판매 X
                    if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
                    {
                        talhongwi.isSeek = true;
                    }
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if(collision.CompareTag("Enemy"))
            {
                Tal_hon_gwi talhongwi = collision.gameObject.GetComponent<Tal_hon_gwi>();

                if (talhongwi != null)
                {
                    // 줍기 Z 판매 X
                    if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
                    {
                        talhongwi.isSeek = true;
                    }
                }
            }
        }
    }
}
