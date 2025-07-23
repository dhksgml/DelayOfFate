using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceManager : MonoBehaviour
{
    [HideInInspector] public Vector2 escape_pos;

    [HideInInspector] public bool resurrection; // 부활가능 상태
    [HideInInspector] public Vector2 resurrection_pos;

    [HideInInspector] public bool sale; // 판매
    [HideInInspector] public Vector2 sale_pos;

    public void Go_to_escape()
    {
        //게임매니저에 플레이어 정보 저장
        GameManager.Instance.SaveCurrentQuickSlot(FindObjectOfType<Player_Item_Use>().quickSlots);
        GameManager.Instance.SavePlayerInfo(FindObjectOfType<PlayerController>());
        SceneManager.LoadScene("Result_Scene");
    }
}
