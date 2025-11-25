using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _checkGroundTransform;
    [SerializeField] private LayerMask _groundMask;

    [Header("Settings")]
    [SerializeField] private float _checkRadiusSphere = 0.2f;
    [SerializeField] private float _gravity = -14f;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _speedRun = 7f;

    [Range(1,100)]
    [SerializeField] private float _sensivity = 500f;

    float rotationX;
    bool isGrounded;

    Vector3 velocity;
    Vector3 move;

    void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
       Cursor.visible = false;
    }

    void Update()
    {
        Rotate();
        Move();
        Velocity();
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _sensivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90f);
        _cameraTransform.localRotation = Quaternion.Euler(rotationX,0,0);

        transform.Rotate(0,mouseX,0);
    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        move = transform.forward * moveY + transform.right * moveX;


        if(Input.GetKey(KeyCode.LeftShift) && (moveX != 0 || moveY != 0))
        {
            _characterController.Move(move * _speedRun * Time.deltaTime);
        }
        else
        {
            _characterController.Move(move * _speed * Time.deltaTime);
        }
    }

    private void Velocity()
    {
        isGrounded = Physics.CheckSphere(_checkGroundTransform.position, _checkRadiusSphere, _groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += Time.deltaTime * _gravity;

        _characterController.Move(velocity * Time.deltaTime);
    }
}
