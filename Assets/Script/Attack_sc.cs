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

    [Header("공격 설정")]
    [HideInInspector]
    public AttackType attackType;

    [Header("이펙트 설정")]
    public SpriteRenderer effectRenderer;
    public float fadeOutTime = 0.25f;

    public float damage;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Invoke(nameof(StartFadeOut), 0.1f);
        PlayAnimation();
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

    private float GetDamageByType(AttackType type)
    {
        switch (type)
        {
            case AttackType.Sword: return Mathf.FloorToInt(Random.Range(10f, 14f+1));
            case AttackType.Bat: return Mathf.FloorToInt(Random.Range(20f, 30f+1));
            case AttackType.Paper: return Mathf.FloorToInt(Random.Range(10f, 12f+1));
            default: return 0f;
        }
    }

    public void CheckWeakness()
    {
        //정정당당 보유 시
        if (PassiveItemManager.Instance != null && PassiveItemManager.Instance.HasEffect("Soul_Add_1_2")) return;
        effectRenderer.color = Color.red;
        TriggerWeaknessEffect();
    }

    protected virtual void TriggerWeaknessEffect()
    {
        // 확장용
    }
}
