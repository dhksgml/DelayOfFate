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
    public float N_Day_current_Soul; //약값을 내기 전 금액
    public int N_Day_Time; // 당일에 클리어한 각(시간)
    public float N_Day_Cost; //해당 정산일에 낼 돈
    public float[] Cost_list = {300,500,1000};
    public Item[] currentQuickSlot = new Item[4];
    public int killcount = 0; //처치한 악귀 수

    public ItemData[] SlotsData;
    private bool initialized; // 플레이어 무기 받아오기 용
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
            if (player_Item_Use == null) return; // 아직 생성 안 됐다면 다음 프레임 다시 시도

            for (int i = 0; i < player_Item_Use.quickSlots.Length; i++)
            {
                if (SlotsData[i] == null) continue;
                player_Item_Use.quickSlots[i] = new Item(SlotsData[i]);
            }
            initialized = true; // 한 번 실행 후 다시 안 하도록
        }
        else if (initialized && SceneManager.GetActiveScene().name == "Stage_Scene")
        {
            ShopQuickSlot shopQuickSlot = FindObjectOfType<ShopQuickSlot>();
            if (shopQuickSlot == null) return; // 아직 생성 안 됐다면 다음 프레임 다시 시도

            for (int i = 0; i < shopQuickSlot.quickSlots.Length; i++)
            {
                if (SlotsData[i] == null) continue;
                shopQuickSlot.SlotsData[i] = SlotsData[i];
            }
            initialized = false; // 한 번 실행 후 다시 안 하도록
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

        Day = 1;
        Gold = 0;
        Soul = 0;
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
        Gold = Mathf.Min(Gold + val, 9999f); // 증가: 최대값 제한
    }

    public void Sub_Gold(float val) // 감소: 최소값 제한
    {
        Gold = Mathf.Max(Gold - val, 0f);
    }

    public void Add_Soul(float val)
    {
        Soul = Mathf.Min(Soul + val, 9999f);
        N_Day_Add_Soul = Mathf.Min(N_Day_Add_Soul + val, 9999f);
    }

    public void Sub_Soul(float val) // 감소
    {
        // 게임 오버를 위해 최소값을 조정함
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
        else if (SceneManager.GetActiveScene().name == "InGame_Scenes")
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(Resources.Load<AudioClip>("BGM/bgm_Search"));
        }
        else if (SceneManager.GetActiveScene().name == "Result_Scene")
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM(Resources.Load<AudioClip>("BGM/bgm_Result"));
        }
    }
    public Item[] GetCurrentQuickSlot()
    {
        return currentQuickSlot;
    }

    public void LoadScene(string loadSceneName)
    {
        // 지정한 씬으로 이동
        SceneManager.LoadScene(loadSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
