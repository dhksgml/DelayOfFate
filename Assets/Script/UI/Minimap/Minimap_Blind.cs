using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap_Blind : MonoBehaviour
{
    // 복제된 미니맵 용 방
    public GameObject copyRoomObj;

    // 방을 가리는 블라인드
    public GameObject blindRoomObj;

    // 아직 방을 안갔을 시 비활성화 해주기 위함
    public GameObject dontSeeRoom;

    // 장소 아이콘 저장용
    public GameObject placeIcon;

    SpriteRenderer sp;

    bool isFind = false;

    // 눈을 사용하면 true로 변경,
    // 전역으로 돌려서 한개만 true되면 다 멈추도록 하는게 좋은듯
    public static bool isUseEye = false;
    // 외부 참조용
    public bool isUseEyeReference = false;

    private void Update()
    {
        if (isUseEye) 
        {
            isUseEyeReference = true;
            UseEyeMapBlind();
        }
        else if (!isUseEye) { isUseEyeReference = false; }
    }

    // 눈 사용시
    void UseEyeMapBlind()
    {
        if (blindRoomObj != null)
        {
            sp = blindRoomObj.GetComponent<SpriteRenderer>();

            sp.color = Color.gray;

            Color c = sp.color;
            c.a = 0.75f;
            sp.color = c;
        }

        if(dontSeeRoom != null)
        {
            dontSeeRoom.SetActive(true);
        }

    }

    // 스테틱 true
    public void UseEye()
    {
        isUseEye = true;
    }

    // 스태틱 false
    public void InitEye()
    {
        isUseEye = false;
    }

    // 카피된 방의 블라인드 오브젝트를 가져오는 메서드
    public void CopyRoomBlindGet()
    {
        // 컴포넌트 추출 후
        Minimap_Blind blindCopy = copyRoomObj.GetComponent<Minimap_Blind>();
        // 가져와서 할당
        blindRoomObj = blindCopy.blindRoomObj;
    }

    public void RoomSetActiveFalse()
    {
        dontSeeRoom.SetActive(false);
    }

    // 플레이어가 맵에 존재하면
    public void PlayerStayMap()
    {
        if (blindRoomObj != null)
        {
            sp = blindRoomObj.GetComponent<SpriteRenderer>();

            sp.color = Color.white;

            Color c = sp.color;
            c.a = 1f;
            sp.color = c;

            isFind = true;

            blindRoomObj.SetActive(false);

            if (isFind == true && dontSeeRoom != null)
            {
                dontSeeRoom.SetActive(true);
            }
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
            //if (isUseEye) { return; }
            PlayerStayMap();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (blindRoomObj == null) { return; }

        // 만약 플레이어가 방을 떠나면
        if (collision.CompareTag("Player"))
        {
            //if (isUseEye) { return; }
            PlayerSeeMap();
        }
    }
}
