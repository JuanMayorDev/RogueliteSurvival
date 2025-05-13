using System.Collections;
using UnityEngine;

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

    [Header("Attack Hit Settings")]
    public float hitDamage = 20f;
    public float attackRadius = 0.5f;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;

    private CharacterAnimationController animationController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animationController = GetComponent<CharacterAnimationController>();
    }

    // Now Dash receives the movementInput
    public void Dash(Vector2 movementInput)
    {
        if (!isDashing)
        {
            animationController.PlayDashAnimation();
            StartCoroutine(PerformDash(movementInput));
        }
    }

    // Added cooldown check in Attack
    public void Attack()
    {
        if (isAttackOnCooldown)
            return; // If it's on cooldown, the attack won't execute

        isAttackOnCooldown = true;
        if (animationController != null)
        {
            animationController.PlayAttackAnimation();
        }
        StartCoroutine(PerformAttackMove());
        StartCoroutine(AttackCooldownCoroutine());
    }
    // Coroutine to reset the attack cooldown
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
        // Adjust the final position precisely
        characterController.Move(targetPosition - transform.position);
        isDashing = false;
    }

    private IEnumerator PerformAttackMove()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + transform.forward * attackMoveDistance;
        float elapsedTime = 0f;
        bool didHit = false;

        while (elapsedTime < attackMoveDuration)
        {
            float progress = elapsedTime / attackMoveDuration;
            float curveValue = attackMoveCurve.Evaluate(progress);
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            Vector3 moveStep = newPosition - transform.position;
            characterController.Move(moveStep);
            //Hit once at the middle of the animation.
            if (!didHit && progress >= 0.5f)
            {
                RaycastHit hit;
                Vector3 origin = transform.position + Vector3.up;
                Vector3 direction = transform.forward; 
                if (Physics.SphereCast(origin, attackRadius, direction, out hit, attackRange, enemyLayer))
                {
                    IHealthController enemyHealth = hit.collider.GetComponent<IHealthController>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(hitDamage);
                        Debug.Log("Enemy hit with SphereCast!");
                    }
                    didHit = true;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the exact final position
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
