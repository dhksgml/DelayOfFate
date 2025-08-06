using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Tal_hon_gwi_Find : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if(collision.CompareTag("Enemy"))
            {
                Tal_hon_gwi talhongwi = collision.gameObject.GetComponent<Tal_hon_gwi>();

                if (talhongwi != null)
                {
                    if (Input.GetKeyUp(KeyCode.E))
                    {
                        talhongwi.isSeek = true;
                    }
                }
            }
        }
    }
}
