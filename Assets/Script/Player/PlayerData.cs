using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float maxHp = 100;
    public float maxMp = 100;
    public float maxSp = 100;

    public float currentHp = 100;
    public float currentMp = 100;
    public float currentSp = 100;

    public int flashLightLevel = 1; //조명레벨
    public float gold = 0; //돈
    public float soul = 0; //영혼
    public Item[] quickSlots = new Item[4]; //플레이어가 파밍한 아이템
    //public Item[] soulItems = new Item[14]; //영혼으로 아이템 사는 거
}
