using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ִ�ü�� ����
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
        stats.maxHp -= bonusMaxHP;
    }

    public void RemoveEffect()
    {
        stats.maxHp += bonusMaxHP;
    }
}
