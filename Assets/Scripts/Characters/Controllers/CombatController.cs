using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), (typeof(CharacterAnimationController)))]
public class CombatController : MonoBehaviour, ICombatController
{
    private CharacterController characterController;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public AnimationCurve dashCurve;
    private bool isDashing = false;

    [Header("Attack Movement Settings")]
    public float attackMoveDistance = 1f;
    public float attackMoveDuration = 0.15f;
    public AnimationCurve attackMoveCurve;

    [Header("Attack Cooldown")]
    public float attackCooldown = 0.5f;
    private bool isAttackOnCooldown = false;

    private CharacterAnimationController animationController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animationController = GetComponent<CharacterAnimationController>();
    }

    // Ahora Dash recibe el movementInput
    public void Dash(Vector2 movementInput)
    {
        if (!isDashing)
        {
            animationController.PlayDashAnimation();
            StartCoroutine(PerformDash(movementInput));
        }
    }

    // Se añade la comprobación del cooldown en Attack
    public void Attack()
    {
        if (isAttackOnCooldown)
            return; // Si está en cooldown, no se ejecuta el ataque

        isAttackOnCooldown = true;
        if (animationController != null)
        {
            animationController.PlayAttackAnimation();
        }
        StartCoroutine(PerformAttackMove());
        StartCoroutine(AttackCooldownCoroutine());
    }

    // Coroutine para restablecer el cooldown del ataque
    private IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttackOnCooldown = false;
    }

    private IEnumerator PerformDash(Vector2 movementInput)
    {
        isDashing = true;
        Vector3 dashDirection = movementInput.sqrMagnitude > 0.1f ?
            new Vector3(movementInput.x, 0, movementInput.y).normalized : transform.forward;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + dashDirection * dashDistance;
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            float progress = elapsedTime / dashDuration;
            float curveValue = dashCurve.Evaluate(progress);
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            Vector3 moveStep = newPosition - transform.position;
            characterController.Move(moveStep);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Ajusta la posición final de forma exacta
        characterController.Move(targetPosition - transform.position);
        isDashing = false;
    }

    private IEnumerator PerformAttackMove()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + transform.forward * attackMoveDistance;
        float elapsedTime = 0f;
        while (elapsedTime < attackMoveDuration)
        {
            float progress = elapsedTime / attackMoveDuration;
            float curveValue = attackMoveCurve.Evaluate(progress);
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            Vector3 moveStep = newPosition - transform.position;
            characterController.Move(moveStep);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Aseguramos la posición final exacta
        characterController.Move(targetPosition - transform.position);
    }

    public bool CanAttack()
    {
        return !isAttackOnCooldown;
    }

    public bool CanDash()
    {
        return !isDashing;
    }
}
