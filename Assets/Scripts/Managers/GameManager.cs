using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [NonSerialized] public bool GameIsPoused = false;
    
    private HeroKnight player;
    private StatLevelSystem statLevelSystem;

    public string SaveFileNameString;

    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        player = PlayerManager.Instance.Player.GetComponent<HeroKnight>();
        statLevelSystem = PlayerManager.Instance.Player.GetComponent<StatLevelSystem>();


        //for testing
        if (DataHolder.SaveFileNameString == null || DataHolder.SaveFileNameString == "")
            DataHolder.SaveFileNameString = "new";

        SaveFileNameString = DataHolder.SaveFileNameString;
        if (DataHolder.NewSaveFile == false)
        {
            LoadData();
        }
        else
        {
            DataHolder.NewSaveFile = false;
            statLevelSystem.SetStandartStats();
            SaveData();
        }
        
    }

    protected void ResumeGame()
    {
        Cursor.visible = false;
        player.enabled = true;
        //pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPoused = false;
    }

    protected void PauseGame()
    {
        Cursor.visible = true;
        player.enabled = false;
        //pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPoused = true;
    }

    public void Pause()
    {
        PauseGame();
    }
    public void Continue()
    {
        ResumeGame();
    }
    public void Restart()
    {
        //ResumeGame();
        //SceneManager.LoadScene(1);
        player.GoldAmountToRelibleGold();
        player.GetHeal(1000);
        SaveData();
        ResumeGame();
        SceneManager.LoadScene("Level0");
    }
    public void MainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextLevel()
    {
        SaveData();
        DataHolder.CurrentLevel++;

        if (SceneUtility.GetBuildIndexByScenePath("Level" + DataHolder.CurrentLevel.ToString()) > 0)
        {
            SceneManager.LoadScene("Level" + DataHolder.CurrentLevel.ToString());
        }
        else
        {
            DataHolder.CurrentLevel = 0;
            SceneManager.LoadScene("Level" + DataHolder.CurrentLevel.ToString());
        }
    }

    public void SaveData()
    {
        SaveDataSystem.SaveData(player, statLevelSystem, SaveFileNameString);
    }

    private void LoadData()
    {
        PlayerData data = SaveDataSystem.LoadData(SaveFileNameString);
        if (data != null)
        {
            Debug.Log("Health: " + data.PlayerCurrentHealthPoints);
            Debug.Log("Coins: " + data.PlayerCoinAmount);
            for (int i = 0; i < 9; i++)
            {
                Debug.Log("StatLevels[" + i + "]: " + data.StatLevels[i]);
            }


            //coins
            player.AddCoins(data.PlayerCoinAmount);

            //stats
            statLevelSystem.SetSaveStats(data.StatLevels);

            //health
            if (DataHolder.CurrentLevel > 0)
                player.CurrentHealthPoints = data.PlayerCurrentHealthPoints;
            else
                player.GetHeal(1000);
            GlobalEventManager.SendHealth();
        }
        else
        {
            Debug.LogError("File not deSerialized");
        }
        
    }

}
