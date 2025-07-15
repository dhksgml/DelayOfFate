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

    [SerializeField] bool isFail; // 실패시

    void Awake()
    {
        // 코루틴 실행
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        bool skip = false;

        for (int i = 0; i < texts.Length; i++)
        {
            float time = 0f;


            // 마우스 클릭 또는 Space 키 입력 대기
            while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space)) 
                yield return null;

            // Space 키를 누르면 스킵 모드
            if (Input.GetKeyDown(KeyCode.Space))
                skip = true;

            // 초기 색깔 세팅
            Color color = texts[i].color;
            color.a = skip ? 1f : 0f;
            texts[i].color = color;

            // 스킵을 했으면 시간을 0으로, 아니면 페이드 시간을 그대로
            time = skip ? fadeTime : 0f;

            // 페이드 인
            while (time < fadeTime)
            {
                // 진행 중에도 Space가 눌리면 즉시 완료
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

            // 확실히 알파 1로 마무리
            color.a = 1f;
            texts[i].color = color;
        }

        // 생존비 납부 실패시
        if (isFail)
        {
            Debug.Log("납부 실패");
            yield break;
        }

        else
        {
            // 버튼 처리
            float buttonTime = 0f;

            // 버튼의 이미지를 가져와 줌
            Image buttonImage = nextButton.GetComponent<Image>();

            // 자식의 TextMeshPro도 가져와줌
            // 이부분은 이미지로 대체할꺼면 제거해도 무방
            TextMeshProUGUI buttonTextImage = nextButton.GetComponentInChildren<TextMeshProUGUI>();

            // 초기 색깔 세팅
            Color buttonColor = buttonImage.color;
            Color buttonTextColor = buttonTextImage.color;

            // 버튼 이미지와, 택스트의 알파값이 skip 상태이면 바로 활성화, 아니면 0으로 해준다.
            buttonColor.a = skip ? 1f : 0f;
            buttonTextColor.a = skip ? 1f : 0f;

            buttonImage.color = buttonColor;
            buttonTextImage.color = buttonTextColor;

            // 페이드 인
            while (buttonTime < fadeTime)
            {
                // 진행 중에도 Space가 눌리면 즉시 완료
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
