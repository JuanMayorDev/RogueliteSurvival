using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour, IMovementController
{
    private CharacterController characterController;
    private Vector3 velocity;
    private float currentSpeed;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 10f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    [Header("Movement Interpolation")]
    public float acceleration = 5f;

    // Flag para forzar la rotación hacia el cursor (por ejemplo, en combate)
    public bool forceCursorRotation = false;

    // Variables para manejo del sprint
    [HideInInspector]
    public bool wasSprintingBeforeJump = false;
    private Vector2 movementInput;
    private bool sprintInput = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        HandleMovement();
        HandleGravity();
        // Si se fuerza la rotación (por ejemplo, en combate), usar la dirección del cursor;
        // de lo contrario, la rotación se basa en el input de movimiento.
        if (forceCursorRotation)
        {
            RotateTowardsCursor();
        }
        else
        {
            HandleRotation();
        }
    }

    public void Move(Vector2 input)
    {
        movementInput = input;
    }

    public void SetSprintInput(bool isSprinting)
    {
        if (characterController.isGrounded)
            sprintInput = isSprinting;
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            // Se guarda si se estaba sprintando al saltar
            wasSprintingBeforeJump = sprintInput;
            velocity.y = jumpForce;
        }
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        bool isGrounded = characterController.isGrounded;
        float targetSpeed = moveSpeed;
        bool canSprint = isGrounded ? sprintInput : wasSprintingBeforeJump;
        if (canSprint)
        {
            targetSpeed = moveSpeed * sprintMultiplier;
        }
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (isGrounded)
        {
            wasSprintingBeforeJump = false;
        }
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
            velocity.y = 0f;
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    // Rotación basada en el input de movimiento (para Exploración)
    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        if (movementInput.magnitude > 0.1f)
        {
            targetDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        }
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Método para rotar hacia la posición del cursor en el plano XZ
    public void RotateTowardsCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 cursorPosition = ray.GetPoint(rayDistance);
            Vector3 direction = (cursorPosition - transform.position).normalized;
            direction.y = 0f;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
