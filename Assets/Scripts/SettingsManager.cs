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

    [Header("Настройки")]
    public bool isInGameMenu = true; // True для MainScene

    // Сохранённые значения
    private float originalSoundVolume;
    private float originalMusicVolume;
    private float originalSensitivity;
    private float originalBrightness;

    void Start()
    {
        LoadOriginalValues();
        SetupSliders();
        SetupButtons();

        Debug.Log("SettingsManager инициализирован");
    }

    void LoadOriginalValues()
    {
        originalSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.8f);
        originalMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        originalSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);
        originalBrightness = PlayerPrefs.GetFloat("Brightness", 1.0f);
    }

    void SetupSliders()
    {
        // Настраиваем слайдер звука
        if (soundSlider != null)
        {
            soundSlider.value = originalSoundVolume;
            soundSlider.onValueChanged.AddListener(OnSoundChanged);
        }

        // Настраиваем слайдер музыки
        if (musicSlider != null)
        {
            musicSlider.value = originalMusicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        // Настраиваем слайдер чувствительности
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = originalSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        // Настраиваем слайдер яркости
        if (brightnessSlider != null)
        {
            brightnessSlider.value = originalBrightness;
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }
    }

    void SetupButtons()
    {
       

        // Кнопка Применить (опционально)
        if (applyButton != null)
        {
            applyButton.onClick.RemoveAllListeners();
            applyButton.onClick.AddListener(ApplySettings);
        }

        // Кнопка Отмена (опционально)
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelChanges);
        }
    }

    // Обработчики слайдеров
    void OnSoundChanged(float value)
    {
        AudioListener.volume = value; // Применяем немедленно
        Debug.Log($"Громкость звука: {Mathf.RoundToInt(value * 100)}%");
    }

    void OnMusicChanged(float value)
    {
        Debug.Log($"Громкость музыки: {Mathf.RoundToInt(value * 100)}%");
    }

    void OnSensitivityChanged(float value)
    {
        Debug.Log($"Чувствительность мыши: {value:F1}");
    }

    void OnBrightnessChanged(float value)
    {
#if UNITY_ANDROID || UNITY_IOS
        Screen.brightness = value;
#endif
        Debug.Log($"Яркость экрана: {Mathf.RoundToInt(value * 100)}%");
    }

    // Методы для кнопок
    public void ApplySettings()
    {
        SaveAllSettings();

        // Закрываем панель если в игровом режиме
        if (isInGameMenu)
            ClosePanel();

        Debug.Log("Настройки применены");
    }

    void SaveAllSettings()
    {
        if (soundSlider != null)
        {
            PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
            if (GameSettings.Instance != null)
                GameSettings.Instance.SetSoundVolume(soundSlider.value);
        }

        if (musicSlider != null)
        {
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
            if (GameSettings.Instance != null)
                GameSettings.Instance.SetMusicVolume(musicSlider.value);
        }

        if (sensitivitySlider != null)
        {
            PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
            if (GameSettings.Instance != null)
                GameSettings.Instance.SetMouseSensitivity(sensitivitySlider.value);
        }

        if (brightnessSlider != null)
        {
            PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
            if (GameSettings.Instance != null)
                GameSettings.Instance.SetBrightness(brightnessSlider.value);
        }

        PlayerPrefs.Save();
    }

    public void CancelChanges()
    {
        // Восстанавливаем оригинальные значения
        if (soundSlider != null)
            soundSlider.value = originalSoundVolume;

        if (musicSlider != null)
            musicSlider.value = originalMusicVolume;

        if (sensitivitySlider != null)
            sensitivitySlider.value = originalSensitivity;

        if (brightnessSlider != null)
            brightnessSlider.value = originalBrightness;

        // Закрываем панель если в игровом режиме
        if (isInGameMenu)
            ClosePanel();

        Debug.Log("Изменения отменены");
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
        Debug.Log("Панель настроек закрыта");
    }

    void ReturnToMainMenu()
    {
        // Для главного меню - своя логика
        gameObject.SetActive(false);
        // Здесь можно показать главное меню
    }

    // Обновить значения слайдеров
    public void RefreshSliders()
    {
        LoadOriginalValues();

        if (soundSlider != null)
            soundSlider.value = originalSoundVolume;

        if (musicSlider != null)
            musicSlider.value = originalMusicVolume;

        if (sensitivitySlider != null)
            sensitivitySlider.value = originalSensitivity;

        if (brightnessSlider != null)
            brightnessSlider.value = originalBrightness;
    }
}