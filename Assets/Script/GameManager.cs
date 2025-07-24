using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData playerData = new PlayerData();

    //���� ���� ������
    public int Day = 1;
    public float Gold = 0;
    public float Soul = 0;
    public float N_Day_Add_Soul; //���Ͽ� �� �� �ҿ�
    public int N_Day_Time; // ���Ͽ� Ŭ������ ��(�ð�)
    public float N_Day_Cost; //�ش� �����Ͽ� �� ��
    public Item[] currentQuickSlot = new Item[4];
    public int killcount = 0; //óġ�� �Ǳ� ��

    public ItemData[] SlotsData;
    private bool initialized; // �÷��̾� ���� �޾ƿ��� ��
    private void Awake()
    {
        // ���� ���� �ڽŰ� ���� Ÿ���� ������Ʈ�� 2�� �̻� �ִ� ��� ��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "InGame_Scenes")
        {
            initialized = false;
        }
        else if (SceneManager.GetActiveScene().name == "Stage_Scene")
        {
            initialized = true;
        }
        
    }
    private void Update()
    {
        if (!initialized && SceneManager.GetActiveScene().name == "InGame_Scenes")
        {
            Player_Item_Use player_Item_Use = FindObjectOfType<Player_Item_Use>();
            if (player_Item_Use == null) return; // ���� ���� �� �ƴٸ� ���� ������ �ٽ� �õ�

            for (int i = 0; i < player_Item_Use.quickSlots.Length; i++)
            {
                if (SlotsData[i] == null) continue;
                player_Item_Use.quickSlots[i] = new Item(SlotsData[i]);
            }
            initialized = true; // �� �� ���� �� �ٽ� �� �ϵ���
        }
        else if (initialized && SceneManager.GetActiveScene().name == "Stage_Scene")
        {
            ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
            if (shopQuickSlot == null) return; // ���� ���� �� �ƴٸ� ���� ������ �ٽ� �õ�

            for (int i = 0; i < shopQuickSlot.quickSlots.Length; i++)
            {
                if (SlotsData[i] == null) continue;
                shopQuickSlot.SlotsData[i] = SlotsData[i];
            }
            initialized = false; // �� �� ���� �� �ٽ� �� �ϵ���
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
    public void Sub_Gold(float val)//����
    {
        Gold -= val;
    }
    public void Add_Soul(float val)
    {
        Soul += val;
        N_Day_Add_Soul += val;
    }
    public void Sub_Soul(float val)//����
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
        playerData.maxSp = player.maxSp;

        playerData.extraHp = player.extraHp;

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
        for (int i = 0; i < SlotsData.Length; i++)
        {
            if (quickSlots[i] != null && quickSlots[i].ToItemData() != null)
                SlotsData[i] = quickSlots[i].ToItemData();
            else
                SlotsData[i] = null;
        }
    }

    public Item[] GetCurrentQuickSlot()
    {
        return currentQuickSlot;
    }

    public void LoadScene(string loadSceneName)
    {
        // ������ ������ �̵�
        SceneManager.LoadScene(loadSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
