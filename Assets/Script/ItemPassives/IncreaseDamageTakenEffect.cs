using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//받는 피해 증가
public class IncreaseDamageTakenEffect : IPassiveEffect
{
    private PlayerData stats;
    private float damageBonus;

    public IncreaseDamageTakenEffect(PlayerData stats, float bonus)
    {
        this.stats = stats;
        this.damageBonus = bonus;
    }

    public void ApplyEffect()
    {
        stats.damageTakenMultiplier += damageBonus;
    }

    public void RemoveEffect()
    {
        stats.damageTakenMultiplier -= damageBonus;
    }
}
