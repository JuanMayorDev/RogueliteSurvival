using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthController : MonoBehaviour, IHealthController
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth = 100f;

    // Slider para mostrar la vida actual
    public Slider healthSlider;

    // Evento para notificar la muerte (puedes asignarlo desde el Inspector)
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
        // Lógica adicional de muerte (por ejemplo, reproducir animación o reiniciar el nivel)
        gameObject.SetActive(false);
    }
}
