using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;      // Start / Leaderboard / Exit
    public GameObject leaderboardPanel;   // Leaderboard list
    public GameObject routeSelectPanel;   // New panel: all routes

    [Header("Leaderboard UI")]
    public TMP_Text leaderboardText;

    [Header("Route Scenes")]
    [Tooltip("Single game scene that contains all routes.")]
    public string gameSceneName = "GameScene";




    public void Start()
    {
        RouteSelection.SelectedRouteIndex = 0;

        // Default: show main menu only
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (routeSelectPanel != null)
            routeSelectPanel.SetActive(false);
    }

    // -------- Main menu buttons --------

    public void OnClickShowRoutePanel()
    {
        // Switch from main menu to route selection page

        if (routeSelectPanel != null)
            routeSelectPanel.SetActive(true);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

    }

    public void OnClickShowLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (routeSelectPanel != null)
            routeSelectPanel.SetActive(false);

        RefreshLeaderboardText();
    }

    public void OnClickHideLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (routeSelectPanel != null)
            routeSelectPanel.SetActive(false);
    }

    public void OnClickExit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // -------- Route select page buttons --------


    public void OnClickRoute1()
    {
        RouteSelection.SelectedRouteIndex = 0;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickRoute2()
    {
        RouteSelection.SelectedRouteIndex = 1;
        SceneManager.LoadScene(gameSceneName);
    }


    // Back button on route select page
    public void OnClickBackFromRouteSelect()
    {
        if (routeSelectPanel != null)
            routeSelectPanel.SetActive(false);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private void LoadRouteScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Game scene name is empty.");
            return;
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(sceneName);
    }


    // -------- Leaderboard logic (unchanged) --------

    private void RefreshLeaderboardText()
    {
        if (leaderboardText == null)
            return;

        List<LeaderboardEntry> entries = Leaderboard.LoadScores();

        if (entries.Count == 0)
        {
            leaderboardText.text = "No scores yet.";
            return;
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < entries.Count; i++)
        {
            string line = string.Format(
                "{0}. {1} - {2}",
                i + 1,
                entries[i].name,
                FormatTime(entries[i].time)
            );
            sb.AppendLine(line);
        }

        leaderboardText.text = sb.ToString();
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }
}
