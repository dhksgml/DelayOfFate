using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result_Text : MonoBehaviour
{
    [Header("�ؽ�Ʈ")]
    [SerializeField] TextMeshProUGUI getGoldText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI currentGoldText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI finalGoldText;
    [SerializeField] TextMeshProUGUI dayText;

    [Header("��ġ")]
    // �̰� �ݿ����ٷ��� ��򰡿��� �� ������ ������ �־�� �� �� ����
    [HideInInspector] public float getGold;
    [HideInInspector] public int time;
    [HideInInspector] public float currentGold;
    [HideInInspector] public float cost;
    [HideInInspector] public float finalGold;
    [HideInInspector] public int day;

    public void Next_button() // �������� ���� �� �̵�
    {
        GameManager.Instance.Next_Day();
        SceneManager.LoadScene("Stage_Scene");
    }
    private void Start()
    {
        getGold = GameManager.Instance.N_Day_Add_Soul;
        time = GameManager.Instance.N_Day_Time;
        currentGold = GameManager.Instance.cu_soul;
        cost = GameManager.Instance.N_Day_Cost;
        finalGold = GameManager.Instance.Soul;
        day = GameManager.Instance.Day;
    }
    void Update()
    {
        getGoldText.text = "����� �� : " + getGold;
        timeText.text = "�Ҹ� �ð� : " + time;
        currentGoldText.text = "���� �ڻ� : " + currentGold;
        costText.text = "�� �� : -" + cost;
        finalGoldText.text = "�� �ڻ� : " + finalGold;
        dayText.text = GameManager.Instance.Day + " ����";
    }
}
