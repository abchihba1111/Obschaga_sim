using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float Speed = 5f;
    public float RotationSpeed = 180f;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactionLayer = 1;

    private Animator animator;
    private Rigidbody rb;
    private Camera playerCamera;
    private float moveForward;
    private float rotateInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        // Если нет Rigidbody - добавляем
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        HandleAnimation();
        HandleDoorInteraction();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        moveForward = 0f;
        if (Keyboard.current.wKey.isPressed) moveForward += 1f;
        if (Keyboard.current.sKey.isPressed) moveForward -= 1f;

        rotateInput = 0f;
        if (Keyboard.current.aKey.isPressed) rotateInput -= 1f;
        if (Keyboard.current.dKey.isPressed) rotateInput += 1f;
    }

    void HandleRotation()
    {
        if (rotateInput != 0f)
        {
            float rotation = rotateInput * RotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
    }

    void HandleMovement()
    {
        if (moveForward != 0f)
        {
            Vector3 movement = transform.forward * moveForward * Speed;
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void HandleAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveForward));
        }
    }

    void HandleDoorInteraction()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            if (hit.collider.CompareTag("Door"))
            {
                Debug.Log("Нажми E чтобы открыть/закрыть дверь");

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    // Ищем DoorController в родительских объектах
                    DoorController door = hit.collider.GetComponentInParent<DoorController>();
                    if (door != null)
                    {
                        door.ToggleDoor();
                        Debug.Log("Дверь найдена и открыта!");
                    }
                    else
                    {
                        Debug.LogError("DoorController не найден на родительских объектах!");
                    }
                }
            }
        }
    }

    // Визуализация луча в редакторе (для отладки)
    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.forward * interactionDistance;
            Gizmos.DrawRay(rayOrigin, rayDirection);
        }
    }
}