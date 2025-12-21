using UnityEngine;

public class CustomerController : MonoBehaviour
{
    private int orderIndex;
    private OrderSystem orderSystem;
    private bool canInteract = false;

    void OnMouseDown()
    {
        if (canInteract && orderSystem != null)
        {
            orderSystem.DeliverOrder(orderIndex);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    public void Setup(int index, OrderSystem system)
    {
        orderIndex = index;
        orderSystem = system;
    }
}