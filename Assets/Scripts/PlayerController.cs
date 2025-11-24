using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5f;
    public float RotationSpeed = 180f;

    private Animator animator;
    private Rigidbody rb;
    private float moveForward;
    private float rotateInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Если нет Rigidbody - добавляем
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false; // Отключаем гравитацию, так как у нас фиксированная высота
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Запрещаем вращение физикой
        }
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        HandleAnimation();
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
            // Используем физику для движения
            Vector3 movement = transform.forward * moveForward * Speed;
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
        }
        else
        {
            // Останавливаем движение когда нет ввода
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
}