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

    [Header("Push Settings")]
    public float pushForce = 2.5f; // Fuerza con la que se empuja el objeto al chocar

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

    // Método que se llama cuando el CharacterController choca con otro collider.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Obtener el Rigidbody del objeto con el que chocamos.
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        // Calcular la dirección del empuje basado en la dirección del movimiento del choque (solo en XZ).
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        // Aplicar la fuerza de empuje.
        body.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}
