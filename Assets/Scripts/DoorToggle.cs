using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Просто по нажатию E в любое время
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed - toggling door");
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        if (!isOpen)
        {
            animator.SetTrigger("Open");
            Debug.Log("Open trigger set");
        }
        else
        {
            animator.SetTrigger("Close");
            Debug.Log("Close trigger set");
        }
        isOpen = !isOpen;
    }

    // Для теста - открывать по клику мыши
    void OnMouseDown()
    {
        Debug.Log("Door clicked");
        ToggleDoor();
    }
}