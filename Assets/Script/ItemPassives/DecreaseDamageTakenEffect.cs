using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�޴� ���� ����
public class DecreaseDamageTakenEffect : IPassiveEffect
{
    private PlayerData stats;
    private float damageBonus;

    public DecreaseDamageTakenEffect(PlayerData stats, float bonus)
    {
        this.stats = stats;
        this.damageBonus = bonus;
    }

    public void ApplyEffect()
    {
        stats.damageTakenMultiplier -= damageBonus;
    }

    public void RemoveEffect()
    {
        stats.damageTakenMultiplier += damageBonus;
    }
}
