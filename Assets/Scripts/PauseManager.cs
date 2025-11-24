using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu; // Перетащите сюда панель меню паузы

    private bool isPaused = false;
    private bool isInGame = false;

    void Start()
    {
        // Проверяем, в какой мы сцене
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            isInGame = true;
            pauseMenu.SetActive(false); // Скрываем меню в игровой сцене при старте
        }
        else
        {
            isInGame = false;
            pauseMenu.SetActive(true); // Показываем меню в главной сцене
        }
    }

    void Update()
    {
        // В игровой сцене ESC показывает/скрывает меню
        if (isInGame && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (pauseMenu.activeSelf)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ContinueGame()
    {
        if (isInGame)
        {
            // Продолжаем игру (скрываем меню)
            isPaused = false;
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }

    public void PauseGame()
    {
        if (isInGame)
        {
            // Ставим на паузу (показываем меню)
            isPaused = true;
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}