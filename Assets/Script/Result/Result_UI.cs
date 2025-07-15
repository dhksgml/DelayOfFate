using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result_UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] Button nextButton;
    [SerializeField] float fadeTime;

    [SerializeField] bool isFail; // ���н�

    void Awake()
    {
        // �ڷ�ƾ ����
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        bool skip = false;

        for (int i = 0; i < texts.Length; i++)
        {
            float time = 0f;


            // ���콺 Ŭ�� �Ǵ� Space Ű �Է� ���
            while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space)) 
                yield return null;

            // Space Ű�� ������ ��ŵ ���
            if (Input.GetKeyDown(KeyCode.Space))
                skip = true;

            // �ʱ� ���� ����
            Color color = texts[i].color;
            color.a = skip ? 1f : 0f;
            texts[i].color = color;

            // ��ŵ�� ������ �ð��� 0����, �ƴϸ� ���̵� �ð��� �״��
            time = skip ? fadeTime : 0f;

            // ���̵� ��
            while (time < fadeTime)
            {
                // ���� �߿��� Space�� ������ ��� �Ϸ�
                if (!skip && Input.GetKeyDown(KeyCode.Space))
                {
                    skip = true;
                    color.a = 1f;
                    texts[i].color = color;
                    break;
                }

                time += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, time / fadeTime);
                texts[i].color = color;
                yield return null;
            }

            // Ȯ���� ���� 1�� ������
            color.a = 1f;
            texts[i].color = color;
        }

        // ������ ���� ���н�
        if (isFail)
        {
            Debug.Log("���� ����");
            yield break;
        }

        else
        {
            // ��ư ó��
            float buttonTime = 0f;

            // ��ư�� �̹����� ������ ��
            Image buttonImage = nextButton.GetComponent<Image>();

            // �ڽ��� TextMeshPro�� ��������
            // �̺κ��� �̹����� ��ü�Ҳ��� �����ص� ����
            TextMeshProUGUI buttonTextImage = nextButton.GetComponentInChildren<TextMeshProUGUI>();

            // �ʱ� ���� ����
            Color buttonColor = buttonImage.color;
            Color buttonTextColor = buttonTextImage.color;

            // ��ư �̹�����, �ý�Ʈ�� ���İ��� skip �����̸� �ٷ� Ȱ��ȭ, �ƴϸ� 0���� ���ش�.
            buttonColor.a = skip ? 1f : 0f;
            buttonTextColor.a = skip ? 1f : 0f;

            buttonImage.color = buttonColor;
            buttonTextImage.color = buttonTextColor;

            // ���̵� ��
            while (buttonTime < fadeTime)
            {
                // ���� �߿��� Space�� ������ ��� �Ϸ�
                if (!skip && Input.GetKeyDown(KeyCode.Space))
                {
                    buttonColor.a = 1f;
                    buttonTextColor.a = 1f;

                    buttonImage.color = buttonColor;
                    buttonTextImage.color = buttonTextColor;
                    break;
                }

                buttonTime += Time.deltaTime;

                buttonColor.a = Mathf.Lerp(0f, 1f, buttonTime / fadeTime);
                buttonTextColor.a = Mathf.Lerp(0f, 1f, buttonTime / fadeTime);

                buttonImage.color = buttonColor;
                buttonTextImage.color = buttonTextColor;

                yield return null;
            }
        }

    }
}
