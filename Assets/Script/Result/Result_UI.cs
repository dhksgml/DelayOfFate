using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result_UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] Button nextButton;
    [SerializeField] float fadeTime;

    [SerializeField] RectTransform stampImage; // 도장 이미지
    [SerializeField] bool isFail;

    void Awake()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        bool skip = false;

        for (int i = 0; i < texts.Length; i++)
        {
            float time = 0f;

            while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
                yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
                skip = true;

            Color color = texts[i].color;
            color.a = skip ? 1f : 0f;
            texts[i].color = color;

            time = skip ? fadeTime : 0f;

            while (time < fadeTime)
            {
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

            color.a = 1f;
            texts[i].color = color;
        }

        if (isFail)
        {
            Debug.Log("납부 실패");
            yield break;
        }
        else
        {
            // 도장 애니메이션 먼저 실행
            StartCoroutine(PlayStampAnimation());

            float buttonTime = 0f;
            Image buttonImage = nextButton.GetComponent<Image>();
            TextMeshProUGUI buttonTextImage = nextButton.GetComponentInChildren<TextMeshProUGUI>();

            Color buttonColor = buttonImage.color;
            Color buttonTextColor = buttonTextImage.color;

            buttonColor.a = skip ? 1f : 0f;
            buttonTextColor.a = skip ? 1f : 0f;

            buttonImage.color = buttonColor;
            buttonTextImage.color = buttonTextColor;

            while (buttonTime < fadeTime)
            {
                if (!skip && Input.GetKeyDown(KeyCode.Space))
                {
                    buttonColor.a = 1f;
                    buttonTextColor.a = 1f;
                    buttonImage.color = buttonColor;
                    buttonTextImage.color = buttonTextColor;
                    break;
                }

                buttonTime += Time.deltaTime;
                float t = buttonTime / fadeTime;

                buttonColor.a = Mathf.Lerp(0f, 1f, t);
                buttonTextColor.a = Mathf.Lerp(0f, 1f, t);

                buttonImage.color = buttonColor;
                buttonTextImage.color = buttonTextColor;

                yield return null;
            }
        }
    }

    IEnumerator PlayStampAnimation()
    {
        // 도장 이미지 활성화
        stampImage.gameObject.SetActive(true);
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/sfx_stamp"));
        // 초기 설정: 2배 크기 + 랜덤 회전
        stampImage.localScale = Vector3.one * 2f;
        float randomZ = Random.Range(-30f, 30f);
        stampImage.localEulerAngles = new Vector3(0f, 0f, randomZ);

        float duration = 0.1f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            stampImage.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t);
            yield return null;
        }

        // 최종 크기 고정
        stampImage.localScale = Vector3.one;
    }
}
