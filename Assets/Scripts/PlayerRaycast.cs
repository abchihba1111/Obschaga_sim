using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _doorLayer;
    [SerializeField] private LayerMask _printerLayer;
    [SerializeField] private float _raycastDistance = 3f;

    private bool isPaused = false;

    void Update()
    {
        if (isPaused) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // 1. Проверяем принтер
            RaycastHit printerHit;
            bool hitPrinter = Physics.Raycast(_camera.transform.position, _camera.transform.forward,
               out printerHit, _raycastDistance, _printerLayer);

            if (hitPrinter)
            {
                if (printerHit.collider.TryGetComponent(out PrinterController printer))
                {
                    Debug.Log("Взаимодействие с принтером");
                    printer.InteractWithPrinter();
                    return; // Не проверяем двери если попали в принтер
                }
            }

            // 2. Проверяем двери
            RaycastHit doorHit;
            bool hitDoor = Physics.Raycast(_camera.transform.position, _camera.transform.forward,
               out doorHit, _raycastDistance, _doorLayer);

            if (hitDoor)
            {
                if (doorHit.collider.TryGetComponent(out OpenableObject openableObject))
                {
                    openableObject.OpenOrClose();
                }
            }
        }
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
}