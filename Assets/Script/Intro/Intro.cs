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

    [Header("�ؽ�Ʈ")]
    public ImageTextSet[] sequence;

    [Space(20)]

    [Header("����")]
    [Tooltip("�ؽ�Ʈ ǥ�õ� UI")]
    public TextMeshProUGUI textUI;

    [Header("�Ӽ�")]
    // �� ���� ��� �ð�
    [Range(0.01f, 0.1f)]
    [SerializeField] float textDelay = 0.05f;

    // ������ ������ �������� �Ѿ�� �ð�
    [Range(0.1f, 1f)]
    [SerializeField] float nextTextTime = 0.5f;

    // �̹��� ������� �ӵ�
    [Range(0.1f, 5f)]
    [SerializeField] float imageFadeTime = 2.0f;


    void Start()
    {
        StartCoroutine(IntroStart());
    }
    // ��Ʈ�� ����
    IEnumerator IntroStart()
    {
        foreach (var set in sequence)
        {
            Image img = set.image;
            string[] texts = set.texts;

            img.gameObject.SetActive(true);

            // ���İ� �ʱ�ȭ
            Color imgColor = img.color;
            imgColor.a = 1f;
            img.color = imgColor;

            // �ؽ�Ʈ �� �پ� ���
            foreach (string t in texts)
            {
                yield return StartCoroutine(TypeText(t));
                yield return new WaitForSeconds(nextTextTime);
            }

            textUI.text = "";

            // �̹��� ���� �����ϰ�
            while (img.color.a > 0f)
            {
                imgColor.a -= Time.deltaTime * imageFadeTime;
                img.color = imgColor;
                yield return null;
            }

            img.gameObject.SetActive(false);
        }
        SceneManager.LoadScene("Stage_Scene"); // ���� ������ �̵�
    }
    // �� ���ھ� ���
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
        SceneManager.LoadScene("Stage_Scene"); // ���� ������ �̵�
    }
}
