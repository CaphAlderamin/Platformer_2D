using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject StatsMenu;
    [SerializeField] private GameObject StatsUpMenu;

    private bool statsMenu;
    private bool statsUpMenu;

    public static GameMenuController Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (statsUpMenu)
            {
                CloseStatsUp();
                return;
            }

            if (statsMenu)
            {
                CloseStats();
                return;
            }
            
            if (!GameManager.Instance.GameIsPoused)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }

    public void Pause()
    {
        GameManager.Instance.Pause();
        PauseMenu.SetActive(true);
    }
    public void Continue()
    {
        GameManager.Instance.Continue();
        PauseMenu.SetActive(false);
    }
    public void Restart()
    {
        GameManager.Instance.Restart();
    }
    public void MainMenu()
    {
        GameManager.Instance.MainMenu();
    }
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void OpenStats()
    {
        StatsMenu.SetActive(true);
        statsMenu = true;
        PauseMenu.SetActive(false);
    }
    public void CloseStats()
    {
        PauseMenu.SetActive(true);
        StatsMenu.SetActive(false);
        StatTip.Instance.HideTip();
        statsMenu = false;
    }

    public void OpenStatsUp()
    {
        GameManager.Instance.Pause();
        StatsUpMenu.SetActive(true);
        statsUpMenu = true;
    }
    public void CloseStatsUp()
    {
        GameManager.Instance.SaveData();
        GameManager.Instance.Continue();
        StatsUpMenu.SetActive(false);
        StatUpTip.Instance.HideTip();
        statsUpMenu = false;
    }
}
