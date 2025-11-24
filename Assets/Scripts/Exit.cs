using UnityEngine;

public class Exit : MonoBehaviour
{
    public void OnButtonExitClick()
    {
        Debug.Log("Выход из игры");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
