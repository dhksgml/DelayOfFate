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
        stats.maxHp += bonusMaxHP;
        stats.currentHp = stats.maxHp;
    }

    public void RemoveEffect()
    {
        stats.maxHp -= bonusMaxHP;
        stats.currentHp = stats.maxHp;
    }
}
