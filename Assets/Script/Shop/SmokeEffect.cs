using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmokeEffect : MonoBehaviour
{
	public Sprite[] sprite;
	private Image image;
	private float targetAlpha;
	private float alphaDuration;
	private float alphaElapsed;

	private Vector3 startScale = Vector3.one * 0.1f;
	private Vector3 endScale = Vector3.one * 1.5f;

	private float moveSpeedY;
	private float moveSpeedX;

	private float rotationSpeed; // 도/초

	private void Awake()
	{
		image = GetComponent<Image>();
		image.sprite = sprite[Random.Range(0, sprite.Length)];
	}

	private void Start()
	{
		// 초기 스케일
		transform.localScale = startScale;

		// 알파 설정
		Color c = image.color;
		c.a = 0.2f;
		image.color = c;

		targetAlpha = Random.Range(0.3f, 0.4f);
		alphaDuration = Random.Range(10f, 12f);
		alphaElapsed = 0f;

		// 이동 속도
		moveSpeedY = Random.Range(48f, 52f);
		moveSpeedX = Random.Range(-10f, 10f);

		// 회전 속도 (초당 도 단위)
		float rotateTime = Random.Range(5f, 7f);
		float direction = Random.value < 0.5f ? 1f : -1f;
		rotationSpeed = 360f / rotateTime * direction;

		// 12초 후 파괴
		Destroy(gameObject, 15f);

		// 스케일 커지기 코루틴 시작
		StartCoroutine(ScaleUp());
	}

	private void Update()
	{
		// 알파 보간
		if (alphaElapsed < alphaDuration)
		{
			alphaElapsed += Time.deltaTime;
			float t = Mathf.Clamp01(alphaElapsed / alphaDuration);
			Color c = image.color;
			c.a = Mathf.Lerp(0.2f, targetAlpha, t);
			image.color = c;
		}

		// 위치 이동
		transform.position += new Vector3(moveSpeedX, moveSpeedY, 0f) * Time.deltaTime;

		// 회전
		transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
	}

	private IEnumerator ScaleUp()
	{
		float duration = 5f;
		float elapsed = 0f;
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / duration);
			transform.localScale = Vector3.Lerp(startScale, endScale, t);
			yield return null;
		}
	}
}
