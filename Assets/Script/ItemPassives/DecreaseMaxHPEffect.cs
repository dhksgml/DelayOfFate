using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//최대체력 증가
public class DecreaseMaxHPEffect : IPassiveEffect
{
    private PlayerData stats;
    private float bonusMaxHP;

    public DecreaseMaxHPEffect(PlayerData stats, float bonus)
    {
        this.stats = stats;
        this.bonusMaxHP = bonus;
    }

    public void ApplyEffect()
    {
        float hpToExtra = Mathf.Max(stats.extraHp, bonusMaxHP);
        float value = hpToExtra;

        if (stats.extraHp > 0)
        {
            stats.extraHp = Mathf.Clamp(stats.extraHp, 0, hpToExtra);
            value = hpToExtra;
        }

        if (value > 0)
        {
            stats.maxHp = Mathf.Max(0, stats.maxHp - value);
        }

        stats.currentHp = stats.maxHp;
        stats.currentExtraHp = stats.extraHp;
    }

    public void RemoveEffect()
    {
        float fromMaxHp = Mathf.Max(0, stats.maxHp - 100f);
        float fromExtraHp = bonusMaxHP - fromMaxHp;

        stats.maxHp -= fromMaxHp;
        stats.extraHp -= fromExtraHp;

        stats.currentHp = Mathf.Min(stats.currentHp, stats.maxHp);
        stats.currentExtraHp = Mathf.Min(stats.currentExtraHp, stats.extraHp);
    }
}
