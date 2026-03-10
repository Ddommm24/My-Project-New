using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour, ILoopResettable
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isLow; 
    private bool isDead = false;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 25f;
    public float staminaRegen = 20f;
    public float staminaRecoverThreshold = 0.5f;
    public bool isExhausted;

    [Header("UI")]
    public Slider healthBar;
    public Slider staminaBar;
    public TMP_Text healthText;
    public TMP_Text staminaText;

    [Header("Death Fade")]
    public CanvasGroup deathFade;
    public float fadeDuration = 2f;


    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        healthBar.maxValue = maxHealth;
        staminaBar.maxValue = maxStamina;

        if (deathFade != null)
            deathFade.alpha = 0f;
        }

    void Update()
    {
        HandleStamina();
        UpdateUI();
    }

    void HandleStamina()
    {
        if (isExhausted)
        {
            currentStamina += staminaRegen * Time.deltaTime;

            if (currentStamina >= maxStamina * staminaRecoverThreshold)
            {
                isExhausted = false;
            }
        }
        else
        {
            bool sprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f;

            if (sprinting)
            {
                currentStamina -= staminaDrain * Time.deltaTime;

                if (currentStamina <= 0f)
                {
                    currentStamina = 0f;
                    isExhausted = true;
                }
            }
            else
            {
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    void UpdateUI()
    {
        healthBar.value = currentHealth;
        staminaBar.value = currentStamina;
        staminaBar.fillRect.GetComponent<Image>().color =
        isExhausted ? Color.cyan : Color.green;
        healthBar.fillRect.GetComponent<Image>().color =
        isLow ? Color.yellow : Color.red;

        healthText.color = isLow ? Color.red : Color.white;
        staminaText.alpha = isExhausted ? 0.5f : 1f;


        if (healthText != null)
        healthText.text = $"{Mathf.CeilToInt(currentHealth)}";

    if (staminaText != null)
        staminaText.text = $"{Mathf.CeilToInt(currentStamina)}";

    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f)
        {
            Die();
        }
        else if (currentHealth <= 25f)
        {
            isLow = true;
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died");
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            deathFade.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        deathFade.alpha = 1f;

        yield return new WaitForSeconds(0.3f);

        TimeLoopManager.Instance.ResetLoop();
    }


    public bool CanSprint()
    {
        return !isExhausted && currentStamina > 0f;
    }

    public void ResetState()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        isExhausted = false;
        isLow = false;
        isDead = false;

        if (deathFade != null)
            deathFade.alpha = 0f;

        UpdateUI();
    }

}
