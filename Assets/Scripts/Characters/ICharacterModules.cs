using UnityEngine;

public interface IMovementController
{
    /// <summary>
    /// Processes movement input (e.g., WASD) and applies movement.
    /// </summary>
    /// <param name="input">Vector2 input.</param>
    void Move(Vector2 input);

    /// <summary>
    /// Makes the character jump.
    /// </summary>
    void Jump();
}

public interface ICombatController
{
    /// <summary>
    /// Executes a dash movement.
    /// </summary>
    void Dash(Vector2 movementInput);

    /// <summary>
    /// Executes an attack movement.
    /// </summary>
    void Attack();

    public bool CanAttack();
    public bool CanDash();
}

public interface IStaminaController
{
    float CurrentStamina { get; }
    float MaxStamina { get; }

    /// <summary>
    /// Attempts to consume the specified amount of stamina.
    /// Returns true if successful.
    /// </summary>
    bool ConsumeStamina(float amount);

    /// <summary>
    /// Regenerates stamina over time.
    /// </summary>
    void RegenerateStamina();
}

public interface IHealthController
{
    float CurrentHealth { get; }
    float MaxHealth { get; }

    /// <summary>
    /// Applies damage to the character.
    /// </summary>
    /// <param name="amount">Amount of damage.</param>
    void TakeDamage(float amount);

    /// <summary>
    /// Heals the character.
    /// </summary>
    /// <param name="amount">Amount to heal.</param>
    void Heal(float amount);
}
