using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceManager : MonoBehaviour
{
    [HideInInspector] public Vector2 escape_pos;

    [HideInInspector] public bool resurrection; // 부활가능 상태
    [HideInInspector] public Vector2 resurrection_pos;

    [HideInInspector] public bool sale; // 판매
    [HideInInspector] public Vector2 sale_pos;


    // 미니맵용
    [HideInInspector] public List<Vector2> resurrection_positions = new();
    [HideInInspector] public List<Vector2> sale_positions = new();
    [HideInInspector] public List<Vector2> escape_positions = new();

    [HideInInspector] public List<Vector2> soul_positions = new();
    [HideInInspector] public List<Vector2> coin_positions = new();
    [HideInInspector] public List<Vector2> eye_positions = new();
    public void Resurrection()
    {
        resurrection = false;
    }

    public void Go_to_escape()
    {
        //게임매니저에 플레이어 정보 저장
        GameManager.Instance.SaveCurrentQuickSlot(FindObjectOfType<Player_Item_Use>().quickSlots);
        GameManager.Instance.SavePlayerInfo(FindObjectOfType<PlayerController>());
        GameManager.Instance.New_Day_date(FindObjectOfType<QuickSlotUI>().angleUnit);
        GameManager.Instance.WeaponData = new ItemData[2];
        SceneManager.LoadScene("Result_Scene");
    }
}
