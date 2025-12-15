using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _raycastDistance = 1f;

    private bool isPaused = false;

    void Update()
    {
        // Не обрабатываем ввод если игра на паузе
        if (isPaused) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward,
               out hit, _raycastDistance, _layerMask))
            {
                if (hit.collider.TryGetComponent(out OpenableObject openableObject))
                {
                    openableObject.OpenOrClose();
                }
            }
        }
    }

    // Методы для управления паузой
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        Debug.Log($"PlayerRaycast: пауза = {paused}");
    }
}