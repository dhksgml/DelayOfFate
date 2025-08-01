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

    public float currentExtraHp;
    public float extraHp;

    public int flashLightLevel = 1; //조명레벨
    public float gold = 0; //돈
    public float soul = 0; //영혼
    public Item[] quickSlots = new Item[4]; //플레이어가 파밍한 아이템
    //public Item[] soulItems = new Item[14]; //영혼으로 아이템 사는 거

    public float damageTakenMultiplier = 1.0f; //받는 데미지 가중치
    public float damageMultiplier = 1.0f; //주는 데미지 가중치
    public float speedMultiplier = 1.0f; //이동속도 가중치

    public bool isDropWhenRevive = false; //구사일생 관련
    public bool isFindNearestItem = false; //가까운 아이템 화살표 표시

    public void Init()
    {
        currentExtraHp = extraHp;

        currentHp = maxHp;
        currentMp = maxMp;
        currentSp = maxSp;
    }
}
