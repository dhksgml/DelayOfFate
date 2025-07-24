using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanGhostKey : MonoBehaviour
{
    public GameObject womanGhostKeyCanvas;
    public bool isActive;
    public GameObject leftKey;
    public GameObject rightKey;
    Coroutine keyCoroutine;


    public void Update()
    {
        // 활성화 되어있으면 켜줌
        if(isActive)
        {
            womanGhostKeyCanvas.SetActive(true);

            if (keyCoroutine == null)
            {
                keyCoroutine = StartCoroutine(KeyShow());
            }
        }
        else if (!isActive)
        {
            // 비활성화 시 코루틴 멈춤
            if (keyCoroutine != null)
            {
                StopCoroutine(keyCoroutine);
                keyCoroutine = null;
            }

            womanGhostKeyCanvas.SetActive(false);
        }
    }


    IEnumerator KeyShow()
    {
        while (isActive)
        {
            if (leftKey.activeSelf)
            {
                rightKey.SetActive(true);
                leftKey.SetActive(false);
            }
            else if (rightKey.activeSelf)
            {
                leftKey.SetActive(true);
                rightKey.SetActive(false);
            }

            yield return new WaitForSeconds(0.33f);
        }
    }
}
