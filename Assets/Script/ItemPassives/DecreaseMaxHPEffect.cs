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
        stats.extraHp -= bonusMaxHP;
    }

    public void RemoveEffect()
    {
        stats.extraHp += bonusMaxHP;
    }
}
