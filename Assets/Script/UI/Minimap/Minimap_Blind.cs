using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap_Blind : MonoBehaviour
{
    // 복제된 미니맵 용 방
    public GameObject copyRoomObj;

    // 방을 가리는 블라인드
    public GameObject blindRoomObj;

    SpriteRenderer sp;


    // 카피된 방의 블라인드 오브젝트를 가져오는 메서드
    public void CopyRoomBlindGet()
    {
        // 컴포넌트 추출 후
        Minimap_Blind blindCopy = copyRoomObj.GetComponent<Minimap_Blind>();
        // 가져와서 할당
        blindRoomObj = blindCopy.blindRoomObj;
    }

    // 플레이어가 맵에 존재하면
    public void PlayerStayMap()
    {
        if (blindRoomObj != null)
        {
            sp = blindRoomObj.GetComponent<SpriteRenderer>();

            sp.color = Color.white;

            blindRoomObj.SetActive(false);
        }
    }

    // 플레이어가 맵을 처음 봤으면
    public void PlayerSeeMap()
    {
        if (blindRoomObj != null)
        {
            sp = blindRoomObj.GetComponent<SpriteRenderer>();

            Color c = Color.gray;

            c.a = 0.5f;

            sp.color = c;   

            blindRoomObj.SetActive(true);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (blindRoomObj == null) { return; }

        // 만약 플레이어가 들어와 있으면
        if (collision.CompareTag("Player"))
        {
            PlayerStayMap();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (blindRoomObj == null) { return; }

        // 만약 플레이어가 방을 떠나면
        if (collision.CompareTag("Player"))
        {
            PlayerSeeMap();
        }
    }
}
