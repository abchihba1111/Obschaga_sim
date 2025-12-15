using UnityEngine;

public class SettingsApplier : MonoBehaviour
{
    void Start()
    {
        ApplyAllSettings();
        Debug.Log("Настройки применены в MainScene");
    }

    void ApplyAllSettings()
    {
        // 1. Громкость звука
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.8f);
        AudioListener.volume = soundVolume;

        // 2. Чувствительность мыши (если есть скрипт игрока)
        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);
        ApplyMouseSensitivity(sensitivity);

        // 3. Громкость музыки (если есть)
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        ApplyMusicVolume(musicVolume);

        // 4. Яркость
        float brightness = PlayerPrefs.GetFloat("Brightness", 1.0f);
        ApplyBrightness(brightness);
    }

    void ApplyMouseSensitivity(float sensitivity)
    {
        // Ищем скрипт игрока с чувствительностью
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Исправленная строка:
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour script in scripts)
            {
                System.Reflection.FieldInfo field = script.GetType().GetField("mouseSensitivity");
                if (field == null) field = script.GetType().GetField("sensitivity");
                if (field == null) field = script.GetType().GetField("lookSensitivity");

                if (field != null)
                {
                    field.SetValue(script, sensitivity);
                    Debug.Log($"Чувствительность {sensitivity} применена к {script.GetType().Name}");
                    return;
                }
            }
        }

        // Если не нашли на игроке, ищем во всей сцене
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (MonoBehaviour script in allScripts)
        {
            System.Reflection.FieldInfo field = script.GetType().GetField("mouseSensitivity");
            if (field == null) field = script.GetType().GetField("sensitivity");
            if (field == null) field = script.GetType().GetField("lookSensitivity");

            if (field != null)
            {
                field.SetValue(script, sensitivity);
                Debug.Log($"Чувствительность {sensitivity} применена к {script.GetType().Name} (найден в сцене)");
                break;
            }
        }
    }

    void ApplyMusicVolume(float volume)
    {
        // Ищем все AudioSource с музыкой
        // Исправленная строка:
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource audio in audioSources)
        {
            if (audio.gameObject.CompareTag("Music") ||
                audio.gameObject.name.Contains("Music") ||
                audio.gameObject.name.Contains("Background"))
            {
                audio.volume = volume;
                Debug.Log($"Громкость музыки {volume} применена к {audio.gameObject.name}");
            }
        }
    }

    void ApplyBrightness(float brightness)
    {
#if UNITY_ANDROID || UNITY_IOS
        Screen.brightness = brightness;
#endif
    }
}