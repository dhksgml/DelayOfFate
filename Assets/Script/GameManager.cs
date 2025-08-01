using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData playerData = new PlayerData();

    public bool isTutorial = true;

    //���� ���� ������
    public int Day = 1;
    public float Gold = 0;
    public float Soul = 0;
    public float N_Day_Add_Soul; //���Ͽ� �� �� �ҿ�
    public float N_Day_current_Soul; //�ప�� ���� �� �ݾ�
    public int N_Day_Time; // ���Ͽ� Ŭ������ ��(�ð�)
    public float N_Day_Cost; //�ش� �����Ͽ� �� ��
    public float[] Cost_list = {500,1200,2000};
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
        N_Day_Cost = Cost_list[Day-1];
    }
    public void AlldataReset()
    {
        playerData.maxHp = 100;
        playerData.maxMp = 100;
        playerData.maxSp = 100;
        playerData.currentHp = 100;
        playerData.currentMp = 100;
        playerData.currentSp = 100;
        playerData.flashLightLevel = 1;
        playerData.gold = 0;
        playerData.soul = 0;
        playerData.isFindNearestItem = false;

        Day = 1;
        Gold = 100;
        Soul = 250;
        N_Day_Add_Soul = 0;
        N_Day_current_Soul = 0;
        N_Day_Time = 0;
        N_Day_Cost = 0;
        currentQuickSlot = new Item[4];
        killcount = 0;
        SlotsData = new ItemData[4];
    }
    public void Next_data_reset()
    {
        N_Day_Add_Soul = 0;
        N_Day_Time = 0;
        Day++;
    }
    public void New_Day_date(int time)
    {
        N_Day_Time = time;
        N_Day_current_Soul = Soul;
        Sub_Soul(N_Day_Cost);
    }
    public void Add_Gold(float val)
    {
        Gold = Mathf.Min(Gold + val, 9999f); // ����: �ִ밪 ����
    }

    public void Sub_Gold(float val) // ����: �ּҰ� ����
    {
        Gold = Mathf.Max(Gold - val, 0f);
    }

    public void Add_Soul(float val)
    {
        Soul = Mathf.Min(Soul + val, 9999f);
        N_Day_Add_Soul = Mathf.Min(N_Day_Add_Soul + val, 9999f);
    }

    public void Sub_Soul(float val) // ����
    {
        // ���� ������ ���� �ּҰ��� ������
        Soul = Mathf.Max(Soul - val, -9999f);
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
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Bgm_on();
    }
    public void Bgm_on()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(Resources.Load<AudioClip>("BGM/bgm_Main_Menu"));
            AlldataReset();
        }
        else if (SceneManager.GetActiveScene().name == "Stage_Scene")
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(Resources.Load<AudioClip>("BGM/bgm_Shop"));
        }
        else if (SceneManager.GetActiveScene().name == "InGame_Scenes" || SceneManager.GetActiveScene().name == "Tutorial_Scenes")
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(Resources.Load<AudioClip>("BGM/bgm_Search"));
        }
        else if (SceneManager.GetActiveScene().name == "Result_Scene")
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(Resources.Load<AudioClip>("BGM/bgm_Result"));
        }
        else if (SceneManager.GetActiveScene().name == "Clear_Scene" || SceneManager.GetActiveScene().name == "Gameover_Scene")
        {
            if (SoundManager.Instance != null) SoundManager.Instance.StopBGM();
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
