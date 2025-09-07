using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap_Blind : MonoBehaviour
{
    // 복제된 미니맵 용 방
    public GameObject copyRoomObj;

    // 방을 가리는 블라인드
    public GameObject blindRoomObj;


    // 카피된 방의 블라인드 오브젝트를 가져오는 메서드
    public void CopyRoomBlindGet()
    {
        // 컴포넌트 추출 후
        Minimap_Blind blindCopy = copyRoomObj.GetComponent<Minimap_Blind>();
        // 가져와서 할당
        blindRoomObj = blindCopy.blindRoomObj;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (blindRoomObj == null) { return; }

        // 만약 플레이어가 들어와 있으면
        if (collision.CompareTag("Player"))
        {
            // 일단꺼주는걸로, 추후 하양 > 회색 > 검은색(기본)으로 변경
            blindRoomObj.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (blindRoomObj == null) { return; }

        // 만약 플레이어가 방을 떠나면
        if (collision.CompareTag("Player"))
        {
            // 일단꺼주는걸로, 추후 하양 > 회색 > 검은색(기본)으로 변경
            blindRoomObj.SetActive(true);
        }
    }
}
