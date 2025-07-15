using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Room_choice : MonoBehaviour
{
    public TMP_Text Request_Name_Text; //�̼Ǹ�
    //�̼� ���� : ��� ȹ��, Ư�� �Ǳ� óġ, �Ǳ� ��� ����, ����ǰ ����, Ư�� ���� ����, ���� � ����, ���� ��������
    public TMP_Text Request_Content_Text; //�̼� ����
    public TMP_Text Level_Content_Text; //�� ����
    public Sprite[] Soul_Image; //���� ��ȥ �̹��� ?��
    public Sprite[] Weapon_Image; //���� �̹��� 5��
    public Image[] Ch_Weapon_Image; //������ ���� �Ǵ� ���� (�ִ� 3)
    private enum Mission_Type // �̼� ����
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
    private int Room_Quantity; //����
    private int Difficulty; //�� ���̵�
    private string Quest = "�̼� ����";
    private int Quest_number;
    private int Reward; // ����
    private enum Recommended_Weapon // ��õ ����
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
                Request_Name_Text.text = "���� �޼�";
                Request_Content_Text.text = "���ܰ� ����� ������ �ʰ� ���� Ȯ���ϼ���";
                Quest_number = Random.Range(2, 8);
                Quest_number *= 100; // 100���� ����
                Quest = $"{Quest_number} �� ȹ��";
                break;

            case Mission_Type.Target_Kill:
                Request_Name_Text.text = "���� ���";
                Request_Content_Text.text = "������ Ÿ�� �Ǳ͸� ã�� �����ϼ���.";
                Quest = $"�Ǳ� XXX óġ";
                break;

            case Mission_Type.Many_Kill:
                Request_Name_Text.text = "���� ��ȭ";
                Request_Content_Text.text = "������ �� �̻��� �Ǳ͸� óġ�Ͻʽÿ�.";
                Quest_number = Random.Range(3, 7);
                Quest = $"�Ǳ� {Quest_number} óġ";
                break;

            case Mission_Type.Harmful_Collect:
                Request_Name_Text.text = "��ǰ ����";
                Request_Content_Text.text = "��ǰ ��ǰ�� �����ϼ���.";
                Quest = $"��ǰ ������ ����";
                break;

            case Mission_Type.Soul_Collect:
                Request_Name_Text.text = "��ȥ ����";
                Request_Content_Text.text = "�Ǳ͸� óġ�ϰ� ��ȥ�� �����ϼ���";
                Quest_number = Random.Range(2, 5);
                Quest = $"��ȥ {Quest_number} ����";
                break;

            case Mission_Type.Many_Collect:
                Request_Name_Text.text = "�ֹ� �뷮 ����";
                Request_Content_Text.text = "������ �� �̻��� ������ ��ƾ� �մϴ�.";
                Quest_number = Random.Range(4, 10);
                Quest = $"��ȥ {Quest_number} ����";
                break;

            case Mission_Type.Seal_Maintenance:
                Request_Name_Text.text = "���� ����";
                Request_Content_Text.text = "������ ������ ������ �����ϼ���.";
                Quest_number = Random.Range(2, 5);
                Quest = $"���� {Quest_number} �� ����";
                break;

            default:
                Request_Name_Text.text = "�� �� ���� �̼�";
                break;
        }
        string[] sizeStages = { "�ּ�", "��", "��", "��", "�ִ�" };
        string[] diffStages = { "����", "��", "��", "��", "�ֻ�" };
        string[] colors = { "#00FF00", "#7FFF00", "#FFA500", "#FF4500", "#FF0000" };

        // �� ũ�� ��� ���
        int sizeIndex;
        if (Room_Quantity <= 10) sizeIndex = 0;
        else if (Room_Quantity <= 13) sizeIndex = 1;
        else if (Room_Quantity <= 16) sizeIndex = 2;
        else if (Room_Quantity <= 20) sizeIndex = 3;
        else sizeIndex = 4;

        // ���̵� ��� ��� (1~5�� ����)
        int diffIndex = Mathf.Clamp(Difficulty - 1, 0, 4);

        // ���� ������ �ؽ�Ʈ ����
        string roomText = $"<color={colors[sizeIndex]}>{sizeStages[sizeIndex]}</color>";
        string diffText = $"<color={colors[diffIndex]}>{diffStages[diffIndex]}</color>";
        Level_Content_Text.text =
        $"[{roomText}] / [{diffText}]\n" +
        $"[��ǥ]\n{Quest}\n\n" +
        $"[����]\n{Reward} ��° ��ȥ";
        Ch_Weapon_Image[0].sprite = Weapon_Image[Random_Weapon];
        Ch_Weapon_Image[1].sprite = Weapon_Image[Random_Weapon];
        Ch_Weapon_Image[2].sprite = Weapon_Image[Random_Weapon];
    }
}
