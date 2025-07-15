using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMoveSpeedEffect : IPassiveEffect
{
    private PlayerData movement;
    private float speedBonus;

    public IncreaseMoveSpeedEffect(PlayerData movement, float bonus)
    {
        this.movement = movement;
        this.speedBonus = bonus;
    }

    public void ApplyEffect()
    {
        movement.speedMultiplier += speedBonus;
    }

    public void RemoveEffect()
    {
        movement.speedMultiplier -= speedBonus;
    }
}
