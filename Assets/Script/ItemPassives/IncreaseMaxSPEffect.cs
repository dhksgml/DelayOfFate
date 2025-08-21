using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ִ�SP ����
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
    }

    public void RemoveEffect()
    {
    }
}
