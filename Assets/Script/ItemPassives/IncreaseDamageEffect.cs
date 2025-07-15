using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�޴� ���� ����
public class IncreaseDamageEffect : IPassiveEffect
{
    private PlayerData stats;
    private float damageBonus;

    public IncreaseDamageEffect(PlayerData stats, float bonus)
    {
        this.stats = stats;
        this.damageBonus = bonus;
    }

    public void ApplyEffect()
    {
        stats.damageMultiplier += damageBonus;
    }

    public void RemoveEffect()
    {
        stats.damageMultiplier -= damageBonus;
    }
}
