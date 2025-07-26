using UnityEngine;

public class Stage_Manager : MonoBehaviour
{
    public GameObject ShopPrefab; // 모든 상점 요소
    public GameObject QuestPrefab; //미션 카드 3개
    public void Quest_ok() // 미션을 고른 후 상점 페이지로 전환
    {
        ShopPrefab.SetActive(true);
        QuestPrefab.SetActive(false);
    }
    public void Shop_end() // 상점 전부 고른 후 전투씬으로 넘어가기
    {
        if(GameManager.Instance.isTutorial)
        {
            GameManager.Instance.LoadScene("Tutorial_Scenes");
        }
        else
        {
            ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
            GameManager.Instance.SlotsData = shopQuickSlot.SlotsData; // 임시 데이터에 있던걸 게임매니저로 옮기기
            GameManager.Instance.LoadScene("InGame_Scenes");
        }
    }
}
