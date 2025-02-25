using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private IMovementController movementController;
    private ICombatController combatController;
    private IStaminaController staminaController;

    public GameObject sword;

    // Variable para almacenar el último movementInput recibido
    public Vector2 currentMovementInput;

    // Variables para estado de combate
    public bool isInCombat = false;
    private float combatTimer = 0f;
    public float combatStateDuration = 5f;

    // Sprint input state, se actualiza con OnSprint
    public bool isSprinting = false;
    // Costo de stamina al correr en combate (por segundo)
    public float sprintStaminaCost = 15f;

    void Awake()
    {
        movementController = GetComponent<MovementController>();
        combatController = GetComponent<CombatController>();
        staminaController = GetComponent<StaminaController>();
    }

    void Update()
    {
        // Actualiza el estado de combate
        if (isInCombat)
        {
            combatTimer -= Time.deltaTime;
            if (combatTimer <= 0f)
            {
                isInCombat = false;
                sword.SetActive(false);
                (movementController as MovementController).forceCursorRotation = false;
            }
        }

        // Si se está corriendo en combate, consumir stamina cada frame.
        if (isInCombat && isSprinting)
        {
            // Consume stamina proporcional al tiempo transcurrido.
            if (!staminaController.ConsumeStamina(sprintStaminaCost * Time.deltaTime))
            {
                // Si no se puede consumir, se desactiva el sprint.
                isSprinting = false;
                (movementController as MovementController)?.SetSprintInput(false);
            }
        }
    }

    // Métodos de input vinculados desde el Player Input
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        currentMovementInput = input;
        movementController.Move(input);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        bool sprinting = context.ReadValueAsButton();
        isSprinting = sprinting;
        (movementController as MovementController)?.SetSprintInput(sprinting);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            movementController.Jump();
            // El salto no activa el estado de combate.
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (combatController.CanDash() && staminaController.ConsumeStamina(20f))
            {
                combatController.Dash(currentMovementInput);
                EnterCombatState();
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (combatController.CanAttack() && staminaController.ConsumeStamina(10f))
            {
                combatController.Attack();
                EnterCombatState();
            }
        }
    }

    private void EnterCombatState()
    {
        isInCombat = true;
        combatTimer = combatStateDuration;
        sword.SetActive(true);
        (movementController as MovementController).forceCursorRotation = true;
    }
}
