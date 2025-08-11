using UnityEngine;

public class Effect_sc : MonoBehaviour
{
	public float fadeTime = 0.5f; // 사라지는 시간 (기본값 지정)
	public string ty; // 타입 (hp, sp, e_at 등)
	public Sprite[] sprite;
	private SpriteRenderer spriteRenderer;
	private Color originalColor;

	private Animator animator;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
		{
			Debug.LogWarning("SpriteRenderer가 없습니다.");
			return;
		}

		originalColor = spriteRenderer.color;
		animator = GetComponent<Animator>();

		// Animator가 있으면 기본적으로 비활성화
		if (animator != null)
			animator.enabled = false;
		StartCoroutine(FadeAndDestroy());
		if (ty == "e_at")
		{
			if (animator == null)
			{
				Debug.LogWarning("Animator가 없습니다. e_at 타입에는 Animator가 필요합니다.");
				Destroy(gameObject);
				return;
			}
			// 애니메이터 활성화 및 애니메이션 재생
			animator.enabled = true;
			animator.Play(ty);
			StartCoroutine(WaitForAnimationAndDestroy());
		}
		else if (ty == "hp" || ty == "sp")
		{
			if (ty == "hp")
				spriteRenderer.sprite = sprite[0];
			else if (ty == "sp")
				spriteRenderer.sprite = sprite[1];
		}
	}


	private void Update()
	{
		// hp일 때만 위로 올라감
		if (ty == "hp" || ty == "sp")
		{
			upward();
		}
	}

	private void upward()
	{
		transform.position += Vector3.up * Random.Range(5, 11) * Time.deltaTime;
	}

	private System.Collections.IEnumerator FadeAndDestroy()
	{
		float timer = 0f;
		while (timer < fadeTime)
		{
			float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
			spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
			timer += Time.deltaTime;
			yield return null;
		}

		Destroy(gameObject);
	}

	private System.Collections.IEnumerator WaitForAnimationAndDestroy()
	{
		AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
		float animationLength = state.length;

		yield return new WaitForSeconds(animationLength);

		Destroy(gameObject);
	}
}
