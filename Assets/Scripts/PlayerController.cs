using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private IMovementController movementController;
    private ICombatController combatController;
    private IStaminaController staminaController;

    [SerializeField] private GameObject sword;

    // Variable para almacenar el �ltimo movementInput recibido
    private Vector2 currentMovementInput;

    // Variables para estado de combate
    private bool isInCombat = false;
    private float combatTimer = 0f;
    public float combatStateDuration = 5f;

    void Awake()
    {
        movementController = GetComponent<MovementController>();
        combatController = GetComponent<CombatController>();
        staminaController = GetComponent<StaminaController>();
    }

    void Update()
    {
        if (isInCombat)
        {
            combatTimer -= Time.deltaTime;
            if (combatTimer <= 0f)
            {
                isInCombat = false;
                sword.SetActive(false);
                // Desactivar la rotaci�n por cursor para volver a la rotaci�n por movimiento
                (movementController as MovementController).forceCursorRotation = false;
            }
        }
    }

    // M�todos de input (vinculados desde el Player Input)
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        currentMovementInput = input;
        movementController.Move(input);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        bool isSprinting = context.ReadValueAsButton();
        (movementController as MovementController)?.SetSprintInput(isSprinting);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            movementController.Jump();
            // El salto no activa el estado de combate
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (staminaController.ConsumeStamina(20f))
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
            if (staminaController.ConsumeStamina(10f))
            {
                // Antes de atacar, ya se estar� rotando hacia el cursor
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
        // Forzar la rotaci�n hacia el cursor en combate
        (movementController as MovementController).forceCursorRotation = true;
    }
}
