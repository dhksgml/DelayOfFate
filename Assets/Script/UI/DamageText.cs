using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class DamageText : MonoBehaviour
{
	public TMP_Text text;
	public float forceX = 2f;
	public float forceY = 3f;
	public float fadeDelay = 0.5f;
	public float fadeDuration = 0.5f;

	private Rigidbody2D rb;
	private CanvasGroup canvasGroup;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		// CanvasGroup은 알파 조절용
		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}
	}

	public void Init(string damageStr, Color color, float maxhp, float cuhp)
	{
		text.text = damageStr;
		// 체력 비율 계산 (0이면 빈 피, 1이면 풀피)
		float hpRatio = Mathf.Clamp01(cuhp / maxhp);

		// 색상 계산: hpRatio가 낮을수록 더 붉은 색
		Color damageColor = Color.Lerp(Color.red, Color.white, hpRatio);

		text.color = damageColor;

		// 좌우 랜덤 방향
		float randomX = Random.Range(-forceX, forceX);
		float randomY = Random.Range(forceY * 0.5f, forceY * 1.5f);
		rb.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);

		StartCoroutine(FadeOut());
	}

	private System.Collections.IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(fadeDelay);

		float t = 0f;
		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
			yield return null;
		}

		Destroy(gameObject);
	}
}
