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
        //���� ȿ���� 5�ʸ��� Ȯ�������� �ߵ�
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
            // ���� ����
            timer = 0f;
        }
    }
    public void Low_MP(int number) // 50% ���� ����
    {
        switch (state[number])
        {
            case 0: Hallucinations(); break;
            case 1: Auditory_hallucinations(); break;
            case 2: Disarray(); break;
            case 3: Liver_and_gall_are_cold(); break;
        }
    }
    public void Barry_low_mp(int number)// 25% ���� ����
    {
        switch (state[number])
        {
            case 0: Hallucinations(); break;
            case 1: Auditory_hallucinations(); break;
            case 2: Disarray(); break;
            case 3: Liver_and_gall_are_cold(); break;
        }
    }
    void Hallucinations()//������� ���̱� �����մϴ١��� ������ ����� �߸�����, ����� ����,�� �� ��
    {
        // �÷��̾� ȭ�鿡 ��������Ʈ�� �ִ� �������� �پ��ϰ� ��ġ �ؼ� �Ѱ��� ������ �׸��� �־ ���̰Բ�
        print("ȯ�� �߻�");
    }
    void Auditory_hallucinations()//��ȯû�� �鸮�� �����մϴ١��� ������ �Ǳ��� �Ҹ��� �̻��� �Ҹ��� ����
    {
        //�̰͵� �ٷ��� ���� ������ ���� �׶� ����
        print("ȯû �߻�");
    }
    void Disarray()//�������� �帮�� �����մϴ١��� ������ �ൿ���� �ʰ�, Ű ������ ��
    {
        //�̰� �غ��� �ѵ�
        print("ȥ�� �߻�");
    }
    void Liver_and_gall_are_cold()//�������� �����ϱ� �����մϴ١��� ����þ߰� �Ϻ��ϰ� �Ⱥ���
    {
        //�̰͵� �þ� �ǵ�� �ű� �ѵ� �غ����� �ϴ�
        print("���� �߻�");
    }
}
