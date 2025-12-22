using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _doorLayer;
    [SerializeField] private LayerMask _printerLayer;
    [SerializeField] private LayerMask _customerLayer; // Новый слой для клиентов
    [SerializeField] private float _raycastDistance = 3f;

    private bool isPaused = false;

    void Update()
    {
        if (isPaused) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // 1. Проверяем клиента (приоритет)
            RaycastHit customerHit;
            bool hitCustomer = Physics.Raycast(_camera.transform.position, _camera.transform.forward,
               out customerHit, _raycastDistance, _customerLayer);

            if (hitCustomer)
            {
                if (customerHit.collider.TryGetComponent(out CustomerController customer))
                {
                    customer.InteractWithCustomer();
                    return; // Не проверяем дальше если кликнули на клиента
                }
            }

            // 2. Проверяем принтер
            RaycastHit printerHit;
            bool hitPrinter = Physics.Raycast(_camera.transform.position, _camera.transform.forward,
               out printerHit, _raycastDistance, _printerLayer);

            if (hitPrinter)
            {
                if (printerHit.collider.TryGetComponent(out PrinterController printer))
                {
                    printer.InteractWithPrinter();
                    return;
                }
            }

            // 3. Проверяем двери
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

    bool TryInteractWithCustomer()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward,
           out hit, _raycastDistance, _customerLayer))
        {
            Debug.Log($"Raycast попал в: {hit.collider.name}");

            CustomerController customer = hit.collider.GetComponent<CustomerController>();
            if (customer != null)
            {
                Debug.Log("Найден CustomerController");
                customer.InteractWithCustomer();
                return true;
            }
        }
        return false;
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
}