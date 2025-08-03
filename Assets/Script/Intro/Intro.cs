using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [System.Serializable]
    public class ImageTextSet
    {
        public Image image;           
        public string[] texts;        
    }

    [Header("텍스트")]
    public ImageTextSet[] sequence;

    [Space(20)]

    [Header("참조")]
    [Tooltip("텍스트 표시될 UI")]
    public TextMeshProUGUI textUI;

    [Header("속성")]
    // 한 글자 출력 시간
    [Range(0.01f, 0.1f)]
    [SerializeField] float textDelay = 0.05f;

    // 문장이 끝나고 다음으로 넘어가는 시간
    [Range(0.1f, 1f)]
    [SerializeField] float nextTextTime = 0.5f;

    // 이미지 사라지는 속도
    [Range(0.1f, 5f)]
    [SerializeField] float imageFadeTime = 2.0f;


    void Start()
    {
        StartCoroutine(IntroStart());
    }
    // 인트로 시작
    IEnumerator IntroStart()
    {
        foreach (var set in sequence)
        {
            Image img = set.image;
            string[] texts = set.texts;

            img.gameObject.SetActive(true);

            // 알파값 초기화
            Color imgColor = img.color;
            imgColor.a = 1f;
            img.color = imgColor;

            // 텍스트 한 줄씩 출력
            foreach (string t in texts)
            {
                yield return StartCoroutine(TypeText(t));
                yield return new WaitForSeconds(nextTextTime);
            }

            textUI.text = "";

            // 이미지 점점 투명하게
            while (img.color.a > 0f)
            {
                imgColor.a -= Time.deltaTime * imageFadeTime;
                img.color = imgColor;
                yield return null;
            }

            img.gameObject.SetActive(false);
        }
        SceneManager.LoadScene("Stage_Scene"); // 다음 씬으로 이동
    }
    // 한 글자씩 출력
    IEnumerator TypeText(string fullText)
    {
        textUI.text = "";

        foreach (char c in fullText)
        {
            textUI.text += c;
            yield return new WaitForSeconds(textDelay);
        }
    }

    public void SkipIntroButton()
    {
        SceneManager.LoadScene("Stage_Scene"); // 다음 씬으로 이동
    }
}
