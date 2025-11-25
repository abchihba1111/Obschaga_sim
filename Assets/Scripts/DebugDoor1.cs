using UnityEngine;

public class DoorDebug : MonoBehaviour
{
    void Update()
    {
        // Тест: открыть дверь по нажатию O (буква O)
        if (Input.GetKeyDown(KeyCode.O))
        {
            DoorController door = GetComponent<DoorController>();
            if (door != null)
            {
                Debug.Log("Принудительное открытие двери через код");
                door.ToggleDoor();
            }
            else
            {
                Debug.LogError("DoorController не найден на этом объекте!");
            }
        }
    }

    // Тест клика мыши
    void OnMouseDown()
    {
        Debug.Log("Клик по двери");
        DoorController door = GetComponent<DoorController>();
        if (door != null)
        {
            door.ToggleDoor();
        }
    }
}