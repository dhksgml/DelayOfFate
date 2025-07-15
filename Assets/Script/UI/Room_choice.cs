using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Room_choice : MonoBehaviour
{
    public TMP_Text Request_Name_Text; //미션명
    //미션 종류 : 골드 획득, 특정 악귀 처치, 악귀 몇마리 제거, 유해품 수거, 특정 물건 수거, 물건 몇개 수거, 봉인 유지보수
    public TMP_Text Request_Content_Text; //미션 내용
    public TMP_Text Level_Content_Text; //상세 내용
    public Sprite[] Soul_Image; //보상 영혼 이미지 ?개
    public Sprite[] Weapon_Image; //무기 이미지 5개
    public Image[] Ch_Weapon_Image; //약점에 대응 되는 무기 (최대 3)
    private enum Mission_Type // 미션 종류
    {
        Add_Coin,
        Target_Kill,
        Many_Kill,
        Harmful_Collect,
        Soul_Collect,
        Many_Collect,
        Seal_Maintenance
    }
    private Mission_Type mission_type;
    Mission_Type[] candidateMissions = new Mission_Type[]
    {
        Mission_Type.Add_Coin,
        Mission_Type.Target_Kill,
        Mission_Type.Many_Kill,
        Mission_Type.Harmful_Collect,
        Mission_Type.Soul_Collect,
        Mission_Type.Many_Collect,
        Mission_Type.Seal_Maintenance
    };
    private int Room_Quantity; //방곗수
    private int Difficulty; //적 난이도
    private string Quest = "미션 내용";
    private int Quest_number;
    private int Reward; // 보상
    private enum Recommended_Weapon // 추천 무기
    {
        Sword,
        Bat,
        Paper,
        Scroll,
        Bottle
    }
    private Recommended_Weapon recommended_weapon;
    private int Random_Weapon;
    void Start()
    {
        Random_Request();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) { Random_Request(); }
    }
    void Random_Request()
    {
        mission_type = candidateMissions[UnityEngine.Random.Range(0, candidateMissions.Length)];
        Room_Quantity = Random.Range(9, 26);
        Difficulty = Random.Range(0, 3);
        Reward = Random.Range(0, 25);
        Random_Weapon = Random.Range(0, 5);
        switch (mission_type)
        {
            case Mission_Type.Add_Coin:
                Request_Name_Text.text = "수익 달성";
                Request_Content_Text.text = "수단과 방법을 가리지 않고 돈을 확보하세요";
                Quest_number = Random.Range(2, 8);
                Quest_number *= 100; // 100단위 랜덤
                Quest = $"{Quest_number} 전 획득";
                break;

            case Mission_Type.Target_Kill:
                Request_Name_Text.text = "지정 토벌";
                Request_Content_Text.text = "지정된 타겟 악귀를 찾아 제거하세요.";
                Quest = $"악귀 XXX 처치";
                break;

            case Mission_Type.Many_Kill:
                Request_Name_Text.text = "구역 정화";
                Request_Content_Text.text = "정해진 수 이상의 악귀를 처치하십시오.";
                Quest_number = Random.Range(3, 7);
                Quest = $"악귀 {Quest_number} 처치";
                break;

            case Mission_Type.Harmful_Collect:
                Request_Name_Text.text = "유품 수거";
                Request_Content_Text.text = "유품 물품을 수거하세요.";
                Quest = $"유품 수색후 수거";
                break;

            case Mission_Type.Soul_Collect:
                Request_Name_Text.text = "영혼 수거";
                Request_Content_Text.text = "악귀를 처치하고 영혼을 수거하세요";
                Quest_number = Random.Range(2, 5);
                Quest = $"영혼 {Quest_number} 수거";
                break;

            case Mission_Type.Many_Collect:
                Request_Name_Text.text = "주물 대량 수거";
                Request_Content_Text.text = "정해진 수 이상의 물건을 모아야 합니다.";
                Quest_number = Random.Range(4, 10);
                Quest = $"영혼 {Quest_number} 수거";
                break;

            case Mission_Type.Seal_Maintenance:
                Request_Name_Text.text = "봉인 보수";
                Request_Content_Text.text = "마력이 약해진 봉인을 복구하세요.";
                Quest_number = Random.Range(2, 5);
                Quest = $"봉인 {Quest_number} 개 수리";
                break;

            default:
                Request_Name_Text.text = "알 수 없는 미션";
                break;
        }
        string[] sizeStages = { "최소", "소", "중", "대", "최대" };
        string[] diffStages = { "최하", "하", "중", "상", "최상" };
        string[] colors = { "#00FF00", "#7FFF00", "#FFA500", "#FF4500", "#FF0000" };

        // 방 크기 등급 계산
        int sizeIndex;
        if (Room_Quantity <= 10) sizeIndex = 0;
        else if (Room_Quantity <= 13) sizeIndex = 1;
        else if (Room_Quantity <= 16) sizeIndex = 2;
        else if (Room_Quantity <= 20) sizeIndex = 3;
        else sizeIndex = 4;

        // 난이도 등급 계산 (1~5로 가정)
        int diffIndex = Mathf.Clamp(Difficulty - 1, 0, 4);

        // 색상 적용한 텍스트 구성
        string roomText = $"<color={colors[sizeIndex]}>{sizeStages[sizeIndex]}</color>";
        string diffText = $"<color={colors[diffIndex]}>{diffStages[diffIndex]}</color>";
        Level_Content_Text.text =
        $"[{roomText}] / [{diffText}]\n" +
        $"[목표]\n{Quest}\n\n" +
        $"[보상]\n{Reward} 번째 영혼";
        Ch_Weapon_Image[0].sprite = Weapon_Image[Random_Weapon];
        Ch_Weapon_Image[1].sprite = Weapon_Image[Random_Weapon];
        Ch_Weapon_Image[2].sprite = Weapon_Image[Random_Weapon];
    }
}
