using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private IMovementController movementController;
    private ICombatController combatController;
    private IStaminaController staminaController;

    public GameObject sword;

    // Variable to store the last movementInput received
    public Vector2 currentMovementInput;

    // Variables for combat state
    public bool isInCombat = false;
    private float combatTimer = 0f;
    public float combatStateDuration = 5f;

    // Sprint input state, updated with OnSprint
    public bool isSprinting = false;
    // Stamina cost when sprinting in combat (per second)
    public float sprintStaminaCost = 15f;

    void Awake()
    {
        movementController = GetComponent<MovementController>();
        combatController = GetComponent<CombatController>();
        staminaController = GetComponent<StaminaController>();
    }

    void Update()
    {
        // Update the combat state
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

        // If sprinting in combat, consume stamina every frame.
        if (isInCombat && isSprinting)
        {
            // Consume stamina proportional to the time passed.
            if (!staminaController.ConsumeStamina(sprintStaminaCost * Time.deltaTime))
            {
                // If it can't be consumed, disable sprinting.
                isSprinting = false;
                (movementController as MovementController)?.SetSprintInput(false);
            }
        }
    }

    // Input methods bound from Player Input
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
            // Jumping does not activate the combat state.
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
