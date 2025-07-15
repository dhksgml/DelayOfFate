using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//최대SP 증가
public class IncreaseMaxSPEffect : IPassiveEffect
{
    private PlayerData stats;
    private float bonusMaxSP;

    public IncreaseMaxSPEffect(PlayerData stats, float bonus)
    {
        this.stats = stats;
        this.bonusMaxSP = bonus;
    }

    public void ApplyEffect()
    {
        stats.maxSp += bonusMaxSP;
    }

    public void RemoveEffect()
    {
        stats.maxSp -= bonusMaxSP;
    }
}
