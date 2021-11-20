using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IDamageable<int>, IKillable
{
    [Header("Player Parameters")]
    [SerializeField] float currentHealth;
    public float CurrentHealth => currentHealth;
    [SerializeField] float maxHealth;
    public float MaxHealth => maxHealth;

    [SerializeField] float currentStamina;
    public float CurrentStamina => currentStamina;
    [SerializeField] float maxStamina;
    public float MaxStamina => maxStamina;
    [SerializeField] float staminaUsageMultiplier;
    [SerializeField] float staminaRecoveryMultiplier;

    [Header("Events")]
    public UnityEvent TakeDamageEvent;

    [Header("Effects")]
    [SerializeField] AudioClip takeDamageSFX;
    [SerializeField] AudioClip deathSFX;

    [SerializeField] float walkingStepTimer;
    [SerializeField] float runningStepTimer;
    bool takeStep = true;
    [SerializeField] AudioClip moveSFX;


    // local references
    PlayerMovementController playerMovementController;
    CharacterController charController;
    AudioSource audioSource;

    GameUIManager gameUIManager;

    private void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        charController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        gameUIManager = FindObjectOfType<GameUIManager>();

        currentHealth = maxHealth;
        currentStamina = maxStamina;

        takeStep = true;
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.R))
            TakeDamage(10);

        if (playerMovementController.IsSprinting)
            UseStamina();
        else
            RecoverStamina();

        if (charController.isGrounded && charController.velocity.magnitude > 2 && takeStep)
        {
            audioSource.clip = moveSFX;
            audioSource.Play();

            // walking
            if (!playerMovementController.IsSprinting)
                StartCoroutine(WaitForFootsteps(walkingStepTimer));
            // running
            else
                StartCoroutine(WaitForFootsteps(runningStepTimer));
        }
        else
            audioSource.Stop();
    }

    public void TakeDamage(int damageTaken)
    {
        print($"Player took {damageTaken} damage");

        currentHealth -= damageTaken;

        TakeDamageEvent.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    void UseStamina()
    {
        currentStamina -= staminaUsageMultiplier * Time.deltaTime;

        if (currentStamina < 0)
            currentStamina = 0;

        gameUIManager.UpdateStaminaSlider();
    }

    void RecoverStamina()
    {
        currentStamina += staminaRecoveryMultiplier * Time.deltaTime;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        gameUIManager.UpdateStaminaSlider();
    }

    IEnumerator WaitForFootsteps(float walkingStepTimer)
    {
        takeStep = false;

        yield return new WaitForSeconds(walkingStepTimer);

        takeStep = true;
    }

    public void Die()
    {
        print("Player died");
    }
}
