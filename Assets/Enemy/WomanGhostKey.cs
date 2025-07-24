using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanGhostKey : MonoBehaviour
{
    public GameObject womanGhostKeyCanvas;
    public bool isActive;

    public void Update()
    {
        // 활성화 되어있으면 켜줌
        if(isActive)
        {
            womanGhostKeyCanvas.SetActive(true);
        }
        // 비활성화 되어있으면 꺼줌
        else if (!isActive)
        {
            womanGhostKeyCanvas.SetActive(false);
        }
    }
}
