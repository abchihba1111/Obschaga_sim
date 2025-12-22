using UnityEngine;

public class CustomerController : MonoBehaviour
{
    private int orderIndex;
    private OrderSystem orderSystem;
    private bool isActive = false;

    void Start()
    {
        // НЕ скрываем при старте! Оставляем видимым
        // Но делаем его неинтерактивным до вызова

        // Устанавливаем слой Customer
        gameObject.layer = LayerMask.NameToLayer("Customer");

        // Отключаем физику
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Добавляем коллайдер если нет
        if (GetComponent<Collider>() == null)
        {
            BoxCollider trigger = gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1f, 2f, 1f);
        }

        // Скрываем визуально но оставляем в сцене
        SetCustomerVisible(false);
    }

    public void Setup(int index, OrderSystem system)
    {
        orderIndex = index;
        orderSystem = system;
        isActive = true;

        // Показываем клиента
        SetCustomerVisible(true);

        Debug.Log($"Клиент активирован для заказа #{index}");
    }

    void SetCustomerVisible(bool visible)
    {
        // Скрываем/показываем рендерер
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = visible;
        }

        // Скрываем/показываем всех дочерних рендереров
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer childRenderer in childRenderers)
        {
            childRenderer.enabled = visible;
        }

        // Включаем/выключаем коллайдер
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = visible;
        }

        isActive = visible;
    }

    public void InteractWithCustomer()
    {
        if (orderSystem != null && isActive)
        {
            Debug.Log("Взаимодействие с клиентом");
            orderSystem.DeliverOrder(orderIndex);
            SetCustomerVisible(false); // Скрываем после выдачи
        }
    }

    void OnMouseDown()
    {
        InteractWithCustomer();
    }
}