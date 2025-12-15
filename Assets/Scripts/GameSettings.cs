using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    // События для уведомления об изменениях
    public System.Action<float> OnSoundVolumeChanged;
    public System.Action<float> OnMouseSensitivityChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDefaultSettings();
            Debug.Log("GameSettings создан и загружен");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadDefaultSettings()
    {
        // Устанавливаем значения по умолчанию если их нет
        if (!PlayerPrefs.HasKey("SoundVolume"))
            PlayerPrefs.SetFloat("SoundVolume", 0.8f);

        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 0.6f);

        if (!PlayerPrefs.HasKey("MouseSensitivity"))
            PlayerPrefs.SetFloat("MouseSensitivity", 2.0f);

        if (!PlayerPrefs.HasKey("Brightness"))
            PlayerPrefs.SetFloat("Brightness", 1.0f);

        PlayerPrefs.Save();
    }

    // Методы для изменения настроек
    public void SetSoundVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SoundVolume", clampedVolume);
        OnSoundVolumeChanged?.Invoke(clampedVolume);
        Debug.Log($"GameSettings: SoundVolume = {clampedVolume}");
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        float clampedSensitivity = Mathf.Clamp(sensitivity, 0.1f, 10f);
        PlayerPrefs.SetFloat("MouseSensitivity", clampedSensitivity);
        OnMouseSensitivityChanged?.Invoke(clampedSensitivity);
        Debug.Log($"GameSettings: MouseSensitivity = {clampedSensitivity}");
    }

    public void SetMusicVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", clampedVolume);
        Debug.Log($"GameSettings: MusicVolume = {clampedVolume}");
    }

    public void SetBrightness(float brightness)
    {
        float clampedBrightness = Mathf.Clamp01(brightness);
        PlayerPrefs.SetFloat("Brightness", clampedBrightness);
        Debug.Log($"GameSettings: Brightness = {clampedBrightness}");

#if UNITY_ANDROID || UNITY_IOS
        Screen.brightness = clampedBrightness;
#endif
    }

    public void SaveAllSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Все настройки сохранены");
    }
}