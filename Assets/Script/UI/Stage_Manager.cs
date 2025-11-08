using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Stage_Manager : MonoBehaviour
{
    public Sprite[] Quest_weapon_image;
    public Image Bk_image;

    public GameObject ShopPrefab; // 모든 상점 요소
    public GameObject QuestPrefab; //미션 카드 3개
    public GameObject WeaponPrefab; //무기 장착 페이지

    public GameObject Weapon_slot; //무기 슬롯 이미지
    public GameObject Weapon_slot_text; //무기 슬롯 텍스트

    public TMP_Text tooltip_text; // 툴팁
    WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        Mission_ch();
        
        MissionManager.Instance.Mission_ok = true; // 미션 부터 시작
        MissionManager.Instance.Mission_start();
    }
    public void Mission_ch() // 미션 씬으로 전환되었을때
    {
        Bk_image.sprite = Quest_weapon_image[1];//바꿔야함
        WeaponPrefab.SetActive(false);
        QuestPrefab.SetActive(true);
        ShopPrefab.SetActive(false);
        Weapon_slot.SetActive(false);
        Weapon_slot_text.SetActive(false);
        weaponManager.canProcessInput = false;
        tooltip_text.text = "조작[방향키]\n결정[Z]";
    }
    public void Weapon_ch() // 장비 씬으로 전환되었을때
    {
        Bk_image.sprite = Quest_weapon_image[2];//바꿔야함
        WeaponPrefab.SetActive(true);
        QuestPrefab.SetActive(false);
        ShopPrefab.SetActive(false);
        Weapon_slot.SetActive(true);
        Weapon_slot_text.SetActive(true);
        StartCoroutine(EnableWeaponInputAfterDelay());
        tooltip_text.text = "조작[방향키]\n장착[Z], 해제[X]";
    }
    IEnumerator EnableWeaponInputAfterDelay()
    {
        weaponManager.canProcessInput = false;

        // Z키가 떼어질 때까지 대기
        while (Input.GetKey(KeyCode.Z))
        {
            yield return null;
        }

        // 추가로 0.1초 대기 (안전장치)
        yield return new WaitForSeconds(0.1f);

        weaponManager.canProcessInput = true;
    }
    public void Shop_ch() // 상점 페이지로 전환
    {
        Bk_image.sprite = Quest_weapon_image[0];//바꿔야함
        WeaponPrefab.SetActive(false);
        QuestPrefab.SetActive(false);
        ShopPrefab.SetActive(true);
        Weapon_slot.SetActive(true);
        Weapon_slot_text.SetActive(true);
        weaponManager.canProcessInput = false;
        tooltip_text.text = "조작[방향키]\n구매[Z]";
    }
    public void Battle_ch() // 상점 전부 고른 후 전투씬으로 넘어가기
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
    public void Test_room_go()//메인씬(테스트 씬 으로 이동)
    {
        ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
        GameManager.Instance.SlotsData = shopQuickSlot.SlotsData; // 임시 데이터에 있던걸 게임매니저로 옮기기
        GameManager.Instance.LoadScene("MainScene");
    }
}
