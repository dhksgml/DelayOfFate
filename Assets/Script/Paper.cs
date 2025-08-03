using UnityEngine;

public class Paper : MonoBehaviour
{
	private float moveSpeed = 12f;
	private float moveDuration = 0.6f;
	private float vanishDelay;
	public SpriteRenderer spriteRenderer;
	public GameObject Attack_Paper;

	private Rigidbody2D rb;
	private bool isMoving = true;
	private float moveTimer;
	private float vanishTimer;
	private bool isVanishing = false;

	public float dashSpeed = 12f;
	public float detectRadius = 5f;

	private Transform targetEnemy = null;

	private void Awake()
	{
		moveSpeed += Random.Range(-1.5f, +1.5f);
		moveDuration += Random.Range(-0.15f, +0.16f);
		vanishDelay = Random.Range(55, 61);
		rb = GetComponent<Rigidbody2D>();
		moveTimer = moveDuration;
	}

	private void Update()
	{
		if (isMoving)
		{
			moveTimer -= Time.deltaTime;
			if (moveTimer <= 0f)
			{
				StopMovement();
			}
		}
		else if (!isVanishing)
		{
			vanishTimer += Time.deltaTime;
			if (vanishTimer >= vanishDelay)
			{
				StartCoroutine(FadeAndDestroy());
				isVanishing = true;
			}
		}
	}

	private void FixedUpdate()
	{
		if (isMoving)
		{
			rb.velocity = transform.right * moveSpeed;
		}
		else if (targetEnemy != null)
		{
			Vector2 dashDir = (targetEnemy.position - transform.position).normalized;
			rb.velocity = dashDir * dashSpeed;
		}
		else
		{
			rb.velocity = Vector2.zero;
			FindClosestEnemy(); // 유도 가능한 적이 있는지 탐지
		}
	}

	private void StopMovement()
	{
		isMoving = false;
		rb.velocity = Vector2.zero;
		vanishTimer = 0f;
	}

	private void FindClosestEnemy()
	{
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);
		float closestDist = Mathf.Infinity;
		Transform closest = null;

		foreach (var hit in hits)
		{
			if (hit.CompareTag("Enemy"))
			{
				float dist = (hit.transform.position - transform.position).sqrMagnitude;
				if (dist < closestDist)
				{
					closestDist = dist;
					closest = hit.transform;
				}
			}
		}

		if (closest != null)
		{
			targetEnemy = closest;
		}
	}

	private System.Collections.IEnumerator FadeAndDestroy()
	{
		float fadeDuration = 0.5f;
		float timer = 0f;
		Color originalColor = spriteRenderer.color;

		while (timer < fadeDuration)
		{
			float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
			spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
			timer += Time.deltaTime;
			yield return null;
		}

		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy"))
		{
			Attack();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Wall") && isMoving)
		{
			StopMovement();
		}
	}

	void Attack()
	{
		GameObject go = Instantiate(Attack_Paper, transform.position, Quaternion.identity);
		Attack_sc attackEffect = go.GetComponent<Attack_sc>();
		attackEffect.attackType = Attack_sc.AttackType.Paper;
		print(attackEffect.attackType);
		Destroy(gameObject);
	}
}
