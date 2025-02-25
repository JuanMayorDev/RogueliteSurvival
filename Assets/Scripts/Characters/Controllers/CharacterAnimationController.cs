using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimationController : MonoBehaviour
{
    // Reference to the Animator component attached to the character.
    [SerializeField] private Animator animator;

    // Optionally, you can reference the PlayerController to read combat state.
    private PlayerController playerController;

    // You can use the CharacterController's velocity to drive movement animations.
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>(); // Asumiendo que el PlayerController está en el mismo GameObject.
    }

    void Update()
    {
        UpdateMovementAnimation();
        UpdateCombatAnimation();
    }

    /// <summary>
    /// Update the "Speed" parameter based on the character's velocity magnitude.
    /// </summary>
    void UpdateMovementAnimation()
    {
        // Calcula la velocidad usando la magnitud del vector de velocidad.
        float speed = playerController.currentMovementInput.magnitude > 0 ? playerController.isSprinting ? 1 : 0.5f : 0;
        animator.SetFloat("Speed", speed);
    }

    /// <summary>
    /// Update a combat state parameter.
    /// Here we use the state of the sword (active in combat) as an example.
    /// </summary>
    void UpdateCombatAnimation()
    {
        // Por ejemplo, si la espada está activa, asumimos que estamos en combate.
        bool isInCombat = false;
        if (playerController != null)
        {
            isInCombat = playerController.isInCombat;
        }
        animator.SetBool("IsInCombat", isInCombat);
    }

    /// <summary>
    /// Triggers the attack animation.
    /// This method can be called by the CombatController when an attack is performed.
    /// </summary>
    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Triggers the dash animation.
    /// </summary>
    public void PlayDashAnimation()
    {
        animator.SetTrigger("Dash");
    }
}
