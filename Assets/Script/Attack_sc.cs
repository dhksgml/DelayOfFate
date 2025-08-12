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

    private Player_Item_Use player_Item_Use;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        player_Item_Use = FindObjectOfType<Player_Item_Use>();
        // Sword 공격이면 랜덤 회전
        if (attackType == AttackType.Sword && effectRenderer != null)
        {
            float randomRotation = Random.Range(0f, 360f);
            effectRenderer.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
        }

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
            case AttackType.Bottle: return Mathf.FloorToInt(444);
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
                    if (attackType == AttackType.Scroll)
                    {
                        damage = enemy.enemyMaxHp / 2;
                    }
                    else if (attackType == AttackType.Bottle)
                    {
                        if (enemy.enemyHeight == 21)
                        {
                            damage = 0;
                        }
                        else
                        {
                            if (attackType.ToString() != enemy.enemyWeakness.ToString())//약점이 아니라면
                            {
                                player_Item_Use.quickSlots[player_Item_Use.selectedSlotIndex].Count--; //공격이 적중했다면 개수 감소

                                // 곗수가 0이 되면 슬롯 비우기
                                if (player_Item_Use.quickSlots[player_Item_Use.selectedSlotIndex].Count <= 0)
                                {
                                    player_Item_Use.quickSlots[player_Item_Use.selectedSlotIndex] = null;
                                }
                            }

                        }
                    }
                }
            }
        }
    }
    protected virtual void TriggerWeaknessEffect()
    {
        // 확장용
        //if (attackType == AttackType.Bottle) { }
    }
}
