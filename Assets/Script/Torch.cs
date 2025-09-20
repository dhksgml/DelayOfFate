using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    void Start()
    {
        int randomNum = Random.Range(0, 4);

        if (randomNum != 0)
        {
            Destroy(gameObject);
        }
    }
}
