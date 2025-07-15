using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData playerData = new PlayerData();

    //게임 관련 데이터
    public int Day = 1;
    public float Gold = 0;
    public float Soul = 0;
    public float N_Day_Add_Soul; //당일에 번 총 소울
    public int N_Day_Time; // 당일에 클리어한 각(시간)
    public float N_Day_Cost; //해당 정산일에 낼 돈
    public Item[] currentQuickSlot = new Item[4];
    

    private void Awake()
    {
        // 현재 씬에 자신과 같은 타입의 오브젝트가 2개 이상 있는 경우 즉시 삭제
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void New_Day_date()
    {
        N_Day_Add_Soul = 0;
        N_Day_Time = 0;
    }
    public void Add_Gold(float val)
    {
        Gold += val;
    }
    public void Sub_Gold(float val)//감소
    {
        Gold -= val;
    }
    public void Add_Soul(float val)
    {
        Soul += val;
        N_Day_Add_Soul += val;
    }
    public void Sub_Soul(float val)//감소
    {
        Soul -= val;
    }
    public void Next_Day()
    {
        Day++;
    }

    public void SavePlayerInfo(PlayerController player)
    {
        playerData.maxHp = player.maxHp;
        playerData.maxMp = player.maxMp;
        playerData.maxSp= player.maxSp;

        playerData.currentHp = player.currentHp;
        playerData.currentMp = player.currentMp;
        playerData.currentSp = player.currentSp;

        playerData.flashLightLevel = player.flashLightLevel;
        playerData.gold = this.Gold;
        playerData.soul = this.Soul;
        playerData.quickSlots = this.currentQuickSlot;

    }

    public void SaveCurrentQuickSlot(Item[] quickSlots)
    {
        if (quickSlots.Length != 4) return;

        for(int i = 0; i < currentQuickSlot.Length; i++)
        {
            currentQuickSlot[i] = quickSlots[i];
        }
    }

    public Item[] GetCurrentQuickSlot()
    {
        return currentQuickSlot;
    }
}
