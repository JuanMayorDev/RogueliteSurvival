using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour, IStaminaController
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaRecoveryRate = 10f;
    private float currentStamina;

    // Slider to display the current stamina
    public Slider staminaSlider;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    void Start()
    {
        currentStamina = maxStamina;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }

    void Update()
    {
        RegenerateStamina();
        UpdateStaminaSlider();
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

    private void UpdateStaminaSlider()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}
