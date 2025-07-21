using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//최대체력 증가
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
    }

    public void RemoveEffect()
    {
        stats.maxHp -= bonusMaxHP;
    }
}
