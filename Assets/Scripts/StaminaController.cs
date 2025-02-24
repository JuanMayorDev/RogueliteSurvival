using UnityEngine;

public class StaminaController : MonoBehaviour, IStaminaController
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaRecoveryRate = 10f;
    private float currentStamina;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        RegenerateStamina();
    }

    public bool ConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }

    public void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }
}
