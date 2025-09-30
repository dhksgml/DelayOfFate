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

    [Header("���� ����")]
    [HideInInspector]
    public AttackType attackType;

    [Header("����Ʈ ����")]
    public SpriteRenderer effectRenderer;
    public float fadeOutTime = 0.25f;

    public float damage;
    private Animator animator;

    private Player_Item_Use player_Item_Use;
    private Player_Item_p player_Item_P;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        player_Item_Use = FindObjectOfType<Player_Item_Use>();
        player_Item_P = FindObjectOfType<Player_Item_p>();
        // Sword �����̸� ���� ȸ��
        if (attackType == AttackType.Sword && effectRenderer != null)
        {
            float randomRotation = Random.Range(0f, 360f);
            effectRenderer.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
        }

        Invoke(nameof(StartFadeOut), 0.1f);
        PlayAnimation();

        float damageMultiplier = 1f;
        if (GameManager.Instance != null && GameManager.Instance.playerData != null)
            damageMultiplier = GameManager.Instance.playerData.damageMultiplier;

        damage = GetDamageByType(attackType) * damageMultiplier;
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
        int itemadd = 0;
        
        switch (type)
        {
            case AttackType.Sword: { if (player_Item_P.item_p[8]) { itemadd += 2; } itemadd += Mathf.FloorToInt(Random.Range(10f, 14f + 1)); break; }
            case AttackType.Bat:{ if (player_Item_P.item_p[15]) { itemadd += 3; } itemadd += Mathf.FloorToInt(Random.Range(20f, 30f + 1)); break; }
            case AttackType.Paper:{ if (player_Item_P.item_p[9]) { itemadd += 5; } itemadd += Mathf.FloorToInt(Random.Range(10f, 12f + 1)); break; }
            case AttackType.Bottle:{ return Mathf.FloorToInt(444);}
            default: return 0f;
        }
        float finalDamage = itemadd;
        return finalDamage;
    }

    public void CheckWeakness()
    {
        //������� ���� ��
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
                            if (attackType.ToString() != enemy.enemyWeakness.ToString())//������ �ƴ϶��
                            {
                                print(player_Item_Use.weaponSlots);
                                print(player_Item_Use.weaponSlots[player_Item_Use.selectedWeaponIndex]);
                                print(player_Item_Use.weaponSlots[player_Item_Use.selectedWeaponIndex].Count);
                                player_Item_Use.weaponSlots[player_Item_Use.selectedWeaponIndex].Count--; //������ �����ߴٸ� ���� ����

                                // ����� 0�� �Ǹ� ���� ����
                                if (player_Item_Use.weaponSlots[player_Item_Use.selectedWeaponIndex].Count <= 0)
                                {
                                    player_Item_Use.weaponSlots[player_Item_Use.selectedWeaponIndex] = null;
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
        // Ȯ���
        //if (attackType == AttackType.Bottle) { }
    }
}
