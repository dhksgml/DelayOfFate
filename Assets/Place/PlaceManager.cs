using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceManager : MonoBehaviour
{
    public Vector2 escape_pos;

    public bool resurrection; // 부활가능 상태
    public Vector2 resurrection_pos;

    public bool sale; // 판매
    public Vector2 sale_pos;

    public void Go_to_escape()
    {
        //게임매니저에 플레이어 정보 저장
        GameManager.Instance.SaveCurrentQuickSlot(FindObjectOfType<Player_Item_Use>().quickSlots);
        GameManager.Instance.SavePlayerInfo(FindObjectOfType<PlayerController>());
        SceneManager.LoadScene("Result_Scene");
    }
}
