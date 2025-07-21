using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//최대체력 감소
public class IncreaseMaxHPEffect : IPassiveEffect
{
    private PlayerData stats;
    private float bonusMaxHP;

    public IncreaseMaxHPEffect(PlayerData stats, float bonus)
    {
        this.stats = stats;
        this.bonusMaxHP = bonus;
    }

    public void ApplyEffect()
    {
        stats.extraHp += bonusMaxHP;
    }

    public void RemoveEffect()
    {
        stats.extraHp -= bonusMaxHP;
    }
}
