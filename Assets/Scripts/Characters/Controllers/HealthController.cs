using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthController : MonoBehaviour, IHealthController
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth = 100f;

    // Slider to display current health
    public Slider healthSlider;

    // Event to notify death (can be assigned from the Inspector)
    public UnityEvent onDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthSlider();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        if (onDeath != null)
        {
            onDeath.Invoke();
        }
        // Additional death logic (e.g., play animation or restart level)
        gameObject.SetActive(false);
    }
}
