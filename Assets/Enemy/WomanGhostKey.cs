using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanGhostKey : MonoBehaviour
{
    public GameObject womanGhostKeyCanvas;
    public bool isActive;

    public void Update()
    {
        // Ȱ��ȭ �Ǿ������� ����
        if(isActive)
        {
            womanGhostKeyCanvas.SetActive(true);
        }
        // ��Ȱ��ȭ �Ǿ������� ����
        else if (!isActive)
        {
            womanGhostKeyCanvas.SetActive(false);
        }
    }
}
