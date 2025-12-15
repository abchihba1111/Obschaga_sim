using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Основные ссылки")]
    public GameObject pausePanel;      // Панель паузы
    public GameObject settingsPanel;   // Панель настроек

    [Header("Кнопки паузы")]
    public Button resumeButton;
    public Button settingsButton;
    public Button menuButton;

    [Header("Настройки")]
    public string menuSceneName = "Menu";
    public KeyCode pauseKey = KeyCode.Escape;
    public float inputCooldown = 0.2f;

    private bool isPaused = false;
    private bool isInGameScene = false;
    private bool canProcessInput = true;

    void Start()
    {
        Debug.Log("=== PauseMenuManager Start ===");

        // Проверяем сцену
        isInGameScene = SceneManager.GetActiveScene().name == "MainScene";

        if (!isInGameScene)
        {
            Debug.Log("Не в игровой сцене, отключаю");
            enabled = false;
            return;
        }

        // Инициализация через корутину
        StartCoroutine(InitializeDelayed());
    }

    IEnumerator InitializeDelayed()
    {
        yield return null; // Ждём кадр

        // Автопоиск панелей
        if (pausePanel == null)
        {
            pausePanel = GameObject.Find("PausePanel");
            Debug.Log(pausePanel != null ? "Найден PausePanel" : "PausePanel не найден!");
        }

        if (settingsPanel == null)
        {
            settingsPanel = GameObject.Find("SettingsPanel");
            Debug.Log(settingsPanel != null ? "Найден SettingsPanel" : "SettingsPanel не найден!");
        }

        // Скрываем панели
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Debug.Log("PausePanel скрыт");
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("SettingsPanel скрыт");
        }

        // Находим и настраиваем кнопки
        FindAndSetupButtons();

        // Начальные настройки игры
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Готов. Нажмите ESC для паузы");
    }

    void FindAndSetupButtons()
    {
        // Ищем кнопки в PausePanel
        if (pausePanel != null)
        {
            Button[] buttons = pausePanel.GetComponentsInChildren<Button>(true);

            foreach (Button btn in buttons)
            {
                string btnName = btn.name.ToLower();

                if (btnName.Contains("resume") || btnName.Contains("продолжить"))
                {
                    resumeButton = btn;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(ResumeGame);
                    Debug.Log($"Найдена кнопка Продолжить: {btn.name}");
                }
                else if (btnName.Contains("settings") || btnName.Contains("настройки"))
                {
                    settingsButton = btn;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(OpenSettings);
                    Debug.Log($"Найдена кнопка Настройки: {btn.name}");
                }
                else if (btnName.Contains("menu") || btnName.Contains("меню"))
                {
                    menuButton = btn;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(GoToMainMenu);
                    Debug.Log($"Найдена кнопка В меню: {btn.name}");
                }
            }
        }

        // Проверяем что все кнопки найдены
        if (resumeButton == null) Debug.LogWarning("Кнопка Продолжить не найдена!");
        if (settingsButton == null) Debug.LogWarning("Кнопка Настройки не найдена!");
        if (menuButton == null) Debug.LogWarning("Кнопка В меню не найдена!");
    }

    void Update()
    {
        if (!isInGameScene || !canProcessInput) return;

        if (Input.GetKeyDown(pauseKey))
        {
            StartCoroutine(HandleEscapePress());
        }
    }

    IEnumerator HandleEscapePress()
    {
        canProcessInput = false;

        Debug.Log($"ESC нажата. Пауза: {isPaused}, Настройки: {settingsPanel?.activeSelf}");

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            // Закрываем настройки и ПОКАЗЫВАЕМ ПАУЗУ (не возобновляем игру!)
            Debug.Log("Закрываю настройки, открываю паузу");
            CloseSettingsAndShowPause();
        }
        else
        {
            // Переключаем паузу
            Debug.Log("Переключаю паузу");
            TogglePause();
        }

        yield return new WaitForSecondsRealtime(inputCooldown);
        canProcessInput = true;
    }

    // ===== УПРАВЛЕНИЕ ПАУЗОЙ =====

    void TogglePause()
    {
        isPaused = !isPaused;
        Debug.Log($"Пауза: {isPaused}");

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        // ВРЕМЯ: ставим на паузу или возобновляем
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log($"Time.timeScale = {Time.timeScale}");

        // КУРСОР: показываем только в паузе/настройках
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        // ЗВУК: паузим только когда игра на паузе
        AudioListener.pause = isPaused;
    }

    // ===== МЕТОДЫ ДЛЯ КНОПОК =====

    public void ResumeGame()
    {
        Debug.Log("Кнопка Продолжить нажата");

        // Скрываем панель паузы и возобновляем игру
        if (pausePanel != null) pausePanel.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.pause = false;
    }

    public void OpenSettings()
    {
        Debug.Log("Кнопка Настройки нажата");

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Debug.Log("PausePanel скрыт");
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            Debug.Log("SettingsPanel показан");

            // Время остаётся на паузе, курсор видим
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void CloseSettingsAndShowPause()
    {
        Debug.Log("Закрываю настройки, показываю паузу");

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("SettingsPanel скрыт");
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Debug.Log("PausePanel показан");

            // Остаёмся в паузе! Не возобновляем игру!
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isPaused = true; // Важно!
        }
    }

    public void GoToMainMenu()
    {
        Debug.Log("Кнопка В главное меню нажата");

        // 1. Проверяем существует ли сцена
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName == menuSceneName)
            {
                sceneExists = true;
                break;
            }
        }

        if (!sceneExists)
        {
            Debug.LogError($"Сцена '{menuSceneName}' не найдена в Build Settings!");

            // Показываем доступные сцены
            Debug.Log("Доступные сцены:");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"- {sceneName}");
            }
            return;
        }

        // 2. Возвращаем игру в нормальное состояние
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 3. Загружаем сцену
        Debug.Log($"Загружаю сцену: {menuSceneName}");
        SceneManager.LoadScene(menuSceneName);
    }

    // ===== ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ =====

    // Для кнопки "Назад" в настройках (назначить в Unity)
    public void OnBackButtonClicked()
    {
        Debug.Log("Кнопка Назад в настройках нажата");
        CloseSettingsAndShowPause();
    }

    void OnDestroy()
    {
        // Очищаем слушатели
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (menuButton != null) menuButton.onClick.RemoveAllListeners();
    }
}