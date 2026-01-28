using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("In-game UI")]
    public TMP_Text timerText;
    public GameObject pauseMenu;

    [Header("Finish UI")]
    public GameObject finishPanel;
    public TMP_Text finalTimeText;
    public InputField nameInputField;

    private float elapsedTime = 0f;
    private bool isRunning = true;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (finishPanel != null)
            finishPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isRunning && !isPaused)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        float seconds = elapsedTime % 60f;
        timerText.text = string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

    public void TogglePause()
    {
        if (!isRunning)
            return;

        isPaused = !isPaused;

        if (pauseMenu != null)
            pauseMenu.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnClickResume()
    {
        if (!isRunning)
            return;

        TogglePause();
    }

    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void OnClickExitToTitle()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("TitleScene");
    }

    // New version: Finish with success flag and final time
    public void Finish(bool success, float finalTime)
    {
        if (!isRunning)
            return;

        isRunning = false;
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (finishPanel != null)
            finishPanel.SetActive(true);

        if (finalTimeText != null)
        {
            string label = success ? "FINISH\nTime: " : "FAILED\nTime: ";
            finalTimeText.text = label + FormatTime(finalTime);
        }

        // Stop background music
        if (BGMManager.Instance != null)
            BGMManager.Instance.StopBGM();
    }

    // Optional helper: expose elapsed time if other scripts need it
    public float ElapsedTime
    {
        get { return elapsedTime; }
    }

    // Backwards-compatible overload for old scripts that call Finish()
    public void Finish()
    {
        // Default: treat as success and use current elapsed time
        Finish(true, elapsedTime);
    }


    public void OnClickSubmitScore()
    {
        string name = (nameInputField != null) ? nameInputField.text : "???";

        if (string.IsNullOrWhiteSpace(name))
            name = "???";

        name = name.Trim().ToUpper();

        if (name.Length > 3)
            name = name.Substring(0, 3);

        Leaderboard.SaveScore(name, elapsedTime);

        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}
