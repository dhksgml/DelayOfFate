using UnityEngine;

public class Paper : MonoBehaviour
{
    private float moveSpeed = 12f; // 속력
    private float moveDuration = 0.6f; // 이동 후 덫 변화시간
    private float vanishDelay; // 덫 지속시간
    public SpriteRenderer spriteRenderer;
    public GameObject Attack_Paper;

    private Rigidbody2D rb;
    private bool isMoving = true;
    private float moveTimer;
    private float vanishTimer;
    private bool isVanishing = false;

    public float detectionRadius = 2f;
    public float dashSpeed = 12f;

    private Transform targetEnemy = null;
    private bool isDashing = false;


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
        else if (!isVanishing && !isDashing)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    targetEnemy = hit.transform;
                    isDashing = true;
                    break;
                }
            }

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
        else if (isDashing && targetEnemy != null)
        {
            Vector2 dashDir = (targetEnemy.position - transform.position).normalized;
            rb.velocity = dashDir * dashSpeed;
        }
    }
    private void StopMovement()
    {
        isMoving = false;
        rb.velocity = Vector2.zero;
        vanishTimer = 0f;
    }
    private System.Collections.IEnumerator FadeAndDestroy()
    {
        float fadeDuration = 0.5f;// 덫 변화 후 감지 거리
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
    void Attack()
    {
        GameObject go = Instantiate(Attack_Paper, gameObject.transform.position, Quaternion.identity);
        Attack_sc attackEffect = go.GetComponent<Attack_sc>();
        attackEffect.attackType = Attack_sc.AttackType.Paper;
        print(attackEffect.attackType);
        Destroy(gameObject);
    }
}
