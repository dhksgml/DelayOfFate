using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mental_Power_Mp : MonoBehaviour
{
    private float timer = 0f;
    private int[] state;
    public bool low_mp;
    public bool barry_low_mp;

    private void Start()
    {
        state = PickTwoDistinctNumbers();
        print(state);
    }
    int[] PickTwoDistinctNumbers()
    {
        List<int> numbers = new List<int> { 0, 1, 2, 3 };
        int first = numbers[Random.Range(0, numbers.Count)];
        numbers.Remove(first);
        int second = numbers[Random.Range(0, numbers.Count)];
        return new int[] { first, second };
    }
    private void Update()
    {
        //증상 효과는 5초마다 확률적으로 발동
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            float rand_1 = Random.Range(0f, 1f);
            if (rand_1 < 0.15f)
                if (low_mp) { Low_MP(0); }
            else if (rand_1 < 0.35f)
                if (barry_low_mp){ Barry_low_mp(0); }

            float rand_2 = Random.Range(0f, 1f);
            if (rand_2 < 0.15f)
                if (low_mp) { Low_MP(1); }
                else if (rand_2 < 0.35f)
                    if (barry_low_mp) { Barry_low_mp(2); }
            // 독립 시행
            timer = 0f;
        }
    }
    public void Low_MP(int number) // 50% 정신 이하
    {
        switch (state[number])
        {
            case 0: Hallucinations(); break;
            case 1: Auditory_hallucinations(); break;
            case 2: Disarray(); break;
            case 3: Liver_and_gall_are_cold(); break;
        }
    }
    public void Barry_low_mp(int number)// 25% 정신 이하
    {
        switch (state[number])
        {
            case 0: Hallucinations(); break;
            case 1: Auditory_hallucinations(); break;
            case 2: Disarray(); break;
            case 3: Liver_and_gall_are_cold(); break;
        }
    }
    void Hallucinations()//“헛것이 보이기 시작합니다…” 가끔씩 대상을 잘못보고, 허공에 물건,적 을 봄
    {
        // 플레이어 화면에 스프라이트만 있는 프리팹을 다양하게 배치 해서 한개를 랜덤한 그림을 넣어서 보이게끔
        print("환각 발생");
    }
    void Auditory_hallucinations()//“환청이 들리기 시작합니다…” 가끔씩 악귀의 소리나 이상한 소리를 들음
    {
        //이것도 다량의 사운드 구비한 다음 그때 구현
        print("환청 발생");
    }
    void Disarray()//“의지가 흐리기 시작합니다…” 가끔씩 행동하지 않고, 키 반전이 됨
    {
        //이건 해볼만 한데
        print("혼란 발생");
    }
    void Liver_and_gall_are_cold()//“간담이 서늘하기 시작합니다…” 암흑시야가 완벽하게 안보임
    {
        //이것도 시야 건드는 거긴 한데 해볼만은 하다
        print("공포 발생");
    }
}
