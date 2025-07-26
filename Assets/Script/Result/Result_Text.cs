using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result_Text : MonoBehaviour
{
    [Header("텍스트")]
    [SerializeField] TextMeshProUGUI getGoldText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI currentGoldText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI finalGoldText;
    [SerializeField] TextMeshProUGUI dayText;

    [Header("수치")]
    // 이걸 반영해줄려면 어딘가에서 이 값을들 저장해 주어야 할 것 같음
    [HideInInspector] public float getGold;
    [HideInInspector] public int time;
    [HideInInspector] public float currentGold;
    [HideInInspector] public float cost;
    [HideInInspector] public float finalGold;
    [HideInInspector] public int day;

    public void Next_button() // 스테이지 선택 씬 이동
    {
        // 게임 오버 씬
        if(finalGold < 0)
        {
            SceneManager.LoadScene("Gameover_Scene");
            return;
        }
        // 클리어 씬
        else if (day >= 3)
        {
            SceneManager.LoadScene("Clear_Scene");
            return;
        }

        GameManager.Instance.Next_data_reset();
        SceneManager.LoadScene("Stage_Scene");
    }
    private void Start()
    {
        getGold = GameManager.Instance.N_Day_Add_Soul;
        time = GameManager.Instance.N_Day_Time;
        currentGold = GameManager.Instance.N_Day_current_Soul;
        cost = GameManager.Instance.N_Day_Cost;
        finalGold = GameManager.Instance.Soul;
        day = GameManager.Instance.Day;
    }
    void Update()
    {
        getGoldText.text = "벌어온 돈 : " + getGold;
        timeText.text = "소모 시간 : " + time + "각";
        currentGoldText.text = "현재 자산 : " + currentGold;
        costText.text = "약 값 : -" + cost;
        finalGoldText.text = "총 자산 : " + finalGold;
        dayText.text = GameManager.Instance.Day + " 일차";
    }
}
