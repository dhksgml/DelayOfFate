using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//최대SP 감소
public class DecreaseMaxSPEffect : IPassiveEffect
{
    private PlayerData stats;
    private float bonusMaxSP;

    public DecreaseMaxSPEffect(PlayerData stats, float bonus)
    {
        this.stats = stats;
        this.bonusMaxSP = bonus;
    }

    public void ApplyEffect()
    {
        stats.maxSp -= bonusMaxSP;
        stats.currentSp = stats.maxSp;
    }

    public void RemoveEffect()
    {
        stats.maxSp += bonusMaxSP;
        stats.currentSp = stats.maxSp;
    }
}
