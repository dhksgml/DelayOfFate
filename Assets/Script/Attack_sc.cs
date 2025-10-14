using UnityEngine;

public class Attack_sc : MonoBehaviour
{
    public enum AttackType
    {
        Sword,
        Bat,
        Paper,
        Scroll,
        Bottle
    }

    [Header("공격 타입")]
    [HideInInspector]
    public AttackType attackType;

    [Header("이펙트 설정")]
    public SpriteRenderer effectRenderer;
    public float fadeOutTime = 0.25f;

    public float damage;
    private Animator animator;
    private Collider2D collider2D;

    private Player_Item_Use player_Item_Use;
    private Player_Item_p player_Item_P;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
    }

    private void Start()
    {
        player_Item_Use = FindObjectOfType<Player_Item_Use>();
        player_Item_P = FindObjectOfType<Player_Item_p>();

        // Sword 타입이면 랜덤 회전
        if (attackType == AttackType.Sword && effectRenderer != null)
        {
            float randomRotation = Random.Range(0f, 360f);
            effectRenderer.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
        }
        if (attackType == AttackType.Bottle)
        {
            collider2D.offset += new Vector2(0.5f, 0f);
        }

        Invoke(nameof(StartFadeOut), 0.1f);
        PlayAnimation();

        // 데미지 계산
        damage = GetDamageByType(attackType);
    }

    private void PlayAnimation()
    {
        if (animator == null) return;

        string stateName = GetStateNameByType(attackType);
        if (!string.IsNullOrEmpty(stateName)) animator.Play(stateName);
    }

    private string GetStateNameByType(AttackType type)
    {
        switch (type)
        {
            case AttackType.Sword: return "SwordAttack";
            case AttackType.Bat: return "BatAttack";
            case AttackType.Paper: return "PaperAttack";
            case AttackType.Scroll: return "ScrollAttack";
            case AttackType.Bottle: return "BottleAttack";
            default: return null;
        }
    }

    private void StartFadeOut()
    {
        StartCoroutine(FadeAndDestroy());
    }

    private System.Collections.IEnumerator FadeAndDestroy()
    {
        float timer = 0f;
        Color originalColor = effectRenderer.color;

        while (timer < fadeOutTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            effectRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // 피해량 '할' 계산 메서드
    private float CalculateDamageMultiplier()
    {
        float damageBonus = 0f;

        // GameManager 공격력 배율 (기본 배율)
        if (GameManager.Instance != null && GameManager.Instance.playerData != null)
        {
            damageBonus += GameManager.Instance.playerData.damageMultiplier - 1f; // 1f는 기본값이므로 보너스만 추출
        }

        if (player_Item_P != null && player_Item_P.item_p_count != null)
        {
            // 12번 아이템: 2할 증가 (중첩 가능)
            if (player_Item_P.item_p[12])
            {
                damageBonus += 0.2f * player_Item_P.item_p_count[12];
            }

            // 14번 아이템: 2할 증가 (중첩 가능)
            if (player_Item_P.item_p[14])
            {
                damageBonus += 0.2f * player_Item_P.item_p_count[14];
            }
        }

        return 1.0f + damageBonus; // 기본 100% + 보너스
    }

    private float GetDamageByType(AttackType type)
    {
        // Scroll과 Bottle은 고정 피해 (할 계산 적용 안함)
        if (type == AttackType.Scroll)
        {
            return 0f; // OnTriggerEnter2D에서 계산됨
        }

        if (type == AttackType.Bottle)
        {
            return Mathf.FloorToInt(100); // 고정 피해
        }

        // 기본 피해량 계산
        int baseDamage = 0;
        int itemBonus = 0; // + 증가량

        switch (type)
        {
            case AttackType.Sword:
                {
                    if (player_Item_P != null && player_Item_P.item_p[8] && player_Item_P.item_p_count != null)
                    {
                        itemBonus += 3 * player_Item_P.item_p_count[8]; // +2 × 개수
                    }
                    baseDamage = Mathf.FloorToInt(Random.Range(10f, 14f + 1));
                    break;
                }
            case AttackType.Bat:
                {
                    if (player_Item_P != null && player_Item_P.item_p[15] && player_Item_P.item_p_count != null)
                    {
                        itemBonus += 4 * player_Item_P.item_p_count[15]; // +3 × 개수
                    }
                    baseDamage = Mathf.FloorToInt(Random.Range(20f, 30f + 1));
                    break;
                }
            case AttackType.Paper:
                {
                    if (player_Item_P != null && player_Item_P.item_p[9] && player_Item_P.item_p_count != null)
                    {
                        itemBonus += 2 * player_Item_P.item_p_count[9]; // +5 × 개수
                    }
                    baseDamage = Mathf.FloorToInt(Random.Range(6f, 8f + 1));
                    break;
                }
            default:
                return 0f;
        }
        // 1단계: + 증가량 적용
        float totalDamage = baseDamage + itemBonus;

        // 2단계: '할' 배율 적용
        float damageMultiplier = CalculateDamageMultiplier();
        totalDamage *= damageMultiplier;

        print($"{type} 최종 피해: 기본={baseDamage}, 보너스={itemBonus}, 배율={damageMultiplier:F2}, 최종={Mathf.FloorToInt(totalDamage)}");

        return Mathf.FloorToInt(totalDamage); // 소수점 버림
    }

    public void CheckWeakness()
    {
        if (CheckWeaknessPassive()) return;
        effectRenderer.color = Color.red;
        TriggerWeaknessEffect();
    }

    public bool CheckWeaknessPassive()
    {
        return PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_1_2");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy == null)
            {
                enemy = collision.GetComponentInParent<Enemy>();
            }
            if (enemy != null && enemy.gameObject.CompareTag("Enemy"))
            {
                if (enemy.gameObject.CompareTag("Enemy"))
                {
                    // Scroll은 적 체력의 절반 (고정 피해, 할 적용 안함)
                    if (attackType == AttackType.Scroll)
                    {
                        damage = enemy.enemyMaxHp / 2;
                    }
                }
            }
        }
    }

    protected virtual void TriggerWeaknessEffect()
    {
        // 확장용
    }
}