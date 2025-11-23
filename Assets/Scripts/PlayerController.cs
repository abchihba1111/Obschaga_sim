using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5f;
    public float RotationSpeed = 180f; // Градусов в секунду

    private Animator animator;
    private float moveForward;
    private float rotateInput;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        HandleMovement();
        HandleAnimation();
    }

    void HandleInput()
    {
        // Движение вперед/назад
        moveForward = 0f;
        if (Keyboard.current.wKey.isPressed) moveForward += 1f;
        if (Keyboard.current.sKey.isPressed) moveForward -= 1f;

        // Поворот влево/вправо
        rotateInput = 0f;
        if (Keyboard.current.aKey.isPressed) rotateInput -= 1f;
        if (Keyboard.current.dKey.isPressed) rotateInput += 1f;
    }

    void HandleRotation()
    {
        // Поворачиваем персонажа
        if (rotateInput != 0f)
        {
            float rotation = rotateInput * RotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
    }

    void HandleMovement()
    {
        // Двигаем вперед/назад относительно текущего поворота
        if (moveForward != 0f)
        {
            Vector3 movement = transform.forward * moveForward * Speed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }
    }

    void HandleAnimation()
    {
        if (animator != null)
        {
            // Анимация зависит от движения вперед/назад
            float animationSpeed = Mathf.Abs(moveForward);
            animator.SetFloat("Speed", animationSpeed);
        }
    }
}