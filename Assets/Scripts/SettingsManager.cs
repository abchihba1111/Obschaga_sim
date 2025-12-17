using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Ссылки на слайдеры")]
    public Slider soundSlider;
    public Slider musicSlider;
    public Slider sensitivitySlider;
    public Slider brightnessSlider;

    [Header("Кнопки")]
    public Button applyButton;
    public Button cancelButton;
    public Button backButton;

    [Header("Источники Звуков")]
    public AudioSource[] AudioSources; // Все AudioSource для управления громкостью

    [Header("Настройки яркости")]
    public GameObject brightnessPanel; // Панель для регулировки яркости
    private Image brightnessImagePanel; // Image компонент панели

    [Header("Чувствительность камеры")]
    public float sensitivityCamera = 500f; // Значение чувствительности

    [Header("Другие настройки")]
    public bool isInGameMenu = true; // True для MainScene

    // Значения по умолчанию
    private const float DEFAULT_SOUND_VOLUME = 0.8f;
    private const float DEFAULT_MUSIC_VOLUME = 0.6f;
    private const float DEFAULT_SENSITIVITY = 500f;
    private const float DEFAULT_BRIGHTNESS = 1.0f; // Максимум при запуске

    // Текущие значения (не сохраняются)
    private float currentSoundVolume;
    private float currentMusicVolume;
    private float currentSensitivity;
    private float currentBrightness;

    // Оригинальные значения при открытии панели
    private float originalSoundVolume;
    private float originalMusicVolume;
    private float originalSensitivity;
    private float originalBrightness;

    void Start()
    {
        Debug.Log("=== SettingsManager Initialization ===");

        // Инициализация яркости
        InitializeBrightnessPanel();

        // Устанавливаем значения по умолчанию при запуске
        SetDefaultValues();

        // Настраиваем слайдеры
        SetupSliders();

        // Настраиваем кнопки
        SetupButtons();

        // Применяем начальные настройки сразу
        ApplyInitialSettings();

        Debug.Log("SettingsManager готов - Все настройки установлены по умолчанию");
    }

    void InitializeBrightnessPanel()
    {
        if (brightnessPanel != null)
        {
            brightnessImagePanel = brightnessPanel.GetComponent<Image>();
            if (brightnessImagePanel == null)
            {
                Debug.LogError("У brightnessPanel нет компонента Image!");
            }
            else
            {
                Debug.Log("Brightness panel initialized");
            }
        }
        else
        {
            Debug.LogWarning("Brightness panel not assigned!");
        }
    }

    void SetDefaultValues()
    {
        // Устанавливаем все значения по умолчанию
        currentSoundVolume = DEFAULT_SOUND_VOLUME;
        currentMusicVolume = DEFAULT_MUSIC_VOLUME;
        currentSensitivity = DEFAULT_SENSITIVITY;
        currentBrightness = DEFAULT_BRIGHTNESS; // Максимум при запуске

        // Сохраняем как оригинальные
        originalSoundVolume = currentSoundVolume;
        originalMusicVolume = currentMusicVolume;
        originalSensitivity = currentSensitivity;
        originalBrightness = currentBrightness;
    }

    void SetupSliders()
    {
        Debug.Log("Setting up sliders with real-time application...");

        // ЗВУК - с ДВУМЯ обработчиками
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.RemoveAllListeners();
            soundSlider.onValueChanged.AddListener(OnSoundChanged); // Для обновления текущего значения
            soundSlider.onValueChanged.AddListener(soundOnChangeSlider); // Для реального применения
            soundSlider.value = currentSoundVolume;
            Debug.Log("Sound slider configured");
        }

        // МУЗЫКА
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
            musicSlider.value = currentMusicVolume;
            Debug.Log("Music slider configured");
        }

        // ЧУВСТВИТЕЛЬНОСТЬ - с ДВУМЯ обработчиками
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged); // Для обновления текущего значения
            sensitivitySlider.onValueChanged.AddListener(sensitivityOnChangeSlider); // Для реального применения
            sensitivitySlider.value = currentSensitivity;
            Debug.Log("Sensitivity slider configured");
        }

        // ЯРКОСТЬ - с ДВУМЯ обработчиками
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged); // Для обновления текущего значения
            brightnessSlider.onValueChanged.AddListener(brightnessOnChangeSlider); // Для реального применения
            brightnessSlider.value = currentBrightness; // Устанавливаем максимум
            Debug.Log("Brightness slider configured - set to maximum");
        }
    }

    void SetupButtons()
    {
        Debug.Log("Setting up buttons...");

        // Кнопка Применить
        if (applyButton != null)
        {
            applyButton.onClick.RemoveAllListeners();
            applyButton.onClick.AddListener(ApplySettings);
            Debug.Log("Apply button configured");
        }

        // Кнопка Отмена
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelChanges);
            Debug.Log("Cancel button configured");
        }
    }

    void ApplyInitialSettings()
    {
        Debug.Log("Applying initial settings...");

        // Применяем начальные значения немедленно
        if (soundSlider != null)
        {
            soundOnChangeSlider(soundSlider.value);
            Debug.Log($"Initial sound volume applied: {soundSlider.value}");
        }

        if (musicSlider != null)
        {
            // Если есть отдельный обработчик для музыки, вызовите его здесь
            Debug.Log($"Initial music volume: {musicSlider.value}");
        }

        if (brightnessSlider != null)
        {
            brightnessOnChangeSlider(brightnessSlider.value);
            Debug.Log($"Initial brightness applied: {brightnessSlider.value} (MAXIMUM)");
        }

        if (sensitivitySlider != null)
        {
            sensitivityOnChangeSlider(sensitivitySlider.value);
            Debug.Log($"Initial sensitivity applied: {sensitivitySlider.value}");
        }

        Debug.Log("All initial settings applied");
    }

    // ===== ОБРАБОТЧИКИ ДЛЯ ОБНОВЛЕНИЯ ТЕКУЩИХ ЗНАЧЕНИЙ =====

    void OnSoundChanged(float value)
    {
        // Только обновляем текущее значение, НЕ сохраняем
        currentSoundVolume = value;
        Debug.Log($"Sound volume changed to: {Mathf.RoundToInt(value * 100)}% (not saved)");
    }

    void OnMusicChanged(float value)
    {
        // Только обновляем текущее значение, НЕ сохраняем
        currentMusicVolume = value;
        Debug.Log($"Music volume changed to: {Mathf.RoundToInt(value * 100)}% (not saved)");
    }

    void OnSensitivityChanged(float value)
    {
        // Только обновляем текущее значение, НЕ сохраняем
        currentSensitivity = value;
        Debug.Log($"Mouse sensitivity changed to: {value:F1} (not saved)");
    }

    void OnBrightnessChanged(float value)
    {
        // Только обновляем текущее значение, НЕ сохраняем
        currentBrightness = value;
        Debug.Log($"Brightness changed to: {Mathf.RoundToInt(value * 100)}% (not saved)");
    }

    // ===== РЕАЛЬНОЕ ПРИМЕНЕНИЕ НАСТРОЕК (В РЕАЛЬНОМ ВРЕМЕНИ) =====

    // ГРОМКОСТЬ ЗВУКА - применяется сразу
    public void soundOnChangeSlider(float value)
    {
        Debug.Log($"Applying sound volume: {Mathf.RoundToInt(value * 100)}%");

        // 1. Глобальная громкость
        AudioListener.volume = value;

        // 2. Индивидуальная громкость для каждого AudioSource
        if (AudioSources != null && AudioSources.Length > 0)
        {
            foreach (AudioSource audioSource in AudioSources)
            {
                if (audioSource != null)
                {
                    audioSource.volume = value;
                }
            }
            Debug.Log($"Applied to {AudioSources.Length} AudioSources");
        }
        else
        {
            Debug.LogWarning("No AudioSources assigned in AudioSources array!");

            // Автопоиск AudioSources если не назначены
            AudioSource[] foundSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            if (foundSources.Length > 0)
            {
                Debug.Log($"Found {foundSources.Length} AudioSources in scene");
                foreach (AudioSource audioSource in foundSources)
                {
                    audioSource.volume = value;
                }
            }
        }
    }

    // ЯРКОСТЬ ЭКРАНА - применяется сразу
    public void brightnessOnChangeSlider(float value)
    {
        Debug.Log($"Applying brightness: {Mathf.RoundToInt(value * 100)}%");

        if (brightnessImagePanel != null)
        {
            // Изменяем альфа-канал (прозрачность)
            // Инвертируем: 0 = светлый (прозрачный), 1 = тёмный (непрозрачный)
            var tempColor = brightnessImagePanel.color;
            tempColor.a = 1f - value; // Инвертируем для интуитивного управления
            brightnessImagePanel.color = tempColor;

            Debug.Log($"Brightness panel alpha set to: {tempColor.a}");
        }
        else
        {
            Debug.LogError("brightnessImagePanel is null! Trying to find it...");

            // Попробуем найти заново
            if (brightnessPanel != null)
            {
                brightnessImagePanel = brightnessPanel.GetComponent<Image>();
                if (brightnessImagePanel != null)
                {
                    Debug.Log("Found brightnessImagePanel, applying brightness...");
                    brightnessOnChangeSlider(value); // Рекурсивно вызываем
                }
                else
                {
                    Debug.LogError("Cannot find Image component on brightnessPanel!");
                }
            }
            else
            {
                Debug.LogError("brightnessPanel is not assigned in Inspector!");
            }
        }

        // Для мобильных устройств - системная яркость
#if UNITY_ANDROID || UNITY_IOS
        Screen.brightness = value;
        Debug.Log($"Mobile brightness set to: {value}");
#endif
    }

    // ЧУВСТВИТЕЛЬНОСТЬ МЫШИ - применяется сразу
    public void sensitivityOnChangeSlider(float value)
    {
        Debug.Log($"Applying mouse sensitivity: {value}");

        sensitivityCamera = value;

        // Применяем к скрипту камеры/игрока
        ApplySensitivityToPlayer(value);
    }

    void ApplySensitivityToPlayer(float sensitivity)
    {
        // Ищем скрипт игрока с чувствительностью
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                // Пробуем разные имена полей
                var field = script.GetType().GetField("mouseSensitivity");
                if (field == null) field = script.GetType().GetField("sensitivity");
                if (field == null) field = script.GetType().GetField("lookSensitivity");
                if (field == null) field = script.GetType().GetField("MouseSensitivity");
                if (field == null) field = script.GetType().GetField("sensitivityCamera");

                if (field != null)
                {
                    field.SetValue(script, sensitivity);
                    Debug.Log($"Mouse sensitivity {sensitivity} applied to {script.GetType().Name}");
                    return;
                }
            }
        }

        // Ищем камеру
        GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (cameraObj != null)
        {
            MonoBehaviour[] scripts = cameraObj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                var field = script.GetType().GetField("mouseSensitivity");
                if (field == null) field = script.GetType().GetField("sensitivity");
                if (field != null)
                {
                    field.SetValue(script, sensitivity);
                    Debug.Log($"Mouse sensitivity {sensitivity} applied to camera: {script.GetType().Name}");
                    return;
                }
            }
        }

        Debug.Log($"Mouse sensitivity set to: {sensitivity} (need to implement in player/camera script)");
    }

    // ===== МЕТОДЫ ДЛЯ КНОПОК =====

    public void ApplySettings()
    {
        Debug.Log("ApplySettings called - applying current settings");

        // Просто применяем текущие настройки (ничего не сохраняем)
        if (soundSlider != null)
            soundOnChangeSlider(soundSlider.value);

        if (brightnessSlider != null)
            brightnessOnChangeSlider(brightnessSlider.value);

        if (sensitivitySlider != null)
            sensitivityOnChangeSlider(sensitivitySlider.value);

        Debug.Log("All settings applied (nothing saved permanently)");

        // Если нужно закрыть панель
        if (isInGameMenu)
        {
            Debug.Log("Closing settings panel...");
            // Здесь должен быть вызов метода из PauseMenuManager
        }
    }

    public void CancelChanges()
    {
        Debug.Log("CancelChanges called - restoring original values");

        // Восстанавливаем оригинальные значения (которые были при открытии панели)
        if (soundSlider != null)
            soundSlider.value = originalSoundVolume;

        if (musicSlider != null)
            musicSlider.value = originalMusicVolume;

        if (sensitivitySlider != null)
            sensitivitySlider.value = originalSensitivity;

        if (brightnessSlider != null)
            brightnessSlider.value = originalBrightness;

        // Применяем восстановленные значения
        if (soundSlider != null)
            soundOnChangeSlider(soundSlider.value);

        if (brightnessSlider != null)
            brightnessOnChangeSlider(brightnessSlider.value);

        if (sensitivitySlider != null)
            sensitivityOnChangeSlider(sensitivitySlider.value);

        Debug.Log("All changes cancelled, restored to values when panel was opened");
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
        Debug.Log("Settings panel closed");
    }

    // ===== ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ =====

    // Вызывается при открытии панели
    void OnEnable()
    {
        Debug.Log("Settings panel opened");

        // Сохраняем текущие значения как оригинальные перед изменением
        if (soundSlider != null) originalSoundVolume = soundSlider.value;
        if (musicSlider != null) originalMusicVolume = musicSlider.value;
        if (sensitivitySlider != null) originalSensitivity = sensitivitySlider.value;
        if (brightnessSlider != null) originalBrightness = brightnessSlider.value;

        Debug.Log($"Settings saved for cancel: Sound={originalSoundVolume}, Brightness={originalBrightness}, Sensitivity={originalSensitivity}");

        // Обновляем текущие значения
        currentSoundVolume = originalSoundVolume;
        currentMusicVolume = originalMusicVolume;
        currentSensitivity = originalSensitivity;
        currentBrightness = originalBrightness;
    }

    void OnDisable()
    {
        Debug.Log("Settings panel closed");
        // НИЧЕГО не сохраняем при закрытии
    }

    public void RefreshSliders()
    {
        Debug.Log("Refreshing sliders with default values...");

        // Сбрасываем все значения к значениям по умолчанию
        if (soundSlider != null)
            soundSlider.value = DEFAULT_SOUND_VOLUME;

        if (musicSlider != null)
            musicSlider.value = DEFAULT_MUSIC_VOLUME;

        if (sensitivitySlider != null)
            sensitivitySlider.value = DEFAULT_SENSITIVITY;

        if (brightnessSlider != null)
            brightnessSlider.value = DEFAULT_BRIGHTNESS; // Максимум

        // Применяем значения
        if (soundSlider != null)
            soundOnChangeSlider(soundSlider.value);

        if (brightnessSlider != null)
            brightnessOnChangeSlider(brightnessSlider.value);

        if (sensitivitySlider != null)
            sensitivityOnChangeSlider(sensitivitySlider.value);

        // Обновляем текущие значения
        currentSoundVolume = DEFAULT_SOUND_VOLUME;
        currentMusicVolume = DEFAULT_MUSIC_VOLUME;
        currentSensitivity = DEFAULT_SENSITIVITY;
        currentBrightness = DEFAULT_BRIGHTNESS;

        Debug.Log("All sliders reset to default values");
    }

    // Метод для сброса яркости к максимуму
    public void ResetBrightnessToMaximum()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.value = 1.0f;
            currentBrightness = 1.0f;
            brightnessOnChangeSlider(1.0f);
            Debug.Log("Brightness reset to maximum (100%)");
        }
    }

    // Метод для полного сброса всех настроек к значениям по умолчанию
    public void ResetAllToDefaults()
    {
        Debug.Log("Resetting all settings to defaults...");

        if (soundSlider != null)
            soundSlider.value = DEFAULT_SOUND_VOLUME;

        if (musicSlider != null)
            musicSlider.value = DEFAULT_MUSIC_VOLUME;

        if (sensitivitySlider != null)
            sensitivitySlider.value = DEFAULT_SENSITIVITY;

        if (brightnessSlider != null)
            brightnessSlider.value = DEFAULT_BRIGHTNESS;

        // Применяем значения
        if (soundSlider != null)
            soundOnChangeSlider(soundSlider.value);

        if (brightnessSlider != null)
            brightnessOnChangeSlider(brightnessSlider.value);

        if (sensitivitySlider != null)
            sensitivityOnChangeSlider(sensitivitySlider.value);

        Debug.Log("All settings reset to defaults");
    }
}