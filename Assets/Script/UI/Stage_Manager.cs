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
        if(GameManager.Instance != null)
        {
            int weaponCount = 0;

            if(GameManager.Instance.WeaponData.Length != 2)
                return;

            for(int i = 0; i < 2; i++)
            {
                if (GameManager.Instance.WeaponData[i] == null)
                    continue;
                weaponCount += 1;
            }

            if(weaponCount == 0)
            {
                Debug.Log("넌 못 지나간다");
                Shop shop = FindObjectOfType<Shop>();
                if (shop != null)
                    shop.speech_bubble_on("무기");
                return;
            }
        }
            
        ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
        GameManager.Instance.SlotsData = shopQuickSlot.SlotsData; // 임시 데이터에 있던걸 게임매니저로 옮기기
        GameManager.Instance.LoadScene("InGame_Scenes");
    }
}
