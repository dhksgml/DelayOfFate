using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Blind : MonoBehaviour
{
    Minimap_Blind minimap_Blind;

    private void Awake()
    {
        minimap_Blind = GetComponentInParent<Minimap_Blind>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (minimap_Blind.blindRoomObj == null) { return; }

        // 만약 플레이어가 들어와 있으면
        if (collision.CompareTag("Player"))
        {
            if (minimap_Blind.isUseEyeReference) { return; }
            minimap_Blind.PlayerStayMap();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (minimap_Blind.blindRoomObj == null) { return; }

        // 만약 플레이어가 방을 떠나면
        if (collision.CompareTag("Player"))
        {
            if (minimap_Blind.isUseEyeReference) { return; }
            minimap_Blind.PlayerSeeMap();
        }
    }
}
