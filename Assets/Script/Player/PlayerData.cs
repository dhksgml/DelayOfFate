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

    public int flashLightLevel = 1; //������
    public float gold = 0; //��
    public float soul = 0; //��ȥ
    public Item[] quickSlots = new Item[4]; //�÷��̾ �Ĺ��� ������
    //public Item[] soulItems = new Item[14]; //��ȥ���� ������ ��� ��
}
