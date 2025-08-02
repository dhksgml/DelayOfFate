using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    void Start()
    {
        Invoke("DestroyEffect", 5f);
    }

    void DestroyEffect()
    {
        Destroy(gameObject);
    }

}
